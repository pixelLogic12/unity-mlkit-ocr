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

```csharp
using UnityEngine;
using TMPro;

public class OCRDemo : MonoBehaviour
{
    [SerializeField] private Texture2D textureToScan;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private SourceLang sourceLang = SourceLang.Japanese;

    public void PerformOCR()
    {
        // Convert the SourceLang enum to the required ML Kit API key
        string langKey = MLKitOCRWrapper.GetSourceLangApiKey(sourceLang);

        // Run the OCR scan asynchronously
        MLKitOCRWrapper.instance.ScanImageText(textureToScan, langKey, (output) => 
        {
            Debug.Log($"Extracted Text: {output}");
            resultText.text = output;
        });
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
