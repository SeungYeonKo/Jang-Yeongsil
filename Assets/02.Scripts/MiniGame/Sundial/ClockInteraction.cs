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
    public TextMeshProUGUI qzText; // 문제 텍스트
    public TextMeshProUGUI qKeyText; // Q 키 텍스트
    public Image additionalImage; // 추가 이미지 오브젝트

    public SliderImageAnimator sliderImageAnimator; // SliderImageAnimator 스크립트 참조

    private bool isNearClock = false;
    private bool isClockViewActive = false;

    void Start()
    {
        // 현재 씬이 "SundialScene"이 아닌 경우, 스크립트 비활성화
        if (SceneManager.GetActiveScene().name != "SundialScene")
        {
            this.enabled = false;
            return;
        }

        // 필요한 오브젝트들을 동적으로 찾습니다.
        if (mainCamera == null)
            mainCamera = Camera.main; // 메인 카메라를 자동으로 할당
        if (miniGameCamera == null)
            miniGameCamera = GameObject.Find("MiniGameCamera").GetComponent<Camera>();
        if (sundialSliderCanvas == null)
            sundialSliderCanvas = GameObject.Find("SundialSlider_Canvas").GetComponent<Canvas>();
        if (sundialSlider == null)
            sundialSlider = GameObject.Find("SundialSlider").GetComponent<Slider>();
        if (sunImage == null)
            sunImage = GameObject.Find("SunImage").GetComponent<Image>();
        if (qzText == null)
            qzText = GameObject.Find("QzText").GetComponent<TextMeshProUGUI>();
        if (qKeyText == null)
            qKeyText = GameObject.Find("QKeyText").GetComponent<TextMeshProUGUI>();
        if (additionalImage == null)
            additionalImage = GameObject.Find("AdditionalImage").GetComponent<Image>();

        // 초기 설정: 미니게임 카메라 비활성화, 개별 UI 오브젝트 비활성화
        if (miniGameCamera != null) miniGameCamera.gameObject.SetActive(false);
        if (sundialSlider != null) sundialSlider.gameObject.SetActive(false);
        if (sunImage != null) sunImage.gameObject.SetActive(false);
        if (qzText != null) qzText.gameObject.SetActive(false);
        if (additionalImage != null) additionalImage.gameObject.SetActive(false);

        // 시작할 때 마우스 커서 숨김
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 해시계 근처에 있을 때 Q 키 입력을 받으면
        if (isNearClock && Input.GetKeyDown(KeyCode.Q))
        {
            ToggleClockView();
        }
    }

    void ToggleClockView()
    {
        Debug.Log("ToggleClockView called");

        if (!isClockViewActive)
        {
            Debug.Log("Activating mini game elements");

            if (mainCamera != null) mainCamera.gameObject.SetActive(false);
            if (miniGameCamera != null) miniGameCamera.gameObject.SetActive(true);

            if (sliderImageAnimator != null)
            {
                sliderImageAnimator.UpdateImage();
            }

            if (sundialSlider != null)
            {
                sundialSlider.gameObject.SetActive(true);
                Debug.Log("Sundial Slider activated");
            }
            if (sunImage != null)
            {
                sunImage.gameObject.SetActive(true);
                Debug.Log("Sun Image activated");
            }
            if (qzText != null)
            {
                qzText.gameObject.SetActive(true);
                Debug.Log("QZ Text activated");
            }
            if (additionalImage != null)
            {
                additionalImage.gameObject.SetActive(true);
                Debug.Log("Additional Image activated");
            }

            if (qKeyText != null) qKeyText.gameObject.SetActive(false);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Debug.Log("Deactivating mini game elements");

            if (mainCamera != null) mainCamera.gameObject.SetActive(true);
            if (miniGameCamera != null) miniGameCamera.gameObject.SetActive(false);

            if (sundialSlider != null)
            {
                sundialSlider.gameObject.SetActive(false);
                Debug.Log("Sundial Slider deactivated");
            }
            if (sunImage != null)
            {
                sunImage.gameObject.SetActive(false);
                Debug.Log("Sun Image deactivated");
            }
            if (qzText != null)
            {
                qzText.gameObject.SetActive(false);
                Debug.Log("QZ Text deactivated");
            }
            if (additionalImage != null)
            {
                additionalImage.gameObject.SetActive(false);
                Debug.Log("Additional Image deactivated");
            }

            if (qKeyText != null) qKeyText.gameObject.SetActive(true);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        isClockViewActive = !isClockViewActive;
        Debug.Log("isClockViewActive: " + isClockViewActive);
    }

    void OnTriggerEnter(Collider other)
    {
        // Sundial 태그를 가진 오브젝트와 충돌했을 때
        if (other.CompareTag("Sundial"))
        {
            isNearClock = true;

            // Q 키 텍스트 활성화
            if (qKeyText != null) qKeyText.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Sundial 태그를 가진 오브젝트가 범위를 벗어났을 때
        if (other.CompareTag("Sundial"))
        {
            isNearClock = false;

            // Q 키 텍스트 비활성화
            if (qKeyText != null) qKeyText.gameObject.SetActive(false);
        }
    }
}
