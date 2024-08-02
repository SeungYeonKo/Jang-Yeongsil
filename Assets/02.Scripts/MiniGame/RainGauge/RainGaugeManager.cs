using Photon.Pun;
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
        int index = playerNumber - 2; // 배열 인덱스는 0부터 시작하므로 1을 뺍니다.

        // 플레이어 번호에 맞는 UI를 활성화
        if (index >= 0 && index < playerUI.Length)
        {
            playerUI[index].SetActive(true);
        }
        else
        {
            Debug.LogError("Invalid player number or playerUI not set in RainGaugeManager.");
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

        UpdateAllPlayerUI();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        UpdateAllPlayerUI();
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
                JarScore.Instance.UpdateJarScores();
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

        foreach (Photon.Realtime.Player player in players)
        {
            if (player.CustomProperties.TryGetValue("IsReady_RainGauge", out object isReadyObj))
            {
                if ((bool)isReadyObj)
                {
                    readyPlayerCount++;
                }
                else
                {
                    //Debug.Log("플레이어가 준비되지 않았습니다: " + player.NickName);
                }
            }
            else
            {
                //Debug.Log("플레이어 준비 상태가 없습니다: " + player.NickName);
            }
        }
        if (readyPlayerCount >= 1)
        {
            //Debug.Log("플레이어 모두 레디");
            return true;
        }
        else
        {
            //Debug.Log("레디한 플레이어 수가 충분하지 않습니다.");
            return false;
        }
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
        JarScore.Instance.DetermineWinner();// 승자 결정

        while (_countEnd > 0)
        {
            Debug.Log($"CountDown: {_countEnd}");
            yield return new WaitForSeconds(1);
            _countEnd--;
        }

        PhotonNetwork.LoadLevel("MainScene");
    }
}
