using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ChatGPTAPI : MonoBehaviour
{
    private const string apiUrl = "https://api.openai.com/v1/chat/completions";
    [SerializeField] private string apiKey = "YOUR_API_KEY_HERE"; // Inspector에서 API 키를 입력하세요

    [System.Serializable]
    public class Message
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    public class ChatRequest
    {
        public string model;
        public List<Message> messages;
        public int max_tokens;
        public float temperature;
    }

    [System.Serializable]
    public class ChatResponse
    {
        public List<Choice> choices;

        [System.Serializable]
        public class Choice
        {
            public Message message;
        }
    }

    public IEnumerator GetChatResponse(string userMessage, System.Action<string> callback)
    {
        List<Message> messages = new List<Message>
        {
            new Message { role = "system", content = "You are a helpful assistant." },
            new Message { role = "user", content = userMessage }
        };

        ChatRequest chatRequest = new ChatRequest
        {
            model = "gpt-3.5-turbo", // 사용하려는 모델의 이름을 입력하세요
            messages = messages,
            max_tokens = 150,
            temperature = 0.7f
        };

        string jsonRequest = JsonUtility.ToJson(chatRequest);
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonRequest);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(postData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                ChatResponse chatResponse = JsonUtility.FromJson<ChatResponse>(request.downloadHandler.text);
                string botReply = chatResponse.choices[0].message.content;
                callback(botReply);
            }
            else
            {
                Debug.LogError($"Error: {request.error}");
                callback(null);
            }
        }
    }
}
