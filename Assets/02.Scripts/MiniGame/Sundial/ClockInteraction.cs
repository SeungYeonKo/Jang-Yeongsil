using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필요

public class ClockInteraction : MonoBehaviour
{
    public Camera mainCamera; // 메인 카메라
    public Camera miniGameCamera; // 미니게임 전용 카메라
    public Canvas sundialSliderCanvas; // 전체 UI 캔버스 (SundialSlider_Canvas)
    public Slider sundialSlider; // 슬라이더 오브젝트
    public Image sunImage; // 슬라이더 이미지 오브젝트
    
    public TextMeshProUGUI qKeyText; // Q 키 텍스트
    public Image additionalImage; // 추가 이미지 오브젝트
    public SliderImageAnimator sliderImageAnimator; // SliderImageAnimator 스크립트 참조

    
    private bool isNearClock = false;
    private bool isClockViewActive = false;

    public SunMiniGame SunMiniGame;


    void Awake()
    {
        // 현재 씬이 "SundialScene"이 아닌 경우, 스크립트 비활성화
        if (SceneManager.GetActiveScene().name != "SundialScene")
        {
            this.enabled = false;
            return;
        }

        // SunMiniGame 스크립트와의 연결을 설정합니다.
        if (SunMiniGame != null)
        {
            SunMiniGame.clockInteraction = this;
            SunMiniGame.isGameActive = false;
        }
    }

    private void Start()
    {
        // 초기 설정: 미니게임 카메라 비활성화, 개별 UI 오브젝트 비활성화
        ResetMiniGame();
    }

    void Update()
    {
        // 해시계 근처에 있을 때 Q 키 입력을 받으면
        if (isNearClock && Input.GetKeyDown(KeyCode.Q))
        {
            SunMiniGame.StartMiniGame();
            SunMiniGame.qzText.gameObject.SetActive(true);
            ToggleClockView();
        }
    }

    public void ResetMiniGame()
    {
        // 미니게임 종료 시 초기 상태로 복원
        if (miniGameCamera != null) miniGameCamera.gameObject.SetActive(false);
        if (sundialSlider != null) sundialSlider.gameObject.SetActive(false);
        if (sunImage != null) sunImage.gameObject.SetActive(false);
        
        if (additionalImage != null) additionalImage.gameObject.SetActive(false);

        // 시작할 때 마우스 커서 숨김
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (mainCamera != null) mainCamera.gameObject.SetActive(true);
        if (qKeyText != null) qKeyText.gameObject.SetActive(false);

        isClockViewActive = false;
    }

    void ToggleClockView()
    {
        if (!isClockViewActive)
        {
            if (mainCamera != null) mainCamera.gameObject.SetActive(false);
            if (miniGameCamera != null) miniGameCamera.gameObject.SetActive(true);

            if (sliderImageAnimator != null)
            {
                sliderImageAnimator.UpdateImage();
            }

            if (sundialSlider != null)
            {
                sundialSlider.gameObject.SetActive(true);
            }
            if (sunImage != null)
            {
                sunImage.gameObject.SetActive(true);
                sliderImageAnimator.displayImage.sprite = sliderImageAnimator.sprites[0];
            }
            
            if (additionalImage != null)
            {
                additionalImage.gameObject.SetActive(true);
            }

            if (qKeyText != null) qKeyText.gameObject.SetActive(false);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        isClockViewActive = !isClockViewActive;
    }

    void OnTriggerEnter(Collider other)
    {
        // Sundial 태그를 가진 오브젝트와 충돌했을 때
        if (other.CompareTag("Player"))
        {
            isNearClock = true;

            // Q 키 텍스트 활성화
            if (qKeyText != null) qKeyText.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Sundial 태그를 가진 오브젝트가 범위를 벗어났을 때
        if (other.CompareTag("Player"))
        {
            isNearClock = false;

            // Q 키 텍스트 비활성화
            if (qKeyText != null) qKeyText.gameObject.SetActive(false);

            SunMiniGame.isGameActive = false;
            ResetMiniGame();
        }
    }
}
