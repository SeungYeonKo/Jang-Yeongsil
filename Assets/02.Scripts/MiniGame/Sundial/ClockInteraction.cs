using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ClockInteraction : MonoBehaviour
{
    public Camera mainCamera;
    public Camera miniGameCamera;
    public Canvas sundialSliderCanvas;
    public Slider sundialSlider;
    

    public TextMeshProUGUI qKeyText;
    public Image additionalImage;
    public SliderImageAnimator sliderImageAnimator;

    private bool isNearClock = false;
    private bool isClockViewActive = false;

    public SunMiniGame SunMiniGame;

    void Awake()
    {
        // SunMiniGame 스크립트와의 연결을 설정
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

        

       
    }

    public void ResetMiniGame()
    {
        if (miniGameCamera != null) miniGameCamera.gameObject.SetActive(false);
        if (sundialSlider != null) sundialSlider.gameObject.SetActive(false);
        /*if (sunImage != null) sunImage.gameObject.SetActive(false);*/
        if (additionalImage != null) additionalImage.gameObject.SetActive(false);

        if (mainCamera != null) mainCamera.gameObject.SetActive(true);
        if (qKeyText != null) qKeyText.gameObject.SetActive(false);

        isClockViewActive = false;

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
            /*if (sunImage != null)
            {
                sunImage.gameObject.SetActive(true);
                sliderImageAnimator.displayImage.sprite = sliderImageAnimator.sprites[0];
            }*/
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
