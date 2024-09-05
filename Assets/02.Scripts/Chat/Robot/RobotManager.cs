using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using TMPro;

public class RobotManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public RobotUI robotUI; // RobotUI 참조
    public bool isUIActive = false;

    private OpenAIApi openAI = new OpenAIApi();
    private List<ChatMessage> messages = new List<ChatMessage>();

    private void Start()
    {
        if (inputField != null)
        {
            inputField.onEndEdit.AddListener(HandleInput);
        }
        else
        {
            Debug.LogError("InputField is not assigned.");
        }

        if (robotUI == null)
        {
            Debug.LogError("RobotUI is not assigned in the RobotManager script.");
            return;
        }

        isUIActive = false;
        robotUI.gameObject.SetActive(isUIActive);
    }

    private void Update()
    {
        if (isUIActive && Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleChatUI();
        }
    }

    public void ToggleChatUI()
    {
        isUIActive = !isUIActive;
        robotUI.gameObject.SetActive(isUIActive);

        if (isUIActive)
        {
            DisplayWelcomeMessage();
        }
    }

    private void DisplayWelcomeMessage()
    {
        string welcomeMessage = "안녕하세요! 저는 장영실 백과사전이에요. 무엇이든 물어보세요!";
        robotUI.DisplayMessage(welcomeMessage);
    }

    private void HandleInput(string inputText)
    {
        if (string.IsNullOrWhiteSpace(inputText))
            return;

        string userMessage = $"[사용자] {inputText}";
        robotUI.DisplayMessage(userMessage);

        AskChatGPT(inputText);

        inputField.text = string.Empty; // 입력 필드 초기화
    }

    public async void AskChatGPT(string userMessage)
    {
        ChatMessage systemMessage = new ChatMessage
        {
            Role = "system",
            Content = "한국어로 쉽게 설명하세요. 당신은 장영실 관련 백과사전입니다. 초등학교 저학년 학생들이 이해하기 쉽게, 간단하고 친절하게 한 문장으로 대답하세요."
        };

        ChatMessage userChatMessage = new ChatMessage
        {
            Content = userMessage,
            Role = "user"
        };

        messages.Clear();
        messages.Add(systemMessage);
        messages.Add(userChatMessage);

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

            string responseMessage = $"[검색결과] {chatResponse.Content}";
            robotUI.DisplayMessage(responseMessage);
        }
    }
}
