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

    private TPSCamera tpsCameraScript; // TPSCamera 스크립트 참조

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

        // TPSCamera 스크립트 참조 가져오기
        tpsCameraScript = mainCamera.GetComponent<TPSCamera>();
    }

    void Update()
    {
        if (isNearClock && Input.GetKeyDown(KeyCode.Q))
        {
            ToggleClockView();
        }
    }

    void ToggleClockView()
    {
        if (!isClockViewActive)
        {
            originalCameraPosition = mainCamera.transform.position;
            originalCameraRotation = mainCamera.transform.rotation;

            mainCamera.transform.position = clockViewTransform.position;
            mainCamera.transform.rotation = clockViewTransform.rotation;

            if (sundialSlider != null) sundialSlider.SetActive(true);
            if (sunImage != null) sunImage.SetActive(true);
            if (qzText != null) qzText.SetActive(true);
            if (qKeyText != null) qKeyText.SetActive(false);

            // 커서 활성화
            if (tpsCameraScript != null)
            {
                tpsCameraScript.UnlockCursor();
            }
        }
        else
        {
            mainCamera.transform.position = originalCameraPosition;
            mainCamera.transform.rotation = originalCameraRotation;

            if (sundialSlider != null) sundialSlider.SetActive(false);
            if (sunImage != null) sunImage.SetActive(false);
            if (qzText != null) qzText.SetActive(false);
            if (qKeyText != null) qKeyText.SetActive(true);

            // 커서 비활성화
            if (tpsCameraScript != null)
            {
                tpsCameraScript.LockCursor();
            }
        }

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
            if (qKeyText != null) qKeyText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearClock = false;
            if (qKeyText != null) qKeyText.SetActive(false);
        }
    }
}
