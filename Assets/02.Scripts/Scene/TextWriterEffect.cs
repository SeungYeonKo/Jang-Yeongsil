using System.Collections;
using TMPro;
using UnityEngine;

public class TextWriterEffect : MonoBehaviour
{
    public TextMeshProUGUI DescriptionUI;
    public float delay = 0.1f;

    [TextArea(1, 10)]
    public string FullText;     
    private string currentText = string.Empty;  

    private void Start()
    {
        StartCoroutine(ShowText());
    }

    private IEnumerator ShowText()
    {
        for (int i = 0; i < FullText.Length; i++)
        {
            currentText = FullText.Substring(0, i + 1);
            DescriptionUI.text = currentText;
            yield return new WaitForSeconds(delay);
        }
    }
}
