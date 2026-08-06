# Prerequisites & Setup



1. Enable Custom Gradle Template in Unity

Go to \*\*Project Settings > Player > Android Settings > Publishing Settings\*\* and enable \*\*Custom Main Gradle Template\*\* (`mainTemplate.gradle`).



2. Add ML Kit Dependency

Open `Assets/Plugins/Android/mainTemplate.gradle` and add the ML Kit dependency inside the `dependencies` block:


dependencies {
    
   // ... other dependencies ...

    implementation 'com.google.mlkit:text-recognition:16.0.1'
    implementation 'com.google.mlkit:text-recognition-chinese:16.0.1'
    implementation 'com.google.mlkit:text-recognition-devanagari:16.0.1'
    implementation 'com.google.mlkit:text-recognition-japanese:16.0.1'
    implementation 'com.google.mlkit:text-recognition-korean:16.0.1'

}

