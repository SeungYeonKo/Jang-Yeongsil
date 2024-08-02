using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용하기 위한 네임스페이스

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

    private bool isNearClock = false;
    private bool isClockViewActive = false;

    void Start()
    {
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
        Debug.Log("ToggleClockView() called"); // 디버깅 메시지 추가

        if (!isClockViewActive)
        {
            Debug.Log("Activating mini game camera and UI elements");

            if (mainCamera != null) mainCamera.gameObject.SetActive(false);
            if (miniGameCamera != null) miniGameCamera.gameObject.SetActive(true);

            if (sundialSlider != null) sundialSlider.gameObject.SetActive(true);
            if (sunImage != null) sunImage.gameObject.SetActive(true);
            if (qzText != null) qzText.gameObject.SetActive(true);
            if (additionalImage != null) additionalImage.gameObject.SetActive(true);

            if (qKeyText != null) qKeyText.gameObject.SetActive(false);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Debug.Log("Deactivating mini game camera and UI elements");

            if (mainCamera != null) mainCamera.gameObject.SetActive(true);
            if (miniGameCamera != null) miniGameCamera.gameObject.SetActive(false);

            if (sundialSlider != null) sundialSlider.gameObject.SetActive(false);
            if (sunImage != null) sunImage.gameObject.SetActive(false);
            if (qzText != null) qzText.gameObject.SetActive(false);
            if (additionalImage != null) additionalImage.gameObject.SetActive(false);

            if (qKeyText != null) qKeyText.gameObject.SetActive(true);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        isClockViewActive = !isClockViewActive;
        Debug.Log("isClockViewActive: " + isClockViewActive); // 상태 확인
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sundial"))
        {
            isNearClock = true;

            // Q 키 텍스트 활성화
            if (qKeyText != null) qKeyText.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Sundial"))
        {
            isNearClock = false;

            // Q 키 텍스트 비활성화
            if (qKeyText != null) qKeyText.gameObject.SetActive(false);
        }
    }
}
