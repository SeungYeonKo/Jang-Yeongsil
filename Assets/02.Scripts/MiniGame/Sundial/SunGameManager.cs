using UnityEngine;

public class SunGameManager : MonoBehaviour
{
    public Transform player;
    public Transform sundial;
    public float interactionDistance = 2f;
    public Camera gameCamera;
    public Vector3 cameraOffset;
    public IGameState CurrentState { get; private set; }
    public GameIdleState GameIdleState = new GameIdleState();
    public GamePlayingState GamePlayingState = new GamePlayingState();
    public GameEndedState GameEndedState = new GameEndedState();

    private bool isNearSundial;
    public bool IsNearSundial
    {
        get { return isNearSundial; }
        set { isNearSundial = value; }
    }

    void Start()
    {
        SwitchState(GameIdleState);
    }

    void Update()
    {
        CurrentState.UpdateState(this);
        CheckPlayerDistance();
    }

    public void SwitchState(IGameState newState)
    {
        if (CurrentState != null)
        {
            CurrentState.ExitState(this);
        }
        CurrentState = newState;
        CurrentState.EnterState(this);
    }

    private void CheckPlayerDistance()
    {
        IsNearSundial = Vector3.Distance(player.position, sundial.position) <= interactionDistance;
    }

    public void StartGame()
    {
        // 게임 시작 시 카메라 위치 변경
        gameCamera.transform.position = sundial.position + cameraOffset;
        gameCamera.transform.LookAt(sundial);
        // 기타 게임 시작 로직
    }

    public void EndGame()
    {
        // 게임 종료 시 처리할 로직
    }
}
