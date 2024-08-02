using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class FireballController : MonoBehaviourPunCallbacks
{
    public PlayableDirector TimelineMaker;
    private string RoomID = "Main";
    private bool isLeavingRoom = false;

    void Start()
    {
        TimelineMaker.Play();
        TimelineMaker.stopped += OnPlayableDirectorStopped;
    }

    void Update()
    {
        // Q 키 입력을 감지
        if (Input.GetKeyDown(KeyCode.Q))
        {
            EndTimelineAndJoinRoom();
        }
    }

    private void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        if (isLeavingRoom)
        {
            return;
        }

        if (TimelineMaker == aDirector)
        {
            LoadLoadingScene();
        }
    }

    private void EndTimelineAndJoinRoom()
    {
        if (!isLeavingRoom)
        {
            isLeavingRoom = true;
            TimelineMaker.Stop(); // 타임라인 중지
            LoadLoadingScene();   // 로딩 씬으로 전환
        }
    }

    private void LoadLoadingScene()
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 20,
            IsVisible = true,
            IsOpen = true,
            EmptyRoomTtl = 1000 * 20,
        };
        PhotonNetwork.JoinOrCreateRoom(RoomID, roomOptions, TypedLobby.Default);
        SceneManager.LoadScene("LoadingScene");
    }
}