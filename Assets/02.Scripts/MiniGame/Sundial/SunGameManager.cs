using UnityEngine;
using UnityEngine.UI;
using TMPro; // 텍스트 프로 매쉬를 사용하기 위한 네임스페이스

public class SunGameManager : MonoBehaviour
{
    public Transform player;
    public Transform sundial;
    public float interactionDistance = 2f;
    public Camera gameCamera;
    public Vector3 cameraOffset;
    public Slider rotationSlider; // 슬라이더 UI 참조
    public TextMeshProUGUI questionText; // 문제를 표시할 UI 텍스트
    public IGameState CurrentState { get; private set; }
    public GameIdleState GameIdleState = new GameIdleState();
    public GamePlayingState GamePlayingState = new GamePlayingState();
    public GameEndedState GameEndedState = new GameEndedState();

    private bool isNearSundial;
    private float correctRangeMin = 300f;
    private float correctRangeMax = 330f;
    private float timeInCorrectRange = 0f;
    private float requiredTimeInCorrectRange = 2f;

    private TPSCamera tpsCamera;

    public bool IsNearSundial
    {
        get { return isNearSundial; }
        set { isNearSundial = value; }
    }

    void Start()
    {
        SwitchState(GameIdleState);
        SetQuestion("1시를 표현하세요");
        tpsCamera = Camera.main.GetComponent<TPSCamera>();
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
        // TPSCamera 비활성화
        if (tpsCamera != null)
        {
            tpsCamera.enabled = false;
        }

        // 게임 시작 시 카메라 위치 변경
        gameCamera.transform.position = sundial.position + cameraOffset;
        gameCamera.transform.LookAt(sundial);

        // 슬라이더 값을 250으로 설정
        if (rotationSlider != null)
        {
            rotationSlider.value = 250f;
        }

        // 기타 게임 시작 로직
        SetQuestion("1시를 표현하세요");
    }

    public void EndGame()
    {
        // TPSCamera 활성화
        if (tpsCamera != null)
        {
            tpsCamera.enabled = true;
        }
        // 기타 게임 종료 로직
    }

    private void SetQuestion(string question)
    {
        if (questionText != null)
        {
            questionText.text = question;
        }
    }

    public void CheckSliderValue()
    {
        if (rotationSlider.value >= correctRangeMin && rotationSlider.value <= correctRangeMax)
        {
            timeInCorrectRange += Time.deltaTime;
            if (timeInCorrectRange >= requiredTimeInCorrectRange)
            {
                SwitchState(GameEndedState);
            }
        }
        else
        {
            timeInCorrectRange = 0f;
        }
    }
}
