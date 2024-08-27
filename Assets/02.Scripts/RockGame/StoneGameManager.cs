using System;
using System.Collections;
using System.Collections.Generic;
using _02.Scripts.RockGame;
using UnityEngine;

public enum StoneGameState
{
    Start,
    Go,
    GameOver
}

public class StoneGameManager : MonoBehaviour
{
    public StoneGameState CurrentState;
    private StoneTimeAttack _stoneTimeAttack;
    void Start()
    {
        _stoneTimeAttack = FindObjectOfType<StoneTimeAttack>();
    }

    void Update()
    {
        switch (CurrentState)
        {
            case StoneGameState.Start:
                // 60초 이하일 때만 Go 상태로 전환
                if (_stoneTimeAttack.TimesUP <= 60)
                {
                    SetCurrentState(StoneGameState.Go);
                }
                break;
            case StoneGameState.Go:
                // 60초 이하이고 0초 이상일 때만 GameOver 상태로 전환
                if (_stoneTimeAttack.TimesUP <= 0)
                {
                    SetCurrentState(StoneGameState.GameOver);
                }
                break;
            case StoneGameState.GameOver:
                // 게임 오버 상태에 대한 추가 로직
                break;
        }
    }

    public void SetCurrentState(StoneGameState newState)
    {
        if (CurrentState != newState) // 상태가 다를 때만 변경
        {
            CurrentState = newState;
            // 상태 전환 시 필요한 추가 로직이 있다면 여기서 처리
            Debug.Log($"State changed to: {newState}");
        }
    }
}
