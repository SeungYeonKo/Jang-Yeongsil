using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class PlayerChatUI : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI chatText; // 플레이어 채팅 텍스트 UI

    private void Start()
    {
        // 초기화: 자식에서 TextMeshPro-Text 컴포넌트를 찾아서 설정합니다.
        chatText = GetComponentInChildren<TextMeshProUGUI>();
        if (chatText != null)
        {
            chatText.gameObject.SetActive(false); // 시작 시 비활성화 상태로 설정
        }
    }

    // 채팅 메시지 표시 및 숨기기
    public void DisplayChatMessage(string message)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("RPCDisplayChatMessage", RpcTarget.All, message);
        }
    }

    private void Update()
    {
        transform.forward = Camera.main.transform.forward;
    }

    // 모든 클라이언트에게 채팅 메시지를 표시하는 RPC 메서드
    [PunRPC]
    private void RPCDisplayChatMessage(string message)
    {
        if (chatText != null && photonView.IsMine)
        {
            chatText.text = message;
            chatText.gameObject.SetActive(true); // 텍스트 표시

            StartCoroutine(HideChatTextAfterDelay());
        }
    }

    // 일정 시간 후에 채팅 텍스트를 숨기는 코루틴
    private IEnumerator HideChatTextAfterDelay()
    {
        yield return new WaitForSeconds(5f); // 5초 대기

        if (chatText != null)
        {
            chatText.gameObject.SetActive(false); // 텍스트 숨기기
        }
    }
}
