using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Android;
using System.IO;
using TMPro;

public class InputController : MonoBehaviour
{
    public static InputController instance;

    public TMP_Dropdown sourceLangDropdown;
    private MLKitOCRWrapper.SourceLang sourceLang = MLKitOCRWrapper.SourceLang.latin;
    private Texture2D cachedPhotoTexture;

    private void Awake()
    {
        if (instance)
            instance = this;
    }

    private void Start()
    {
        sourceLangDropdown.onValueChanged.AddListener(OnSourceLangDropdownValChange);
    }

    #region TAKE PHOTO INPUT

    public void OpenCamera()
    {

        if (NativeCamera.IsCameraBusy())
        {
            Debug.LogWarning("Camera app is already open or busy!");
            return;
        }

        NativeCamera.TakePicture((filePath) =>
        {
            if (filePath != null)
            {
                Debug.Log("Photo successfully captured and saved at: " + filePath);

                if (cachedPhotoTexture != null)
                {
                    Destroy(cachedPhotoTexture);
                }

                // Load the file from disk storage into a Texture2D format
                cachedPhotoTexture = NativeCamera.LoadImageAtPath(filePath, maxSize: 2048, markTextureNonReadable: false);

                if (cachedPhotoTexture != null)
                {
                    Debug.Log("Succeess taked the photo!!!.");

                    Scan(cachedPhotoTexture);
                }
                else
                {

                    Debug.Log("NativeCam return null.");
                }
            }
            else
            {
                Debug.Log("User exited the camera app without snapping a picture.");
            }
        }, maxSize: 2048);
    }

    #endregion

    #region UPLOAD IMAGE TEXTURE INPUT

    public void UploadImage()
    {
        // Define allowed file types (MIME types on Android, UTIs on iOS)
        string[] allowedTypes = new string[] { "image/*"};

        // Open native file picker asynchronously
        NativeFilePicker.PickFile((filePath) =>
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.Log("File selection was cancelled by user.");
                return;
            }

            Debug.Log("Picked file path: " + filePath);

            ProcessFileWithIO(filePath);

        }, allowedTypes);
    }

    private void ProcessFileWithIO(string filePath)
    {
        try
        {
            // 1. Read file bytes using System.IO
            byte[] fileBytes = File.ReadAllBytes(filePath);

            // 2. Create a temporary Texture2D instance (dimensions will auto-resize upon load)
            Texture2D texture = new(2, 2, TextureFormat.RGBA32, false);

            // 3. Load PNG/JPG bytes into the Texture2D
            bool isLoaded = ImageConversion.LoadImage(texture, fileBytes);

            if (isLoaded)
            {
                Scan(texture);
            }
            else
            {
                Debug.LogError("Failed to convert image bytes to Texture2D.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error reading image file: {ex.Message}");
        }
    }

    #endregion

    private void Scan(Texture2D texture)
    {
        MLKitOCRWrapper.instance.ScanImageText(texture, MLKitOCRWrapper.GetSourceLangApiKey(sourceLang), (output) => {
            OutputController.instance.SetOutput(output);
        });
    }

    private void OnSourceLangDropdownValChange(int index)
    {
        switch (index)
        {
            case 0:
                sourceLang = MLKitOCRWrapper.SourceLang.latin;
                break;
            case 1:
                sourceLang = MLKitOCRWrapper.SourceLang.japanese;
                break;
            case 2:
                sourceLang = MLKitOCRWrapper.SourceLang.korean;
                break;
            case 3:
                sourceLang = MLKitOCRWrapper.SourceLang.chinese;
                break;
            case 4:
                sourceLang = MLKitOCRWrapper.SourceLang.devanagari;
                break;
        }
    }
}
