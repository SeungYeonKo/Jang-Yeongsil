using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private void Start()
    {
        Instance = this;
        TimeRemaining = _gameDuration;
    }

    private void Update()
    {
        switch (CurrentGameState)
        {
            case GameState.Ready:
                //if (AreAllPlayersReady())
                {
                    SetGameState(GameState.Loading);
                }
                break;

            case GameState.Loading:
                break;

            case GameState.Go:
               // UpdateGameTimer();
                break;

            case GameState.Over:
                if (!_isGameOver)
                {
                    _isGameOver = true;
                    //StartCoroutine(ShowVictoryAndLoadScene());
                }
                break;
        }
    }

    public void SetGameState(GameState newState)
    {
        CurrentGameState = newState;
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
        for (int i = 0; i < _countDown + 1; i++)
        {
            yield return new WaitForSeconds(1);
            Debug.Log($"CountDown: {i}");
        }
        SetGameState(GameState.Go);
    }


}
