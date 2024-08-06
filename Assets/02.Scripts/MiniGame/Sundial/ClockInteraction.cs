using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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
        // SunMiniGame 스크립트와의 연결을 설정합니다.
        if (SunMiniGame != null)
        {
            SunMiniGame.clockInteraction = this;
            SunMiniGame.isGameActive = false;
        }
    }

    private void Start()
    {
        ResetMiniGame();
    }

    void Update()
    {
        if (isNearClock && Input.GetKeyDown(KeyCode.Q))
        {
            SunMiniGame.StartMiniGame();
            SunMiniGame.qzText.gameObject.SetActive(true);
            ToggleClockView();
        }

        if (SunMiniGame != null)
        {
            if (SunMiniGame.isGameActive)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    public void ResetMiniGame()
    {
        if (miniGameCamera != null) miniGameCamera.gameObject.SetActive(false);
        if (sundialSlider != null) sundialSlider.gameObject.SetActive(false);
        if (sunImage != null) sunImage.gameObject.SetActive(false);
        if (additionalImage != null) additionalImage.gameObject.SetActive(false);

        if (mainCamera != null) mainCamera.gameObject.SetActive(true);
        if (qKeyText != null) qKeyText.gameObject.SetActive(false);

        isClockViewActive = false;

        // 미니게임이 종료되면 기본 TPSCamera의 커서 설정으로 돌아감
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        if (other.CompareTag("Player"))
        {
            isNearClock = true;
            if (qKeyText != null) qKeyText.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearClock = false;
            if (qKeyText != null) qKeyText.gameObject.SetActive(false);

            SunMiniGame.isGameActive = false;
            ResetMiniGame();
        }
    }
}
