using UnityEngine;
using TMPro;
using Photon.Pun;
using System.Collections;

public class PlayerChatUI : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI chatText; // 플레이어 채팅 텍스트 UI

    private void Awake()
    {
        // 초기화: 자식에서 TextMeshPro-Text 컴포넌트를 찾아서 설정합니다.
        chatText = GetComponentInChildren<TextMeshProUGUI>();
        if (chatText == null)
        {
            Debug.LogError("chatText is not assigned or not found in children.");
        }
        else
        {
            chatText.gameObject.SetActive(false); // 시작 시 비활성화 상태로 설정
        }
    }

    // 채팅 메시지를 표시하고 3초 후에 숨기기
    public void DisplayChatMessage(string message)
    {
        if (photonView.IsMine)
        {
            Debug.Log("Sending message via RPC: " + message); // 디버그 로그 추가
            photonView.RPC("RPCDisplayChatMessage", RpcTarget.All, message);
        }
    }

    private void Update()
    {
        // 카메라를 향해 항상 정면을 바라보도록 설정
        if (Camera.main != null)
        {
            transform.forward = Camera.main.transform.forward;
        }
    }

    // 모든 클라이언트에게 채팅 메시지를 표시하는 RPC 메서드
    [PunRPC]
    private void RPCDisplayChatMessage(string message, PhotonMessageInfo info)
    {
        if (chatText != null)
        {
            string playerName = info.Sender.NickName; // 메시지를 보낸 플레이어의 이름
            string formattedMessage = string.IsNullOrWhiteSpace(message) ? "" : $"[{playerName}]: {message}";

            chatText.text = formattedMessage;
            chatText.gameObject.SetActive(true); // 메시지 표시

            Debug.Log("Displaying message: " + formattedMessage); // 디버그 로그 추가

            StopAllCoroutines(); // 기존 코루틴 중지
            StartCoroutine(HideChatTextAfterDelay());
        }
        else
        {
            Debug.LogError("chatText is null in RPCDisplayChatMessage.");
        }
    }

    // 일정 시간 후에 채팅 텍스트를 숨기는 코루틴
    private IEnumerator HideChatTextAfterDelay()
    {
        yield return new WaitForSeconds(3f); // 3초 대기

        if (chatText != null)
        {
            chatText.gameObject.SetActive(false); // 텍스트 숨기기
        }
    }
}
