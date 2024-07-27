using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using UnityEngine.Events;
using System;

public class GptNPC : MonoBehaviour
{
    public OnResponseEvent OnResponse;

    [System.Serializable]
    public class OnResponseEvent : UnityEvent<string> { }

    private OpenAIApi openAI;
    private List<ChatMessage> messages = new List<ChatMessage>();

    private void Awake()
    {
        // 환경 변수에서 API 키 가져오기
        string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("OpenAI API 키가 설정되지 않았습니다. 환경 변수를 확인하세요.");
            return;
        }

        // OpenAIApi 인스턴스 초기화
        openAI = new OpenAIApi(apiKey);
    }

    public async void AskChatGPT(string newText)
    {
        if (openAI == null)
        {
            Debug.LogError("OpenAIApi 인스턴스가 초기화되지 않았습니다.");
            return;
        }

        ChatMessage newMessage = new ChatMessage
        {
            Content = newText,
            Role = "user"
        };

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

            OnResponse.Invoke(chatResponse.Content);
        }
    }
}
