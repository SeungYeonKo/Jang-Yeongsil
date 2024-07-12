using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public Text dialogueText;
    public InputField inputField;
    private ChatGPTAPI chatGPTAPI;

    void Start()
    {
        chatGPTAPI = GetComponent<ChatGPTAPI>();
    }

    public void OnSendButtonClicked()
    {
        string userMessage = inputField.text;
        StartCoroutine(chatGPTAPI.GetChatResponse(userMessage, OnChatResponseReceived));
    }

    private void OnChatResponseReceived(string response)
    {
        if (!string.IsNullOrEmpty(response))
        {
            dialogueText.text = response;
        }
        else
        {
            dialogueText.text = "Something went wrong. Please try again.";
        }
    }
}
