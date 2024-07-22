using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System;

public class ChatUI : MonoBehaviourPunCallbacks
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

        // 입력 필드를 항상 활성화 상태로 설정
        chatInputField.interactable = true;
    }

    private void Update()
    {
        // 입력 필드가 비활성화 상태일 때 키 입력 감지하여 활성화
        if (!chatInputField.isFocused && Input.anyKeyDown)
        {
            chatInputField.interactable = true;
            chatInputField.ActivateInputField();
        }
    }

    private async void OnSendButtonClicked()
    {
        string message = chatInputField.text;
        if (!string.IsNullOrEmpty(message))
        {
            // 사용자가 입력한 메시지를 채팅창에 출력
            DisplayMessage($"[{PhotonNetwork.NickName ?? "Null"}] {message}");

            // 만약 메시지가 /장영실로 시작하는 경우 ChatGPT에 메시지 보내고 응답 받기
            if (message.StartsWith("/장영실"))
            {
                string chatGptMessage = message.Substring(5).Trim();
                if (!string.IsNullOrEmpty(chatGptMessage))
                {
                    try
                    {
                        // ChatGPT에 메시지 보내고 응답 받기
                        string response = await chatManager.SendMessageToChatGPT(chatGptMessage);

                        // ChatGPT의 응답을 모든 플레이어에게 전송
                        photonView.RPC("RPCDisplayMessage", RpcTarget.All, "[장영실] " + response);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Exception: {ex.Message}");
                        DisplayMessage("Error communicating with ChatGPT");
                    }
                }
            }
            else
            {
                // 메시지를 모든 플레이어에게 전송
                photonView.RPC("RPCDisplayMessage", RpcTarget.All, $"[{PhotonNetwork.NickName ?? "Null"}] {message}");
            }

            chatInputField.text = string.Empty; // 메시지를 보낸 후 입력 필드를 초기화
            chatInputField.interactable = false; // 메시지 전송 후 다시 비활성화
        }
    }

    [PunRPC]
    public void RPCDisplayMessage(string message)
    {
        DisplayMessage(message);
    }

    public void DisplayMessage(string message)
    {
        chatDisplayText.text += "\n" + message; // 채팅 내용 추가
    }
}
