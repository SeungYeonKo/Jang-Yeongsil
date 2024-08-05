using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public enum GameState
{
    Ready,
    Loading,
    Go,
    Over,
}
public class RainGaugeManager : MonoBehaviourPunCallbacks
{
    public static RainGaugeManager Instance { get; private set; }

    private float _gameDuration = 30f;
    public float TimeRemaining;

    private int _countDown = 5; // 시작 카운트다운
    private int _countEnd = 5; // 종료 후 대기
    private bool _isGameOver = false;
    private bool _isStartCoroutine = false;

    public GameState CurrentGameState = GameState.Ready;

    public GameObject[] playerUI; // 0번 인덱스부터 4개의 플레이어 UI를 배열로 관리
    private int playerNumber;
    private Dictionary<int, RainGaugePlayer> players = new Dictionary<int, RainGaugePlayer>();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DisableAllUI();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void AssignUI(int playerNumber)
    {
        for (int i = 0; i < playerNumber && i < playerUI.Length; i++)
        { 
            playerUI[i].SetActive(true);
        }
    }
    public void RegisterPlayer(RainGaugePlayer player)
    {
        if (!players.ContainsKey(player.MyNum))
        {
            players[player.MyNum] = player;
            Debug.Log($"Player {player.MyNum} registered.");
            AssignUI(player.MyNum);
        }
    }
    public void DisableAllUI()
    {
        foreach (GameObject ui in playerUI)
        {
            ui.SetActive(false);
        }
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        if (PhotonNetwork.IsMasterClient)
        {
            AssignPlayerNumber(newPlayer);
        }
        Debug.Log($"{newPlayer}님이 입장했습니다.");
        Debug.Log($"{PhotonNetwork.PlayerList}");
        UpdateAllPlayerUI();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        UpdateAllPlayerUI();
    }
 
    private void AssignPlayerNumber(Photon.Realtime.Player player)
    {
        // 사용 중인 번호를 추적하기 위해 HashSet 사용
        HashSet<int> usedNumbers = new HashSet<int>();
        foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
        {
            if (p.CustomProperties.TryGetValue("PlayerNumber", out object playerNumberObj))
            {
                usedNumbers.Add((int)playerNumberObj);
            }
        }

        // 사용되지 않은 가장 작은 번호 찾기
        int newPlayerNumber = 0;
        for (int i = 0; i < 4; i++) // 최대 플레이어 수 4로 가정
        {
            if (!usedNumbers.Contains(i))
            {
                newPlayerNumber = i;
                break;
            }
        }

        // 새로운 플레이어에게 번호 할당
        Hashtable props = new Hashtable
        {
            { "PlayerNumber", newPlayerNumber }
        };
        player.SetCustomProperties(props); // Custom Properties 설정 및 동기화
    }

    private void UpdateAllPlayerUI()
    {
        DisableAllUI(); // 모든 UI를 비활성화

        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("PlayerNumber", out object playerNumberObj))
            {
                int playerNumber = (int)playerNumberObj;
                AssignUI(playerNumber); // 각 플레이어의 UI를 활성화
                Debug.Log($"{playerNumber}");
            }
        }
    }
    public RainGaugePlayer GetPlayer(int playerNumber)
    {
        players.TryGetValue(playerNumber, out RainGaugePlayer player);
        return player;
    }
    public override void OnEnable()
    {
        InitializeGame();
    }

    public override void OnDisable()
    {
        InitializeGame();

        if (PhotonNetwork.LocalPlayer != null)
        {
            PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
        }
    }

    private void InitializeGame()
    {
        CurrentGameState = GameState.Ready;
        TimeRemaining = _gameDuration;
        _isGameOver = false;
        _isStartCoroutine = false;

        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            player.SetCustomProperties(new Hashtable { { "IsReady_RainGauge", false } });
        }
    }

    private void Update()
    {
        switch (CurrentGameState)
        {
            case GameState.Ready:
                if (AreAllPlayersReady())
                {
                    SetGameState(GameState.Loading);
                }
                break;

            case GameState.Loading:
                break;

            case GameState.Go:
                UpdateGameTimer();
                if (TimeRemaining > 0)
                {
                    JarScore.Instance.UpdateJarScores();
                }
                else
                {
                    TimeRemaining = 0;
                    SetGameState(GameState.Over);
                }
                break;

            case GameState.Over:
                if (!_isGameOver)
                {
                    _isGameOver = true;
                    StartCoroutine(ShowVictoryAndLoadScene());
                }
                break;
        }
    }

    public void SetGameState(GameState newState)
    {
        CurrentGameState = newState;
        Debug.Log($"Game state changed to: {CurrentGameState}");
        HandleGameStateChange(newState);
    }



    private void HandleGameStateChange(GameState newState)
    {
        switch (newState)
        {
            case GameState.Loading:
                if (!_isStartCoroutine)
                {
                    StartCoroutine(StartCountDown());
                    _isStartCoroutine = true;
                }
                break;

            case GameState.Go:
                break;

            case GameState.Over:
                break;
        }
    }

    private IEnumerator StartCountDown()
    {
        for (int i = _countDown; i >= 0; i--)
        {
            yield return new WaitForSeconds(1);
            Debug.Log($"CountDown: {i}");
        }
        SetGameState(GameState.Go);
    }

    public bool AreAllPlayersReady()
    {
        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList.ToArray();
        //Debug.Log("Player count: " + players.Length);
        int readyPlayerCount = 0;


        if (players.Length < 2)
        {
            return false; 
        }

        foreach (Photon.Realtime.Player player in players)
        {
            if (player.CustomProperties.TryGetValue("IsReady_RainGauge", out object isReadyObj))
            {
                if ((bool)isReadyObj)
                {
                    readyPlayerCount++;
                }
            }
        }

        return readyPlayerCount == players.Length;
    }

    private void UpdateGameTimer()
    {
        if (TimeRemaining > 0)
        {
            TimeRemaining -= Time.deltaTime;

        }
        else
        {
            TimeRemaining = 0;
            SetGameState(GameState.Over);
        }
    }

    private IEnumerator ShowVictoryAndLoadScene()
    {
        int winnerNumber = JarScore.Instance.DetermineWinner(); // 승자 결정 후 플레이어 번호 반환

        if (winnerNumber != -1)
        {
            PlayerPrefs.SetInt("WinnerPlayerNumber", winnerNumber); // 승자 정보를 저장
        }
        else
        {
            PlayerPrefs.DeleteKey("WinnerPlayerNumber"); // 승자가 없을 경우 저장된 정보를 삭제
        }

        while (_countEnd > 0)                     
        {
            Debug.Log($"CountDown: {_countEnd}");
            yield return new WaitForSeconds(1);
            _countEnd--;
        }
        
        PhotonNetwork.LoadLevel("MainScene");
    }
}