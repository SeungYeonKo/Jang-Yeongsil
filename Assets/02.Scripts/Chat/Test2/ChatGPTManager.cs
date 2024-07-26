using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using UnityEngine.Events;
using TMPro;

public class ChatGPTManager : MonoBehaviour
{
    public OnResponseEvent OnResponse;

    [System.Serializable]
    public class OnResponseEvent : UnityEvent<string> { }

    private OpenAIApi openAI = new OpenAIApi();
    private List<ChatMessage> messages = new List<ChatMessage>();

    // InputField에 대한 참조
    public TMP_InputField inputField;

    // Unity가 시작될 때 호출되는 메서드
    void Start()
    {
        if (inputField != null)
        {
            inputField.onEndEdit.AddListener(AskChatGPT);
        }
        else
        {
            Debug.LogError("InputField is not assigned.");
        }
    }

    public async void AskChatGPT(string newText)
    {
        if (string.IsNullOrWhiteSpace(newText))
            return;

        ChatMessage newMessage = new ChatMessage();
        newMessage.Content = newText;
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
        }
    }
}
