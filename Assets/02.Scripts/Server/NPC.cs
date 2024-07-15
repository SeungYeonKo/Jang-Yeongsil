using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPC : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public TMP_InputField inputField;
    private ChatGPTAPI chatGPTAPI;

    void Start()
    {
        chatGPTAPI = GetComponent<ChatGPTAPI>();
        inputField.onEndEdit.AddListener(SendMessageOnEnter);
    }

    public void SendMessageOnEnter(string input)
    {
        if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(input))
        {
            StartCoroutine(chatGPTAPI.GetChatResponse(input, OnChatResponseReceived));
            inputField.text = "";
            inputField.ActivateInputField(); // 포커스를 유지하여 연속 입력 가능하게 함
        }
    }

    private void OnChatResponseReceived(string response)
    {
        if (!string.IsNullOrEmpty(response))
        {
            dialogueText.text += "\n" + response;
        }
        else
        {
            dialogueText.text += "\n" + "Something went wrong. Please try again.";
        }
    }
}
