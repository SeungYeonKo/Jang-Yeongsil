using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SunMiniGame : MonoBehaviour
{
    public ClockInteraction clockInteraction;
    public PlayerMoveAbility playerMoveAbility;
    public Slider sundialSlider;
    public TextMeshProUGUI questionText;
    public Image rightImage;
    public TextMeshProUGUI qzText;
    public Image SunImage;
    public Image ImagePP;

    public Camera mainCamera;  // 추가
    public Camera miniGameCamera;
    public Camera moonCamera;

    private float tolerance = 5f;
    public float answerHoldTime = 3.0f;
    private bool isPlayerAnswering = false;
    private float answerHoldTimer = 0.0f;

    public float SuccsessTimer = 10f;
    public float Timer = 0.0f;

    private Dictionary<string, float> questionAnswerPairs;
    private List<string> remainingQuestions;
    private float correctValue;

    public TextMeshProUGUI SuccsessText;
    public TextMeshProUGUI SceneChangeText;

    public int SuccsessCount;

    public bool isGameActive = false;
    private bool _isGameOver = false;

    public List<GameObject> moons;
    public SkyboxManager skyboxManager;

    public bool SceneChangeDelays = false;

    // 추가된 변수
    public Sprite[] sunSprites; // 슬라이더 값에 따른 스프라이트 배열
    private int currentSpriteIndex = 0; // 현재 선택된 스프라이트 인덱스

    void Start()
    {
        miniGameCamera.enabled = true;
        moonCamera.enabled = false;
        DisableUILayerOnMainCamera();
        questionAnswerPairs = new Dictionary<string, float>
        {
            {"오전 8시30분을 표시하세요", 85f},
            {"오전 9시를 표시하세요", 105f},
            {"오전 9시30분을 표시하세요", 125f},
            {"오전 10시를 표시하세요", 143f},
            {"오전 10시30분을 표시하세요", 165f},
            {"오전 11시를 표시하세요", 181f},
            {"오전 11시30분을 표시하세요", 201f},
            {"오후 12시를 표시하세요", 265f},
            {"오전 12시30분을 표시하세요", 300f},
            {"오후 1시를 표시하세요", 320f},
            {"오후 1시30분을 표시하세요", 338f},
            {"오후 2시를 표시하세요", 355f},
            {"오후 2시30분을 표시하세요", 370f},
            {"오후 3시를 표시하세요", 389f},
            {"오후 3시30분을 표시하세요", 405f},
            {"오후 4시를 표시하세요", 428f}
        };

        remainingQuestions = new List<string>(questionAnswerPairs.Keys);

        if (questionText != null)
            questionText.text = "";

        sundialSlider.minValue = 0;
        sundialSlider.maxValue = 500;

        rightImage.gameObject.SetActive(false);
        SuccsessText.gameObject.SetActive(false);
        SceneChangeText.gameObject.SetActive(false);
        qzText.gameObject.SetActive(false);

        // 슬라이더가 변경될 때마다 스프라이트를 업데이트하는 콜백 설정
        sundialSlider.onValueChanged.AddListener(UpdateSunImage);
    }

    void Update()
    {
        if (isGameActive)
        {
            CheckAnswer();
        }


        /*if (SuccsessCount == 3 && isGameActive)
        {
            
        }*/
    }

    public void StartMiniGame()
    {
        if (playerMoveAbility != null)
            playerMoveAbility.DisableMovement();

        if (remainingQuestions.Count > 0)
        {
            int randomIndex = Random.Range(0, remainingQuestions.Count);
            string selectedQuestion = remainingQuestions[randomIndex];
            correctValue = questionAnswerPairs[selectedQuestion];

            questionText.text = selectedQuestion;
            remainingQuestions.RemoveAt(randomIndex);
        }

        isGameActive = true;
        isPlayerAnswering = true;
        answerHoldTimer = 0.0f;

        ActivateSunImage(); // 미니게임이 시작되면 sunImage 활성화
    }

    private void CheckAnswer()
    {
        if (isPlayerAnswering)
        {
            if (Mathf.Abs(sundialSlider.value - correctValue) <= tolerance && !Input.GetMouseButton(0))
            {
                answerHoldTimer += Time.deltaTime;
                if (answerHoldTimer >= answerHoldTime)
                {
                    isPlayerAnswering = false;
                    OnCorrectAnswer();
                }
            }
            else
            {
                answerHoldTimer = 0.0f;
            }
        }
    }

    private void OnCorrectAnswer()
    {
        SuccsessCount++;

        StartCoroutine(HandleCorrectAnswer());
    }

    private IEnumerator HandleCorrectAnswer()
    {
        // 1. UI 2초간 활성화
        rightImage.gameObject.SetActive(true);
        SuccsessText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.0f);

        // 2. UI 비활성화
        rightImage.gameObject.SetActive(false);
        SuccsessText.gameObject.SetActive(false);

        // 3. 카메라 전환 및 UI 비활성화
        miniGameCamera.enabled = false;
        moonCamera.enabled = true;

        SunImage.gameObject.SetActive(false); // 카메라 전환 시 sunImage 비활성화
        sundialSlider.gameObject.SetActive(false);
        qzText.gameObject.SetActive(false);
        ImagePP.gameObject.SetActive(false);

        // 4. Moon 오브젝트 서서히 비활성화
        yield return StartCoroutine(FadeOutAndDisable(moons[SuccsessCount - 1]));

        // 5. 1초 후 카메라 전환
        yield return new WaitForSeconds(1.0f);
        moonCamera.enabled = false;
        miniGameCamera.enabled = true;

        // **슬라이더 값에 맞는 이미지 다시 설정**
        UpdateSunImage(sundialSlider.value);  // 슬라이더 값에 맞는 이미지를 다시 설정합니다.

        // 6. 미니게임 UI 활성화
        ActivateSunImage(); // 카메라가 돌아오면 SunImage 다시 활성화
        sundialSlider.gameObject.SetActive(true);
        qzText.gameObject.SetActive(true);
        ImagePP.gameObject.SetActive(true);

        // **7. 새로운 문제 제시**
        if (SuccsessCount < 3)
        {
            StartMiniGame();  // 정답이 3개 미만일 경우 새 문제를 시작합니다.
        }
        else if (SuccsessCount == 3&& isGameActive)
        {
            OnCoroutineCompleted(); // 코루틴이 완료된 후 콜백 호출
        }
    }
    private void OnCoroutineCompleted()
    {
        StartCoroutine(EndGameSequence());
        if (skyboxManager != null)
        {
            skyboxManager.StartSkyboxTransition();
        }
        isGameActive = false;
        // 코루틴이 완료된 후 실행할 코드
        Debug.Log("코루틴 완료 후 처리 로직 실행");
    }
    private IEnumerator FadeOutAndDisable(GameObject obj)
    {
        Renderer objRenderer = obj.GetComponent<Renderer>();
        if (objRenderer != null)
        {
            Material objMaterial = objRenderer.material;

            Color startColor = objMaterial.color;
            float fadeDuration = 2.0f;
            float fadeTimer = 0.0f;

            while (fadeTimer < fadeDuration)
            {
                fadeTimer += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, fadeTimer / fadeDuration);
                objMaterial.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

                yield return null;
            }

            obj.SetActive(false);
        }
    }

    // 슬라이더 값에 따라 SunImage의 Source Image를 업데이트하는 함수
    private void UpdateSunImage(float sliderValue)
    {
        // 슬라이더 값에 따른 스프라이트 인덱스를 계산
        int spriteIndex = GetSpriteIndexBySliderValue(sliderValue);

        // 계산된 인덱스가 유효한 범위인지 확인
        if (sunSprites != null && spriteIndex >= 0 && spriteIndex < sunSprites.Length)
        {
            if (sunSprites[spriteIndex] != null)
            {
                SunImage.sprite = sunSprites[spriteIndex];  // SunImage의 Source Image 업데이트
                currentSpriteIndex = spriteIndex;
            }
            else
            {
                Debug.LogError($"sunSprites[{spriteIndex}] is null! Check sprite assignment.");
            }
        }
        else
        {
            Debug.LogError($"Invalid sprite index: {spriteIndex}. Ensure it's within the range.");
        }
    }

    // 슬라이더 값에 따라 적절한 스프라이트 인덱스를 계산하는 함수
    private int GetSpriteIndexBySliderValue(float sliderValue)
    {
        int maxIndex = sunSprites.Length - 1;
        float valueRange = sundialSlider
.maxValue / maxIndex;

        // 슬라이더 값이 잘못된 경우를 방지
        if (maxIndex <= 0)
        {
            Debug.LogError("No sprites assigned in sunSprites array!");
            return 0;
        }

        return Mathf.Clamp(Mathf.FloorToInt(sliderValue / valueRange), 0, maxIndex);
    }

    private IEnumerator EndGameSequence()
    {
        // UI 비활성화
        if (SunImage != null) SunImage.gameObject.SetActive(false);
        if (rightImage != null) rightImage.gameObject.SetActive(false);
        if (SuccsessText != null) SuccsessText.gameObject.SetActive(false);
        if (sundialSlider != null) sundialSlider.gameObject.SetActive(false);
        if (ImagePP != null) ImagePP.gameObject.SetActive(false);

        // SceneChangeText 활성화 및 5초 대기
        SceneChangeText.gameObject.SetActive(true);

        GameEND();

        yield return new WaitForSeconds(5.0f);

        // MainCamera 활성화 후 UI 레이어 비활성화
        mainCamera.gameObject.SetActive(true);
        miniGameCamera.gameObject.SetActive(false);
        moonCamera.gameObject.SetActive(false);
        DisableUILayerOnMainCamera();  // MainCamera에서 UI 레이어 비활성화
        
        // 씬 전환 로직
        PhotonManager.Instance.LeaveAndLoadRoom("Main");
    }

    private void GameEND()
    {
        Debug.Log("GameEND 함수 호출됨"); // 함수 호출 확인 로그

        // 기존 내용
        isGameActive = false;
        _isGameOver = true;
        qzText.gameObject.SetActive(false);

        if (clockInteraction != null)
        {
            clockInteraction.ResetMiniGame();
        }

        if (playerMoveAbility != null)
        {
            playerMoveAbility.EnableMovement();
        }

        if (PhotonNetwork.IsMasterClient)
        {
            Hashtable playerProperties = new Hashtable { { "SunMiniGameOver", true } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
            Debug.Log($"{playerProperties} 저장");
        }
    }

    // sunImage를 활성화하는 함수
    public void ActivateSunImage()
    {
        if (SuccsessCount == 3)
        {
            return;
        }
        if (SunImage != null)
        {
            SunImage.gameObject.SetActive(true);
            UpdateSunImage(sundialSlider.value);  // 활성화될 때마다 이미지를 업데이트
        }
    }

    // sunImage를 비활성화하는 함수
    public void DeactivateSunImage()
    {
        if (SunImage != null)
        {
            SunImage.gameObject.SetActive(false);
        }
    }
    void DisableUILayerOnMainCamera()
    {
        int uiLayer = LayerMask.NameToLayer("UI");
        if (mainCamera != null)
        {
            mainCamera.cullingMask &= ~(1 << uiLayer);  // UI 레이어의 비트 마스크를 해제
        }
        int playerLayer = LayerMask.NameToLayer("Player");
        if (miniGameCamera != null)
        {
            miniGameCamera.cullingMask &= ~(1 << uiLayer);  // Player 레이어의 비트 마스크를 해제
        }
    }
    private void ActivateGameCameras()
    {
        // miniGameCamera와 moonCamera 활성화, mainCamera 비활성화
        if (mainCamera != null) mainCamera.gameObject.SetActive(false);
        if (miniGameCamera != null) miniGameCamera.gameObject.SetActive(true);
        if (moonCamera != null) moonCamera.gameObject.SetActive(true);
    }

    private void ActivateMainCamera()
    {
        // mainCamera 활성화, miniGameCamera와 moonCamera 비활성화
        if (mainCamera != null) mainCamera.gameObject.SetActive(true);
        if (miniGameCamera != null) miniGameCamera.gameObject.SetActive(false);
        if (moonCamera != null) moonCamera.gameObject.SetActive(false);
    }
}
