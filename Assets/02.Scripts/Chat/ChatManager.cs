using UnityEngine;
using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    private ChatClient chatClient; // 채팅 클라이언트
    private string chatChannel = "global"; // 기본 채팅 채널
    private string chatGptApiKey = ""; // ChatGPT API 키
    private string chatGptApiUrl = "https://api.openai.com/v1/chat/completions";
    public ChatUI chatUI; // ChatUI 참조
    private bool isUIActive = false;

    private void Start()
    {
        isUIActive = false;
        chatUI.gameObject.SetActive(isUIActive);
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new Photon.Chat.AuthenticationValues(PhotonNetwork.NickName));
    }

    private void Update()
    {
        chatClient?.Service();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isUIActive = !isUIActive;
            chatUI.gameObject.SetActive(isUIActive);
        }
    }

    public void SendMessageToChat(string message)
    {
        if (chatClient != null && chatClient.CanChat)
        {
            if (message.StartsWith("/장영실"))
            {
                // ChatGPT API 호출
                var query = message.Substring("/장영실".Length).Trim();
                SendMessageToChatGPT(query).ContinueWith(task => {
                    string response = task.Result;
                    string formattedMessage = $"[장영실] {response}";
                    chatClient.PublishMessage(chatChannel, formattedMessage);
                });
            }
            else
            {
                string formattedMessage = $"[{PhotonNetwork.NickName ?? "Null"}] {message}";
                chatClient.PublishMessage(chatChannel, formattedMessage);
            }
        }
    }

    public async Task<string> SendMessageToChatGPT(string message)
    {
        var client = new HttpClient();
        var requestData = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new { role = "system", content = "You are a helpful assistant." },
                new { role = "user", content = message }
            },
            max_tokens = 150,
            temperature = 0.7
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {chatGptApiKey}");

        try
        {
            var response = await client.PostAsync(chatGptApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<ChatGptResponse>(responseString);
                return responseObject.choices[0].message.content.Trim();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Debug.LogError("Error: API Endpoint not found. Please check the URL.");
                return "Error: API Endpoint not found.";
            }
            else
            {
                Debug.LogError($"Error: {response.StatusCode}");
                return "Error communicating with ChatGPT";
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception: {ex.Message}");
            return "Error communicating with ChatGPT";
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

    [System.Serializable]
    public class ChatGptResponse
    {
        public Choice[] choices;

        [System.Serializable]
        public class Choice
        {
            public Message message;
        }

        [System.Serializable]
        public class Message
        {
            public string role;
            public string content;
        }
    }
}
