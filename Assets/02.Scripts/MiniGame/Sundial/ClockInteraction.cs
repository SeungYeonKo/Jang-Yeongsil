using UnityEngine;
using UnityEngine.UI;

public class ClockInteraction : MonoBehaviour
{
    public Camera mainCamera; // 메인 카메라
    public Transform clockViewTransform; // 해시계를 바라보는 위치와 회전값
    public GameObject uiCanvas; // 전체 UI 캔버스
    public TextPopup textPopup; // 텍스트 팝업 스크립트
    public GameObject sundialSlider; // 슬라이더 오브젝트
    public GameObject sunImage; // 슬라이더 이미지 오브젝트
    public GameObject qzText; // 문제 텍스트
    public GameObject qKeyText; // Q 키 텍스트
    public GameObject additionalImage; // 추가 이미지 오브젝트
    public TPSCamera tpsCamera; // TPS 카메라 스크립트 참조

    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    private bool isNearClock = false;
    private bool isClockViewActive = false;

    void Start()
    {
        // 초기 설정: 슬라이더와 이미지, 문제 텍스트 비활성화
        if (sundialSlider != null) sundialSlider.SetActive(false);
        if (sunImage != null) sunImage.SetActive(false);
        if (qzText != null) qzText.SetActive(false);
        if (additionalImage != null) additionalImage.SetActive(false);
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
        // 카메라 위치와 회전 전환
        if (!isClockViewActive)
        {
            // 현재 카메라 위치와 회전 저장
            originalCameraPosition = mainCamera.transform.position;
            originalCameraRotation = mainCamera.transform.rotation;

            // 해시계를 바라보는 위치로 카메라 이동
            mainCamera.transform.position = clockViewTransform.position;
            mainCamera.transform.rotation = clockViewTransform.rotation;

            // TPS 카메라 스크립트 비활성화
            if (tpsCamera != null) tpsCamera.enabled = false;

            // 슬라이더와 이미지, 문제 텍스트 활성화
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
            // 원래 위치로 카메라 이동
            mainCamera.transform.position = originalCameraPosition;
            mainCamera.transform.rotation = originalCameraRotation;

            // TPS 카메라 스크립트 활성화
            if (tpsCamera != null) tpsCamera.enabled = true;

            // 슬라이더와 이미지, 문제 텍스트 비활성화
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
