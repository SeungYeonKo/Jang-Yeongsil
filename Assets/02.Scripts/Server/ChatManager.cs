using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class ChatManager : MonoBehaviourPunCallbacks
{
    //public Button SendButton; //채팅 입력버튼
    public TextMeshProUGUI ChatLog; //채팅 내역
    public TMP_InputField ChatInputField; //채팅입력 인풋필드
    ScrollRect scroll_rect = null; //채팅이 많이 쌓일 경우 스크롤바의 위치를 아래로 고정하기 위함

    void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        scroll_rect = GameObject.FindObjectOfType<ScrollRect>();
    }

    /// <summary>
    /// chatterUpdate(); 메소드로 주기적으로 플레이어 리스트를 업데이트하며
    /// input에 포커스가 맞춰져있고 엔터키가 눌려졌을 경우에도 SendButtonOnClicked(); 메소드를 실행.
    /// </summary>
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && !ChatInputField.isFocused) SendButtonOnClicked();
    }

    public void SendButtonOnClicked()
    {
        if (ChatInputField.text.Equals(""))
        {
            Debug.Log("Empty");
            return;
        }
        string msg = string.Format("[{0}] {1}", PhotonNetwork.LocalPlayer.NickName, ChatInputField.text);
        photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, msg);
        ReceiveMsg(msg);
        ChatInputField.ActivateInputField(); // 메세지 전송 후 바로 메세지를 입력할 수 있게 포커스를 Input Field로 옮기는 편의 기능
        ChatInputField.text = "";
    }

    public void GameStart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("OnGameRoom", RpcTarget.AllBuffered);
        }
        else
        {
            Debug.Log("마스터 클라이언트가 아님");
        }
    }

    // 플레이어 포톤 연결되면 다시 설정
    /*public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        string msg = string.Format("<color=#00ff00>[{0}]님이 입장하셨습니다.</color>", newPlayer.NickName);
        ReceiveMsg(msg);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        string msg = string.Format("<color=#ff0000>[{0}]님이 퇴장하셨습니다.</color>", otherPlayer.NickName);
        ReceiveMsg(msg);
    }*/

    [PunRPC]
    public void ReceiveMsg(string msg)
    {
        ChatLog.text += "\n" + msg;
        StartCoroutine(ScrollUpdate());
    }

    [PunRPC]
    public void OnGameRoom()
    {
        PhotonNetwork.LoadLevel(2);
    }

    IEnumerator ScrollUpdate()
    {
        yield return null;
        scroll_rect.verticalNormalizedPosition = 0.0f;
    }
}