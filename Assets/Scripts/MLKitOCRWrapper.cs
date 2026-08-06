using System;
using UnityEngine;

public class MLKitOCRWrapper : MonoBehaviour
{
    public static MLKitOCRWrapper instance;

    private AndroidJavaObject nativeOcrHelper;

    public enum SourceLang
    {
        latin,
        japanese,
        korean,
        chinese,
        devanagari
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
        
        // Only initialize if running on an actual Android device
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                // Initialize our native Java wrapper helper class
                nativeOcrHelper = new AndroidJavaObject("com.unity3d.player.MLKitOCRHelper");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize native MLKit Java class: {ex.Message}");
            }
        }
    }

    #region IMAGE TO TEXT

    /// <summary>
    /// MLKit ocr source language supports Latin api code("latin"), Japanese("japanese"), Korean("korean"), Chinese("chinese") 
    /// the chinese for some reason didnt work i'll try to do something about in the next update.
    /// and its support Devanagari("devanagari").
    /// </summary>
    public void ScanImageText(Texture2D inputTexture, string sourceLang, Action<string> onScanCompleted)
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            Debug.LogError("ML Kit OCR only runs natively on an Android Device!");
            onScanCompleted?.Invoke("non android detected, ocr wrapper only works on android.");
            return;
        }

        if (inputTexture == null)
        {
            Debug.LogError("OCR Error: Passed texture is empty/null!");
            return;
        }

        // Convert the Unity Texture2D cleanly to a compressed JPG byte array
        byte[] imageBytes = inputTexture.EncodeToJPG(85);

        // Create the asynchronous listener that Android will fire back into
        MLKitCallbackListener listener = new MLKitCallbackListener(onScanCompleted);

        // Call the Java wrapper method asynchronously
        nativeOcrHelper.Call("processImageBytes", imageBytes, sourceLang, listener);
    }

    public static string GetSourceLangApiKey(SourceLang lang)
    {
        switch (lang)
        {
            case SourceLang.latin:
                return "latin";
            case SourceLang.japanese:
                return "japanese";
            case SourceLang.korean:
                return "korean";
            case SourceLang.chinese:
                return "chinese";
            case SourceLang.devanagari:
                return "devanagari";
        }

        return "latin";
    }

    // This handles the communication bridge returning back from Java to C#
    private class MLKitCallbackListener : AndroidJavaProxy
    {
        private Action<string> resultCallback;

        public MLKitCallbackListener(Action<string> callback)
            : base("com.unity3d.player.MLKitOCRHelper$OCRCallback") // Maps directly to our nested Java Interface
        {
            resultCallback = callback;
        }

        // Fired automatically by the Java side if OCR successfully parses words
        public void onSuccess(string recognizedText)
        {
            // Jump back onto Unity's main thread securely
            MobileMainThreadDispatcher.ExecuteOnMainThread(() => {
                resultCallback?.Invoke(recognizedText);
            });
        }

        // Fired automatically by Java if the image processing fails
        public void onFailure(string errorDetails)
        {
            MobileMainThreadDispatcher.ExecuteOnMainThread(() => {
                Debug.LogError($"MLKit Native Failure: {errorDetails}");
                resultCallback?.Invoke($"Error: {errorDetails}");
            });
        }
    }

    #endregion


    #region TRANSLATE

    public void TranslateOnDevice(string text, string sourceLang, string targetLang, Action<string> onSuccess, Action<string> onFailure)
    {
#if UNITY_EDITOR
        Debug.LogWarning("ML Kit cannot run in the Unity Editor. Simulating text return.");
        onSuccess?.Invoke($"[Simulated Offline Output] {text}");
#elif UNITY_ANDROID
        try
        {
            using (AndroidJavaClass helperClass = new AndroidJavaClass("com.unity3d.player.MLKitTranslateHelper"))
            {
                // Create our callback interface proxy
                TranslationCallbackProxy proxy = new TranslationCallbackProxy(onSuccess, onFailure);

                // Call our static Java method
                helperClass.CallStatic("translateText", text, targetLang, proxy);
            }
        }
        catch (Exception ex)
        {
            onFailure?.Invoke($"C# Native Exception: {ex.Message}");
        }
#else
        onFailure?.Invoke("Unsupported platform.");
#endif
    }

    private class TranslationCallbackProxy : AndroidJavaProxy
    {
        private readonly Action<string> _onSuccess;
        private readonly Action<string> _onFailure;

        public TranslationCallbackProxy(Action<string> onSuccess, Action<string> onFailure)
            : base("com.unity3d.player.MLKitTranslateHelper$TranslationCallback")
        {
            _onSuccess = onSuccess;
            _onFailure = onFailure;
        }

        // Must match the exact name and signature of the Java interface method
        public void onResponse(string translatedText)
        {
            _onSuccess?.Invoke(translatedText);
        }

        // Must match the exact name and signature of the Java interface method
        public void onError(string errorMsg)
        {
            _onFailure?.Invoke(errorMsg);
        }
    }

    #endregion
}



