using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using OpenAI;

public class RobotManager : MonoBehaviour
{
    public OnResponseEvent OnResponse;

    [System.Serializable]
    public class OnResponseEvent : UnityEvent<string> { }

    private OpenAIApi openAI = new OpenAIApi();
    private List<OpenAI.ChatMessage> messages = new List<OpenAI.ChatMessage>();

    public TMP_InputField inputField;
    public RobotUI robotUI;

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

        if (robotUI == null)
        {
            Debug.LogError("RobotUI is not assigned in the RobotManager script.");
            return;
        }

        // UI를 항상 활성화
        robotUI.gameObject.SetActive(true);

        // 친절한 인사말 출력
        robotUI.DisplayMessage("[장영실 백과사전] 무엇이든 물어보세요! 장영실의 발명품이나 생애에 대해 알고 싶으면 질문해 주세요.");
    }

    private void HandleInput(string inputText)
    {
        if (string.IsNullOrWhiteSpace(inputText))
            return;

        string userMessage = $"[나] {inputText}";

        // 사용자 입력을 채팅창에 표시
        robotUI.DisplayMessage(userMessage);

        AskRobot(inputText);

        inputField.text = string.Empty; // 입력 필드 초기화
    }

    public async void AskRobot(string userMessage)
    {
        // 키워드 기반의 사전 정의된 응답을 확인
        string keywordResponse = GetKeywordResponse(userMessage);
        if (keywordResponse != null)
        {
            robotUI.DisplayMessage($"\n[장영실 백과사전] {keywordResponse}");
            return;
        }

        // ChatGPT 요청을 위한 시스템 메시지
        OpenAI.ChatMessage systemMessage = new OpenAI.ChatMessage
        {
            Role = "system",
            Content = "한국어로 쉽게 설명하세요. 당신은 장영실 관련 백과사전입니다. 초등학교 저학년 학생들이 이해할 수 있게, 간단하고 친절하게 한 문장으로 대답하세요."
        };

        OpenAI.ChatMessage userChatMessage = new OpenAI.ChatMessage
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

            string responseMessage = $"[장영실 백과사전] {chatResponse.Content}";
            robotUI.DisplayMessage(responseMessage);
        }
    }

    private string GetKeywordResponse(string userMessage)
    {
        // 소문자로 변환하여 키워드 검색이 대소문자에 영향을 받지 않도록 함
        string lowerMessage = userMessage.ToLower();

        // 키워드와 대응되는 상세하고 친절한 응답
        if (lowerMessage.Contains("발명품"))
        {
            return "장영실은 해시계, 자격루, 천문도, 그리고 측우기와 같은 다양한 발명품을 만들었습니다.";
        }
        else if (lowerMessage.Contains("생애"))
        {
            return "장영실은 조선 시대의 과학자이자 발명가로, 원래는 천민 신분이었지만 세종대왕의 눈에 띄어 조선 과학 발전에 큰 역할을 했습니다. 그는 중국으로 보내져 과학 기술을 배우고, 돌아와 다양한 발명품을 만들어 조선의 과학을 크게 발전시켰습니다.";
        }
        else if (lowerMessage.Contains("해시계"))
        {
            return "해시계는 해의 그림자로 시간을 측정하는 기구로, 낮 동안 시간을 알 수 있게 해주는 장치입니다. 장영실이 만든 해시계는 조선에서 시간을 측정하는 데 아주 중요한 역할을 했습니다.";
        }
        else if (lowerMessage.Contains("자격루"))
        {
            return "자격루는 물의 흐름을 이용해 시간을 알려주는 자동 시계입니다. 장영실이 만든 자격루는 밤에도 자동으로 시간을 알려줘서, 왕과 신하들이 시간을 잘 알 수 있게 도와줬습니다.";
        }
        else if (lowerMessage.Contains("측우기"))
        {
            return "측우기는 비가 얼마나 오는지를 측정하는 기구입니다. 장영실이 만든 이 기구 덕분에 농사 짓는 사람들이 비의 양을 기록해 농사를 더 잘 지을 수 있었습니다.";
        }

        // 키워드에 해당하지 않으면 null 반환
        return null;
    }
}
