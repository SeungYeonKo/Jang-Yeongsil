using UnityEngine;

public interface IGameState
{
    void EnterState(SunGameManager gameManager);
    void UpdateState(SunGameManager gameManager);
    void ExitState(SunGameManager gameManager);
}

public class GameIdleState : IGameState
{
    public void EnterState(SunGameManager gameManager)
    {
        // Idle 상태에 들어갈 때 초기화 작업
    }

    public void UpdateState(SunGameManager gameManager)
    {
        // Idle 상태에서의 업데이트 로직
        if (gameManager.IsNearSundial && Input.GetKeyDown(KeyCode.Q))
        {
            gameManager.SwitchState(gameManager.GamePlayingState);
        }
    }

    public void ExitState(SunGameManager gameManager)
    {
        // Idle 상태를 나올 때 작업
    }
}

public class GamePlayingState : IGameState
{
    public void EnterState(SunGameManager gameManager)
    {
        // 게임이 시작될 때 초기화 작업
        gameManager.StartGame();
    }

    public void UpdateState(SunGameManager gameManager)
    {
        // 게임 중에 실행될 업데이트 로직
    }

    public void ExitState(SunGameManager gameManager)
    {
        // 게임이 끝날 때 작업
        gameManager.EndGame();
    }
}

public class GameEndedState : IGameState
{
    public void EnterState(SunGameManager gameManager)
    {
        // 게임이 종료될 때 초기화 작업
    }

    public void UpdateState(SunGameManager gameManager)
    {
        // 종료된 상태에서의 업데이트 로직
    }

    public void ExitState(SunGameManager gameManager)
    {
        // 종료 상태를 나올 때 작업
    }
}
