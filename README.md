# Prerequisites & Setup

## 1.1 Set Up Prefab
Go to `assets/prefab` and grab the MLKit prefab place it in the first scene of your project. 

## 1.2 Set MainTemplate.gradle dependencies.
1. Enable Custom Gradle Template in Unity

Go to \*\*Project Settings > Player > Android Settings > Publishing Settings\*\* and enable \*\*Custom Main Gradle Template\*\* (`mainTemplate.gradle`).



2. Add ML Kit Dependency

Open `Assets/Plugins/Android/mainTemplate.gradle` and add the ML Kit dependency inside the `dependencies` block:


dependencies {
    
   `// ... other dependencies ...`

    implementation 'com.google.mlkit:text-recognition:16.0.1'
    implementation 'com.google.mlkit:text-recognition-chinese:16.0.1'
    implementation 'com.google.mlkit:text-recognition-devanagari:16.0.1'
    implementation 'com.google.mlkit:text-recognition-japanese:16.0.1'
    implementation 'com.google.mlkit:text-recognition-korean:16.0.1'

}


# Tutorial

Call `MLKitOCRWrapper.instance.ScanImageText()` by passing your target `Texture2D`, the `SourceLang` enum converted to an API key, and a callback function to handle the output text:

This project seamlessly pairs with [Yasir Kula's Native Camera for Unity](https://github.com/yasirkula/UnityNativeCamera) to capture high-resolution photos on Android/iOS and immediately extract text.

> ⚠️ **Important:** Set `markTextureNonReadable: false` when calling `NativeCamera.LoadImageAtPath()` so the ML Kit wrapper can access the pixel data!

```csharp
using UnityEngine;
using TMPro;

public class NativeCameraOCRManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private SourceLang sourceLang = SourceLang.japanese;
    [SerializeField] private TMP_Text outputTextUI;

    private Texture2D cachedPhotoTexture;

    public void OpenCameraAndScan()
    {
        // Check if camera is busy
        if (NativeCamera.IsCameraBusy())
        {
            Debug.LogWarning("Camera app is already open or busy!");
            return;
        }

        // 1. Take picture using Native Camera plugin
        NativeCamera.TakePicture((filePath) =>
        {
            if (filePath == null)
            {
                Debug.Log("User exited camera without taking a photo.");
                return;
            }

            Debug.Log($"Photo captured at: {filePath}");

            // Clean up old texture memory to avoid memory leaks
            if (cachedPhotoTexture != null)
            {
                Destroy(cachedPhotoTexture);
            }

            // 2. Load Texture2D (Must set markTextureNonReadable: false for OCR)
            cachedPhotoTexture = NativeCamera.LoadImageAtPath(filePath, maxSize: 2048, markTextureNonReadable: false);

            if (cachedPhotoTexture != null)
            {
                // 3. Convert SourceLang enum & run OCR scan
                string apiKey = MLKitOCRWrapper.GetSourceLangApiKey(sourceLang);

                MLKitOCRWrapper.instance.ScanImageText(cachedPhotoTexture, apiKey, (output) =>
                {
                    Debug.Log($"[OCR Result]: {output}");
                    
                    if (outputTextUI != null)
                    {
                        outputTextUI.text = string.IsNullOrEmpty(output) 
                            ? "No text recognized." 
                            : output;
                    }
                });
            }
            else
            {
                Debug.LogError("Failed to load Texture2D from path.");
            }
        }, maxSize: 2048);
    }

    private void OnDestroy()
    {
        // Clean up texture when object is destroyed
        if (cachedPhotoTexture != null)
        {
            Destroy(cachedPhotoTexture);
        }
    }
}
```
## Supported Languages
```csharp
public enum SourceLang
{
    latin,
    japanese,
    korean,
    chinese,
    devanagari
}
```

## 📥 Download Demo

Try out the app on your Android device:
* [Download latest Demo APK](https://github.com/pixelLogic12/unity-mlkit-ocr/releases/latest/download/Demo.apk)
