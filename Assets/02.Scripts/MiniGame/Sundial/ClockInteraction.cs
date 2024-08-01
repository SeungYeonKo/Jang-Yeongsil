using UnityEngine;
using UnityEngine.UI;

public class ClockInteraction : MonoBehaviour
{
    public Camera mainCamera; // 메인 카메라
    public Camera miniGameCamera; // 미니게임 전용 카메라
    public GameObject sundialSliderCanvas; // 전체 UI 캔버스 (SundialSlider_Canvas)
    public TextPopup textPopup; // 텍스트 팝업 스크립트
    public GameObject sundialSlider; // 슬라이더 오브젝트
    public GameObject sunImage; // 슬라이더 이미지 오브젝트
    public GameObject qzText; // 문제 텍스트
    public GameObject qKeyText; // Q 키 텍스트
    public GameObject additionalImage; // 추가 이미지 오브젝트

    private bool isNearClock = false;
    private bool isClockViewActive = false;

    void Start()
    {
        // 초기 설정: 미니게임 카메라 비활성화, 개별 UI 오브젝트 비활성화
        if (miniGameCamera != null) miniGameCamera.gameObject.SetActive(false);
        if (sundialSlider != null) sundialSlider.SetActive(false);
        if (sunImage != null) sunImage.SetActive(false);
        if (qzText != null) qzText.SetActive(false);
        if (additionalImage != null) additionalImage.SetActive(false);

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
        if (!isClockViewActive)
        {
            // 메인 카메라 비활성화, 미니게임 카메라 활성화
            if (mainCamera != null) mainCamera.gameObject.SetActive(false);
            if (miniGameCamera != null) miniGameCamera.gameObject.SetActive(true);

            // SundialSlider_Canvas를 전체적으로 끄지 않고 개별 요소만 활성화
            if (sundialSlider != null) sundialSlider.SetActive(true);
            if (sunImage != null) sunImage.SetActive(true);
            if (qzText != null) qzText.SetActive(true);
            if (additionalImage != null) additionalImage.SetActive(true);

            // Q 키 텍스트 비활성화
            if (qKeyText != null) qKeyText.SetActive(false);

            // 마우스 커서 활성화
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // 메인 카메라 활성화, 미니게임 카메라 비활성화
            if (mainCamera != null) mainCamera.gameObject.SetActive(true);
            if (miniGameCamera != null) miniGameCamera.gameObject.SetActive(false);

            // SundialSlider_Canvas 내의 개별 UI 요소 비활성화
            if (sundialSlider != null) sundialSlider.SetActive(false);
            if (sunImage != null) sunImage.SetActive(false);
            if (qzText != null) qzText.SetActive(false);
            if (additionalImage != null) additionalImage.SetActive(false);

            // Q 키 텍스트 활성화
            if (qKeyText != null) qKeyText.SetActive(true);

            // 마우스 커서 비활성화
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // 텍스트 팝업 비활성화
        if (textPopup != null && isClockViewActive)
        {
            textPopup.HideText();
        }

        isClockViewActive = !isClockViewActive;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearClock = true;

            // Q 키 텍스트 활성화
            if (qKeyText != null) qKeyText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearClock = false;

            // Q 키 텍스트 비활성화
            if (qKeyText != null) qKeyText.SetActive(false);
        }
    }
}
