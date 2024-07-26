using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using UnityEngine.Events;
using TMPro;
using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;

public class ChatGPTManager : MonoBehaviourPunCallbacks, IChatClientListener
{
    public OnResponseEvent OnResponse;

    [System.Serializable]
    public class OnResponseEvent : UnityEvent<string> { }

    private OpenAIApi openAI = new OpenAIApi();
    private List<ChatMessage> messages = new List<ChatMessage>();

    // InputField에 대한 참조
    public TMP_InputField inputField;
    public ChatUI chatUI; // ChatUI 참조
    private ChatClient chatClient; // 채팅 클라이언트
    private string chatChannel = "global"; // 기본 채팅 채널
    private bool isUIActive = false;

    // Unity가 시작될 때 호출되는 메서드
    void Start()
    {
        if (inputField != null)
        {
            inputField.onEndEdit.AddListener(HandleInput);
        }
        else
        {
            Debug.LogError("InputField is not assigned.");
        }

        if (chatUI == null)
        {
            Debug.LogError("ChatUI is not assigned in the ChatGPTManager script.");
            return;
        }

        isUIActive = false;
        chatUI.gameObject.SetActive(isUIActive);

        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new Photon.Chat.AuthenticationValues(PhotonNetwork.NickName));
    }

    void Update()
    {
        chatClient?.Service();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isUIActive = !isUIActive;
            chatUI.gameObject.SetActive(isUIActive);
        }
    }

    private void HandleInput(string inputText)
    {
        if (string.IsNullOrWhiteSpace(inputText))
            return;

        if (inputText.StartsWith("/장영실"))
        {
            // ChatGPT에 메시지 요청
            string chatGptMessage = inputText.Substring("/장영실".Length).Trim();
            if (!string.IsNullOrEmpty(chatGptMessage))
            {
                AskChatGPT(chatGptMessage);
            }
        }
        else
        {
            // 일반 채팅 메시지 전송
            SendMessageToChat(inputText);
        }

        inputField.text = string.Empty; // 입력 필드 초기화
    }

    public async void AskChatGPT(string userMessage)
    {
        ChatMessage newMessage = new ChatMessage();
        newMessage.Content = userMessage;
        newMessage.Role = "user";

        messages.Add(newMessage);

        CreateChatCompletionRequest request = new CreateChatCompletionRequest();
        request.Messages = messages;
        request.Model = "gpt-3.5-turbo";

        var response = await openAI.CreateChatCompletion(request);

        if (response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            messages.Add(chatResponse);

            Debug.Log(chatResponse.Content);

            OnResponse.Invoke(chatResponse.Content);
            SendMessageToChat($"[ChatGPT] {chatResponse.Content}");
        }
    }

    public void SendMessageToChat(string message)
    {
        if (chatClient != null && chatClient.CanChat)
        {
            string formattedMessage = $"[{PhotonNetwork.NickName ?? "Null"}] {message}";
            chatClient.PublishMessage(chatChannel, formattedMessage);
        }
    }

    // IChatClientListener 구현
    public void DebugReturn(DebugLevel level, string message) { }
    public void OnChatStateChange(ChatState state) { }
    public void OnConnected()
    {
        chatClient.Subscribe(chatChannel);
    }
    public void OnDisconnected() { }
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < messages.Length; i++)
        {
            string formattedMessage = messages[i].ToString(); // 메시지를 그대로 사용
            chatUI.DisplayMessage(formattedMessage);
        }
    }
    public void OnPrivateMessage(string sender, object message, string channelName) { }
    public void OnSubscribed(string[] channels, bool[] results) { }
    public void OnUnsubscribed(string[] channels) { }
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) { }
    public void OnUserSubscribed(string channel, string user) { }
    public void OnUserUnsubscribed(string channel, string user) { }
}
