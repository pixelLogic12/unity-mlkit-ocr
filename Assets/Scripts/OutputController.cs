using UnityEngine;
using System.Collections;
using TMPro;

public class OutputController : MonoBehaviour
{
    public static OutputController instance;
    [SerializeField] TextMeshProUGUI outputTxt;
    [SerializeField] RectTransform outputBg;
    [SerializeField] Vector2 bgPadding;

    [Header("TEST")]
    public bool test;
    public string testText;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Update()
    {
        if (test)
        {
            test = false;
            SetOutput(testText);
        }
    }

    public void SetOutput(string newText)
    {
        // Catch if the new text is null.
        if (string.IsNullOrEmpty(newText))
        {
            Debug.LogError("Output text in empty. the text output cannot be null.");
            outputTxt.text = "";
            outputBg.sizeDelta = Vector2.zero;
            return;
        }

        outputTxt.text = newText;
        StartCoroutine(SetBGSize());
    }

    IEnumerator SetBGSize()
    {
        yield return null;

        // Force update output tmp text mesh update
        outputTxt.ForceMeshUpdate();

        // Set background width base on the output.txt text
        float width = outputTxt.preferredWidth + bgPadding.x;

        // Clamp the new width
        if (width > outputTxt.GetComponent<RectTransform>().rect.width)
            width = outputTxt.GetComponent<RectTransform>().rect.width + bgPadding.x;

        outputBg.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

        // Set background height base on the output.txt text
        float height = outputTxt.preferredHeight + bgPadding.y;
        outputBg.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }
}
