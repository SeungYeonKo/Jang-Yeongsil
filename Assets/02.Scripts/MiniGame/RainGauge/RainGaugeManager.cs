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

    private float _gameDuration = 60f;
    public float TimeRemaining;

    private int _countDown = 5; // 시작 카운트다운
    private int _countEnd = 5; // 종료 후 대기
    private bool _isGameOver = false;
    private bool _isStartCoroutine = false;

    public GameState CurrentGameState = GameState.Ready;

    public GameObject[] playerUI; // 0번 인덱스부터 4개의 플레이어 UI를 배열로 관리
    private int playerNumber;
    private Dictionary<int, RainGaugePlayer> players = new Dictionary<int, RainGaugePlayer>();
    private UI_RainGaugeManager uiRainGaugeManager;

    private ParticleSystem rainParticleSystem;

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
        uiRainGaugeManager = FindObjectOfType<UI_RainGaugeManager>();
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
        if (uiRainGaugeManager != null)
        {
            InitializeGame();
        }

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
        uiRainGaugeManager.SetReadyImageState(false);
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        SoundManager.instance.StopBgm();
        SoundManager.instance.PlayBgm(SoundManager.Bgm.RainGauge);

        GameObject waterfallEffect = GameObject.Find("WaterfallSmallEffect");
        if (waterfallEffect != null)
        {
            rainParticleSystem = waterfallEffect.GetComponent<ParticleSystem>();
            if (rainParticleSystem != null)
            {
                rainParticleSystem.gameObject.SetActive(false); 
            }
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
                uiRainGaugeManager.SetReadyImageState(false);
                SetRainParticleSystemActive(false);
                break;

            case GameState.Go:
                uiRainGaugeManager.SetReadyImageState(false);
                SetRainParticleSystemActive(true);
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
                uiRainGaugeManager.SetReadyImageState(false);
                SetRainParticleSystemActive(false);
                if (!_isGameOver)
                {
                    _isGameOver = true;
                    StartCoroutine(ShowVictoryAndLoadScene());
                }
                break;
        }

        // Q 키가 눌렸을 때 자동으로 승자가 되도록 처리
        if (Input.GetKeyDown(KeyCode.Q) && CurrentGameState == GameState.Go)
        {
            int localPlayerNumber = (int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerNumber"];
            PlayerPrefs.SetInt("WinnerPlayerNumber", localPlayerNumber); // 승자 정보 저장
            SetGameState(GameState.Over); // 게임 종료
        }
    }

    private void SetRainParticleSystemActive(bool isActive)
    {
        if (rainParticleSystem != null && rainParticleSystem.gameObject.activeSelf != isActive)
        {
            rainParticleSystem.gameObject.SetActive(isActive);
            Debug.Log($"RainParticleSystem is now {(isActive ? "enabled" : "disabled")}");
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
        uiRainGaugeManager.SetReadyImageState(false);

        for (int i = _countDown; i >= 0; i--)
        {
            yield return new WaitForSeconds(1);
        }
        SetGameState(GameState.Go);
    }

    public bool AreAllPlayersReady()
    {
        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList.ToArray();
        int readyPlayerCount = 0;

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
            Hashtable winnerProperties = new Hashtable { { "WinnerPlayerNumber", winnerNumber } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(winnerProperties); // Photon의 Custom Properties로 승자 정보 저장
        }

        // RainMiniGameOver 설정
        Hashtable rainGameOverProps = new Hashtable { { "RainMiniGameOver", true } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(rainGameOverProps);

        while (_countEnd > 0)
        {
            yield return new WaitForSeconds(1);
            _countEnd--;
        }

        // 씬 이동 전에 Cheugugi 오브젝트를 활성화 시킬 수 있도록 해당 상태를 저장합니다.
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "CheugugiActivated", true } });

        PhotonManager.Instance.LeaveAndLoadRoom("Main");
    }
}
