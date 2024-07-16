using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    private ChatClient chatClient;
    private string chatChannel = "global";
    private string chatGptApiKey = "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";
    private string chatGptApiUrl = "https://api.openai.com/v1/engines/davinci-codex/completions";
    public ChatUI chatUI;

    private void Start()
    {
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new Photon.Chat.AuthenticationValues(PhotonNetwork.NickName));
    }

    private void Update()
    {
        chatClient?.Service();
    }

    public void SendMessageToChat(string message)
    {
        if (chatClient != null && chatClient.CanChat)
        {
            chatClient.PublishMessage(chatChannel, message);
        }
    }

    public async Task<string> SendMessageToChatGPT(string message)
    {
        var client = new HttpClient();
        var requestData = new
        {
            prompt = message,
            max_tokens = 150,
            n = 1,
            stop = (string)null,
            temperature = 0.7
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {chatGptApiKey}");

        var response = await client.PostAsync(chatGptApiUrl, content);
        var responseString = await response.Content.ReadAsStringAsync();

        var responseObject = JsonConvert.DeserializeObject<ChatGptResponse>(responseString);

        return responseObject.choices[0].text.Trim();
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
            chatUI.DisplayMessage($"{senders[i]}: {messages[i]}");
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
            public string text;
        }
    }
}
