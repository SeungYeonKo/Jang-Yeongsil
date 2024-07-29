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
public class RainGaugeManager : MonoBehaviour
{
    public static RainGaugeManager Instance { get; private set; }

    private float _gameDuration = 30f; 
    public float TimeRemaining;

    private int _countDown = 5; // 시작 카운트다운
    private int _countEnd = 5; // 종료 후 대기
    private bool _isGameOver = false;
    private bool _isStartCoroutine = false;

    public GameState CurrentGameState = GameState.Ready;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        InitializeGame();
    }

    private void OnDisable()
    {
        InitializeGame();
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
        if (readyPlayerCount >= 2)
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
