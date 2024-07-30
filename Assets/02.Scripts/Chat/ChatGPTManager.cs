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
            // 사용자 입력을 채팅창에 표시
            string userMessage = $"[{PhotonNetwork.NickName ?? "Null"}] {inputText}";
            chatUI.DisplayMessage(userMessage);
            
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
            SendMessageToChat($"[{PhotonNetwork.NickName ?? "Null"}] {inputText}");
        }

        inputField.text = string.Empty; // 입력 필드 초기화
    }

    public async void AskChatGPT(string userMessage)
    {
        ChatMessage newMessage = new ChatMessage
        {
            Content = userMessage,
            Role = "user"
        };

        messages.Clear();  // 중복을 피하기 위해 메시지 리스트를 초기화
        messages.Add(newMessage);

        CreateChatCompletionRequest request = new CreateChatCompletionRequest
        {
            Messages = messages,
            Model = "gpt-3.5-turbo"
        };

        var response = await openAI.CreateChatCompletion(request);

        if (response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            messages.Add(chatResponse);

            Debug.Log(chatResponse.Content);

            // ChatGPT 응답에 닉네임을 포함하지 않도록 변경
            string responseMessage = $"[장영실] {chatResponse.Content}";

            // ChatGPT 응답을 네트워크로 전송하지 않고 로컬에만 표시
            chatUI.DisplayMessage(responseMessage);
        }
    }

    public void SendMessageToChat(string message)
    {
        if (chatClient != null && chatClient.CanChat)
        {
            chatClient.PublishMessage(chatChannel, message);
            chatUI.DisplayMessage(message);  // 기존 텍스트에 사용자 메시지 추가
        }
        else
        {
            // 포톤 연결이 되지 않은 경우에도 로컬에 메시지 표시
            chatUI.DisplayMessage(message);
        }
    }

    // IChatClientListener 구현
    public void DebugReturn(DebugLevel level, string message) { }
    public void OnChatStateChange(ChatState state) { }
    public new void OnConnected()
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
