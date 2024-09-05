using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RobotUI : MonoBehaviour
{
    public TMP_InputField chatInputField; // 채팅 입력 필드
    public TextMeshProUGUI chatDisplayText; // 채팅 내용 표시 텍스트
    public Button sendButton; // 전송 버튼
    public RobotManager robotManager; // RobotManager 참조

    private void Start()
    {
        if (robotManager == null)
        {
            Debug.LogError("RobotManager is not assigned in the RobotUI script.");
            return;
        }

        // RobotManager의 inputField를 RobotUI의 chatInputField로 설정
        robotManager.inputField = chatInputField;
        robotManager.OnResponse.AddListener(DisplayMessage);

        sendButton.onClick.AddListener(OnSendButtonClicked); // 전송 버튼 클릭 시 이벤트 등록
        chatInputField.onSubmit.AddListener(delegate { OnSendButtonClicked(); }); // Enter 키로 전송 가능하게 설정
        chatDisplayText.text = string.Empty; // 처음 실행 시 로그 창 초기화
    }

    private void OnSendButtonClicked()
    {
        string message = chatInputField.text;
        if (!string.IsNullOrEmpty(message))
        {
            robotManager.AskRobot(message);
            chatInputField.text = string.Empty; // 메시지를 보낸 후 입력 필드를 초기화
        }
    }

    public void DisplayMessage(string message)
    {
        chatDisplayText.text += message + "\n"; // 메시지를 텍스트 필드에 추가
    }
}
