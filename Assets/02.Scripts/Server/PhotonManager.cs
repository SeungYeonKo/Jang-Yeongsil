using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    string _nickname;

    public static PhotonManager Instance;

    //public GameObject StartButton;

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
        //StartButton?.SetActive(false);
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

        // StartButton.SetActive(true);
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
            case "Main":
                PhotonNetwork.LoadLevel("MainScene");
                break;
            case "Test":
                PhotonNetwork.LoadLevel("GJS");
                break;
            case "RainGaugeDescriptionScene":
                StartCoroutine(LoadRainGaugeAfterDelay(3));
                break;
            case "MiniGame2":
                PhotonNetwork.LoadLevel("SundialScene");
                break;
            case "MiniGame3":
                PhotonNetwork.LoadLevel("TowerClimbScene");
                break;
            case "MuseumScene":
                PhotonNetwork.LoadLevel("MuseumScene");
                break;
            case "ClepsydraScene":
                PhotonNetwork.LoadLevel("ClepsydraScene");
                break;
            case "AstronomicalChartScene":
                PhotonNetwork.LoadLevel("AstronomicalChartScene");
                break;
            case "NewRainGauge":
                PhotonNetwork.LoadLevel("NewRainGauge");
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
                    descriptionSceneName = "LoadingScene";
                    break;
                case "MiniGame2":
                    descriptionSceneName = "LoadingScene";
                    break;
                case "MuseumScene":
                    descriptionSceneName = "LoadingScene";
                    break;
                case "Main":
                    descriptionSceneName = "LoadingScene";
                    break;
                case "RainGaugeDescriptionScene":
                    descriptionSceneName = "RainGaugeDescriptionScene";
                    break;
                default:
                    descriptionSceneName = "LoadingScene";
                    break;
            }
            AsyncOperation loadingScene = SceneManager.LoadSceneAsync(descriptionSceneName, LoadSceneMode.Additive);
            yield return loadingScene;
        }
    }

    private IEnumerator LoadRainGaugeAfterDelay(int seconds)
    {
        PhotonNetwork.IsMessageQueueRunning = false;
        yield return new WaitForSeconds(seconds);
        PhotonNetwork.LoadLevel("RainGauge");

        yield return new WaitForSeconds(1);
        PhotonNetwork.IsMessageQueueRunning = true;
    }
    private IEnumerator LoadingSceneAfterDelay(int seconds)
    {
        // Photon의 메시지 큐가 일시적으로 멈추게 함 (로딩 동안 네트워크 메시지 처리를 막기 위해)
        PhotonNetwork.IsMessageQueueRunning = false;

        // 주어진 시간만큼 대기 (여기서는 seconds로 넘어온 값을 사용)
        yield return new WaitForSeconds(seconds);

        // 로딩 씬을 비동기로 로드하고 슬라이더를 업데이트
        AsyncOperation loadingScene = SceneManager.LoadSceneAsync("LoadingScene");

        // 로딩이 완료될 때까지 대기 (이 시점에서 슬라이더로 진행률을 보여줄 수 있음)
        yield return loadingScene;

        // 추가로 1초 대기 후에 메시지 큐 다시 활성화
        yield return new WaitForSeconds(1);
        PhotonNetwork.IsMessageQueueRunning = true;
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