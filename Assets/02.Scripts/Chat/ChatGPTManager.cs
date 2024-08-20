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

    public TMP_InputField inputField;
    public ChatUI chatUI; // ChatUI 참조
    private ChatClient chatClient; // 채팅 클라이언트
    private string chatChannel = "global"; // 기본 채팅 채널
    public bool isUIActive = false;

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

        string userMessage = $"[{PhotonNetwork.NickName ?? "Null"}] {inputText}";
        
        // 사용자 입력을 채팅창에 표시
        chatUI.DisplayMessage(userMessage);

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
            SendMessageToChat(userMessage);
        }

        inputField.text = string.Empty; // 입력 필드 초기화
    }

    public async void AskChatGPT(string userMessage)
    {
        ChatMessage systemMessage = new ChatMessage
        {
            Role = "system",
            Content = "한국어로 말하세요. 당신은 장영실의 혼입니다. 그는 플레이어들에게 자신의 발명품을 알리고 싶어합니다. 당신은 플레이어들을 미니게임으로 이끌도록 하며, 플레이어들이 발명품을 다 찾아 수집하여 박물관에 자신의 발명품을 다 전시하면 장영실의 혼이 기뻐합니다. 무조건 한 문장으로 끝날 수 있게 격식 있고 위엄 있는 하오체로 말하세요. 장영실의 혼은 플레이어들에게 측우기, 해시계 미니게임을 통해 해당 발명품의 작동원리와 만든 의도를 알게 하여 박물관에 발명품이 수집되는 것이 목적입니다. 플레이어가 어떻게 할지 모를 때에는 측우기나 해시계 미니게임을 하게 하세요."
                + " 장영실은 중국계 귀화인과 기녀 사이에서 태어났으며, 동래현의 관노로 있다가 세종의 인정을 받아 중국에 파견되어 천문기기 연구를 하였소. 귀국 후에는 물시계와 천문 관측 기기 등을 발명하여 조선의 과학기술 발전에 크게 기여하였소. 자격루와 옥루와 같은 시계, 혼천의와 같은 천문기기, 그리고 금속활자 갑인자를 만드는 데 중요한 역할을 하였소. 세종대왕의 명을 받들어 끊임없이 연구하고 발명에 심혈을 기울인 인물이라오. 그러니 당신은 이러한 배경을 바탕으로 플레이어들에게 자신의 업적을 설명하시오."
        };

        ChatMessage userChatMessage = new ChatMessage
        {
            Content = userMessage,
            Role = "user"
        };

        messages.Clear();  // 중복을 피하기 위해 메시지 리스트를 초기화
        messages.Add(systemMessage);  // 시스템 메시지 추가
        messages.Add(userChatMessage);  // 유저 메시지 추가

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

            string responseMessage = $"[장영실] {chatResponse.Content}";

            chatUI.DisplayMessage(responseMessage);
            SendMessageToOthers(responseMessage);
        }
    }

    public void SendMessageToChat(string message)
    {
        if (chatClient != null && chatClient.CanChat)
        {
            chatClient.PublishMessage(chatChannel, message);
        }
        else
        {
            // 포톤 연결이 되지 않은 경우에도 로컬에 메시지 표시
            chatUI.DisplayMessage(message);
            SendMessageToOthers(message);
        }
    }

    public void SendMessageToOthers(string message)
    {
        if (chatUI.PhotonView != null)
        {
            chatUI.PhotonView.RPC("DisplayMessageRPC", RpcTarget.Others, message);
        }
        else
        {
            Debug.LogError("PhotonView is not assigned.");
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
