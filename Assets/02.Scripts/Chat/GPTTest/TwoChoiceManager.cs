using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TwoChoiceManager : MonoBehaviour
{
    public GameObject choicePanel;           // 선택지를 담을 UI 패널
    public TMP_Text questionText;            // 질문을 표시할 텍스트
    public Button choiceButton1;             // 첫 번째 선택지 버튼
    public Button choiceButton2;             // 두 번째 선택지 버튼
    public TMP_Text choiceText1;             // 첫 번째 선택지의 텍스트
    public TMP_Text choiceText2;             // 두 번째 선택지의 텍스트

    private string choice1;                  // 첫 번째 선택지 값
    private string choice2;                  // 두 번째 선택지 값

    void Start()
    {
        // 선택지 패널은 기본적으로 비활성화
        choicePanel.SetActive(false);

        // 선택지 버튼에 대한 리스너 연결
        choiceButton1.onClick.AddListener(() => HandleChoice(choice1));
        choiceButton2.onClick.AddListener(() => HandleChoice(choice2));
    }

    // 이지선다를 보여주는 함수
    public void DisplayTwoChoices(string question, string option1, string option2)
    {
        questionText.text = question;      // 질문 텍스트 설정
        choiceText1.text = option1;        // 첫 번째 선택지 텍스트 설정
        choiceText2.text = option2;        // 두 번째 선택지 텍스트 설정

        choice1 = option1;                 // 선택지 값 저장
        choice2 = option2;

        choicePanel.SetActive(true);       // 선택지 패널을 활성화
    }

    // 선택지가 선택되었을 때 호출되는 함수
    public void HandleChoice(string selectedChoice)
    {
        choicePanel.SetActive(false);      // 선택지가 선택되면 패널을 비활성화

        // ChatGPTManager에서 GPT 대화 수행
        ChatGPTManager chatGPTManager = FindObjectOfType<ChatGPTManager>();
        if (chatGPTManager != null)
        {
            string message = $"[{Photon.Pun.PhotonNetwork.NickName ?? "Null"}] {selectedChoice}에 대해 더 알고 싶소.";
            chatGPTManager.AskChatGPT(message);
        }
        else
        {
            Debug.LogError("ChatGPTManager not found in the scene.");
        }
    }
}
