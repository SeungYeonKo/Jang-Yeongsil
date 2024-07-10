using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    string _nickname;

    public static PhotonManager Instance;

    public GameObject StartButton;

    [HideInInspector]
    public string NextRoomName = string.Empty;
    private bool isLeavingRoom = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartButton?.SetActive(false);
        _nickname = PlayerPrefs.GetString("LoggedInId", "Player");

        PhotonNetwork.GameVersion = "0.0.1";
        PhotonNetwork.NickName = _nickname;

        // Custom Properties 설정
        Hashtable customProperties = new Hashtable { { "Nickname", _nickname } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);

        PhotonNetwork.AutomaticallySyncScene = false;
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;

    }

    public override void OnConnected()
    {
        Debug.Log("네임 서버 접속");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("마스터 서버 접속");
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }


    public override void OnJoinedLobby()
    {
        Debug.Log("로비 입장");

        if (!string.IsNullOrEmpty(NextRoomName))
        {
            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = 20,
                IsVisible = true,
                IsOpen = true,
                EmptyRoomTtl = 1000 * 20,
            };

            PhotonNetwork.JoinOrCreateRoom(NextRoomName, roomOptions, TypedLobby.Default);
            return;
        }

        StartButton.SetActive(true);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("방 생성 성공!");
        Debug.Log($"RoomName: {PhotonNetwork.CurrentRoom.Name}");
        //PhotonNetwork.LoadLevel("VillageScene");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"방 입장 성공! : ({PhotonNetwork.CurrentRoom.Name})");
        Debug.Log($"RoomPlayerCount: {PhotonNetwork.CurrentRoom.PlayerCount}");

        switch (PhotonNetwork.CurrentRoom.Name)
        {
            case "Village":
                PhotonNetwork.LoadLevel("VillageScene");
                break;
            case "MiniGame1":
                PhotonNetwork.LoadLevel("BattleTileScene");
                break;
            case "MiniGame2":
                PhotonNetwork.LoadLevel("FallGuysScene");
                break;
            case "MiniGame3":
                PhotonNetwork.LoadLevel("TowerClimbScene");
                break;
        }
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("랜덤방 입장 실패");
        Debug.Log(message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("방 입장 실패");
        Debug.Log(message);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("방 생성 실패");
        Debug.Log(message);
    }


    public void LeaveAndLoadRoom(string nextRoom)
    {
        NextRoomName = nextRoom;
        StartCoroutine(LeaveRoomAndLoadDescriptionScene());
        PhotonNetwork.LeaveRoom();
    }

    private IEnumerator LeaveRoomAndLoadDescriptionScene()
    {
        string descriptionSceneName;

        if (!string.IsNullOrEmpty(NextRoomName))
        {
            switch (NextRoomName)
            {
                case "MiniGame1":
                    descriptionSceneName = "BattleTileDescriptionScene";
                    break;
                case "MiniGame2":
                    descriptionSceneName = "FallGuysDescriptionScene";
                    break;
                case "MiniGame3":
                    descriptionSceneName = "TowerClimbDescriptionScene";
                    break;
                case "Village":
                    descriptionSceneName = "VillageLoadScene";
                    break;
                default:
                    descriptionSceneName = "VillageLoadScene";
                    break;
            }
            AsyncOperation loadingScene = SceneManager.LoadSceneAsync(descriptionSceneName, LoadSceneMode.Additive);
            yield return loadingScene;
        }
    }

    public override void OnLeftRoom()
    {
        if (!isLeavingRoom)
        {
            return;
        }
        Debug.Log("방을 떠났습니다. 이제 방에 참가하거나 생성합니다: " + NextRoomName);
        isLeavingRoom = false; // 플래그 리셋
        if (!string.IsNullOrEmpty(NextRoomName))
        {
            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = 20,
                IsVisible = true,
                IsOpen = true,
                EmptyRoomTtl = 1000 * 20,
            };
            PhotonNetwork.JoinOrCreateRoom(NextRoomName, roomOptions, TypedLobby.Default);
        }
    }
}