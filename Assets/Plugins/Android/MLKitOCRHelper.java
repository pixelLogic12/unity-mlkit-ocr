package com.unity3d.player;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import com.google.android.gms.tasks.OnFailureListener;
import com.google.android.gms.tasks.OnSuccessListener;
import com.google.mlkit.vision.common.InputImage;
import com.google.mlkit.vision.text.Text;
import com.google.mlkit.vision.text.TextRecognition;
import com.google.mlkit.vision.text.TextRecognizer;

// Import BOTH options packages:
import com.google.mlkit.vision.text.latin.TextRecognizerOptions;
import com.google.mlkit.vision.text.japanese.JapaneseTextRecognizerOptions;
import com.google.mlkit.vision.text.chinese.ChineseTextRecognizerOptions;
import com.google.mlkit.vision.text.korean.KoreanTextRecognizerOptions;
import com.google.mlkit.vision.text.devanagari.DevanagariTextRecognizerOptions;

public class MLKitOCRHelper {

    public interface OCRCallback {
        void onSuccess(String text);
        void onFailure(String error);
    }

    public void processImageBytes(final byte[] byteData, final String language, final OCRCallback callback) {
        try {
            // 1. Decode byte array to Bitmap
            Bitmap bitmap = BitmapFactory.decodeByteArray(byteData, 0, byteData.length);
            if (bitmap == null) {
                callback.onFailure("Failed to decode raw byte array into Android Bitmap.");
                return;
            }

            InputImage image = InputImage.fromBitmap(bitmap, 0);
           TextRecognizer recognizer;

            // Select the correct client based on the requested source script
            if (language != null && language.equalsIgnoreCase("japanese")) {
                recognizer = TextRecognition.getClient(new JapaneseTextRecognizerOptions.Builder().build());
            } else if (language != null && language.equalsIgnoreCase("chinese")) {
                recognizer = TextRecognition.getClient(new ChineseTextRecognizerOptions.Builder().build());
            } else if (language != null && language.equalsIgnoreCase("korean")) {
                recognizer = TextRecognition.getClient(new KoreanTextRecognizerOptions.Builder().build());
            } else if (language != null && language.equalsIgnoreCase("devanagari")) {
                recognizer = TextRecognition.getClient(new DevanagariTextRecognizerOptions.Builder().build());
            } else {
                // Default to Latin (handles English, Spanish, French, etc.)
                recognizer = TextRecognition.getClient(TextRecognizerOptions.DEFAULT_OPTIONS);
            }

            // 3. Process the image
            recognizer.process(image)
                .addOnSuccessListener(new OnSuccessListener<Text>() {
                    @Override
                    public void onSuccess(Text visionText) {
                        callback.onSuccess(visionText.getText());
                    }
                })
                .addOnFailureListener(new OnFailureListener() {
                    @Override
                    public void onFailure(Exception e) {
                        callback.onFailure(e.getMessage());
                    }
                });

        } catch (Exception e) {
            callback.onFailure("Java Runtime Exception: " + e.getMessage());
        }
    }
}