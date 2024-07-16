using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ChatUI : MonoBehaviour
{
    public TMP_InputField chatInputField; // 채팅 입력 필드
    public TextMeshProUGUI chatDisplayText; // 채팅 내용 표시 텍스트
    public Button sendButton; // 전송 버튼
    public ChatManager chatManager; // ChatManager 참조

    private void Start()
    {
        sendButton.onClick.AddListener(OnSendButtonClicked); // 전송 버튼 클릭 시 이벤트 등록
        chatInputField.onSubmit.AddListener(delegate { OnSendButtonClicked(); }); // Enter 키로 전송 가능하게 설정
        chatDisplayText.text = string.Empty; // 처음 실행 시 로그 창 초기화
    }

    private void Update()
    {
        // 'T' 키를 누르면 입력 필드에 포커스를 설정
        if (Input.GetKeyDown(KeyCode.T))
        {
            chatInputField.ActivateInputField();
        }
    }

    private async void OnSendButtonClicked()
    {
        string message = chatInputField.text;
        if (!string.IsNullOrEmpty(message))
        {
            // 채팅 메시지 보내기
            chatManager.SendMessageToChat(message);

            try
            {
                // ChatGPT에 메시지 보내고 응답 받기
                string response = await chatManager.SendMessageToChatGPT(message);

                // 응답 메시지 표시
                DisplayMessage("ChatGPT: " + response);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception: {ex.Message}");
                DisplayMessage("Error communicating with ChatGPT");
            }

            chatInputField.text = string.Empty; // 메시지를 보낸 후 입력 필드를 초기화
        }
    }

    public void DisplayMessage(string message)
    {
        chatDisplayText.text += "\n" + message; // 채팅 내용 추가
    }
}
