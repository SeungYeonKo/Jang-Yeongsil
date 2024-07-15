using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatUI : MonoBehaviour
{
    public TMP_InputField chatInputField;
    public TextMeshProUGUI chatDisplayText;
    public Button sendButton;
    public ChatManager chatManager;

    private void Start()
    {
        sendButton.onClick.AddListener(OnSendButtonClicked);
    }

    private async void OnSendButtonClicked()
    {
        string message = chatInputField.text;
        if (!string.IsNullOrEmpty(message))
        {
            // 채팅 메시지 보내기
            chatManager.SendMessageToChat(message);

            // ChatGPT에 메시지 보내고 응답 받기
            string response = await chatManager.SendMessageToChatGPT(message);

            // 응답 메시지 표시
            DisplayMessage("ChatGPT: " + response);

            chatInputField.text = string.Empty;
        }
    }

    public void DisplayMessage(string message)
    {
        chatDisplayText.text += "\n" + message;
    }
}
