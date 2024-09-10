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
    public Slider sundialSlider; // 슬라이더 오브젝트 참조
    public TextMeshProUGUI questionText; // 문제를 표시할 텍스트 UI
    public Image rightImage; // 정답 시 표시할 이미지
    public TextMeshProUGUI qzText; // 문제 텍스트
    public Image SunImage; // Sun 이미지
    public Image ImagePP; // Post processing 이미지

    public Camera miniGameCamera; // 미니게임 카메라
    public Camera moonCamera; // Moon을 비출 카메라

    private float tolerance = 5f; // 정답으로 인정되는 오차 범위
    public float answerHoldTime = 3.0f; // 정답으로 간주되기 위해 플레이어가 슬라이더를 멈추는 시간
    private bool isPlayerAnswering = false; // 플레이어가 정답을 맞추기 위해 슬라이더를 조작하고 있는지
    private float answerHoldTimer = 0.0f;

    public float SuccsessTimer = 10f;
    public float Timer = 0.0f;

    private Dictionary<string, float> questionAnswerPairs; // 문제와 답을 저장할 딕셔너리
    private List<string> remainingQuestions; // 남은 문제를 추적하는 리스트
    private float correctValue; // 현재 문제에 대한 정답 값

    public TextMeshProUGUI SuccsessText;
    public TextMeshProUGUI SceneChangeText;

    public int SuccsessCount;

    public bool isGameActive = false; // 게임이 진행 중인지 여부를 나타내는 변수
    private bool _isGameOver = false;

    // Moon 오브젝트들을 관리하기 위한 리스트
    public List<GameObject> moons;
    // SkyboxManager 참조
    public SkyboxManager skyboxManager; 

    void Start()
    {
        // 미니게임 카메라를 활성화하고, Moon 카메라는 비활성화
        miniGameCamera.enabled = true;
        moonCamera.enabled = false;

        // 문제와 정답의 경우의 수를 설정합니다.
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
            {"오후 4시를 표시하세요", 428f},
        };

        remainingQuestions = new List<string>(questionAnswerPairs.Keys); // 모든 문제를 남은 문제 리스트에 추가

        if (questionText != null)
        {
            questionText.text = ""; // 초기에는 빈 텍스트로 설정
        }

        // 슬라이더의 최소값과 최대값을 설정합니다.
        if (sundialSlider != null)
        {
            sundialSlider.minValue = 0;
            sundialSlider.maxValue = 500;
        }

        rightImage.gameObject.SetActive(false);
        SuccsessText.gameObject.SetActive(false);
        SceneChangeText.gameObject.SetActive(false);
        qzText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isGameActive)
        {
            CheckAnswer();
        }
        else if (Timer >= 0 && SuccsessCount >= 3) // 게임이 끝난 후 타이머가 동작하도록 수정
        {
            Timer += Time.deltaTime;
            SceneChangeText.gameObject.SetActive(true);
            if (Timer >= SuccsessTimer)
            {
                Timer = 0;
                PhotonManager.Instance.LeaveAndLoadRoom("Main");
            }
        }

        if (SuccsessCount == 3 && isGameActive)
        {
            StartCoroutine(EndGameSequence());
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // 해시테이블을 초기화하거나 특정 키 값을 제거
                Hashtable emptyProperties = new Hashtable { { "SunMiniGameOver", null } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(emptyProperties);
                Debug.Log("SunMiniGameOver 리셋");
            }
        }
    }

    public void StartMiniGame()
    {
        if (playerMoveAbility != null)
        {
            playerMoveAbility.DisableMovement();
        }

        // 남아있는 문제 중에서 랜덤하게 선택
        if (remainingQuestions.Count > 0)
        {
            int randomIndex = Random.Range(0, remainingQuestions.Count);
            string selectedQuestion = remainingQuestions[randomIndex];
            correctValue = questionAnswerPairs[selectedQuestion];

            // 첫 문제일 경우 UI를 보여주지 않고 문제 텍스트만 설정
            if (SuccsessCount == 0)
            {
                questionText.text = selectedQuestion;
            }
            else
            {
                StartCoroutine(ShowCorrectAnswerUI(selectedQuestion)); // 문제 텍스트 설정을 코루틴으로 관리
            }

            remainingQuestions.RemoveAt(randomIndex); // 선택된 문제는 리스트에서 제거
        }

        isGameActive = true;
        isPlayerAnswering = true;
        answerHoldTimer = 0.0f;
    }

    private void CheckAnswer()
    {
        if (isPlayerAnswering)
        {
            // 플레이어가 슬라이더를 움직였는지 확인
            if (Mathf.Abs(sundialSlider.value - correctValue) <= tolerance)
            {
                if (!Input.GetMouseButton(0))
                {
                    answerHoldTimer += Time.deltaTime;

                    // 플레이어가 정답 위치에서 지정된 시간 동안 멈춰 있으면 정답 처리
                    if (answerHoldTimer >= answerHoldTime)
                    {
                        isPlayerAnswering = false;
                        OnCorrectAnswer();
                    }
                }
            }
            else
            {
                // 슬라이더가 정답 범위를 벗어나면 타이머를 리셋
                answerHoldTimer = 0.0f;
            }
        }
    }

    private void OnCorrectAnswer()
    {
        Debug.Log("정답 맞춤");
        SuccsessCount += 1;

        // 정답을 맞췄을 때 Moon 오브젝트를 서서히 비활성화하고 카메라 전환
        if (SuccsessCount <= moons.Count)
        {
            // MoonCamera로 전환하고 UI를 비활성화
            moonCamera.enabled = true;
            miniGameCamera.enabled = false;

            // 미니게임 UI 비활성화
            rightImage.gameObject.SetActive(false);
            qzText.gameObject.SetActive(false);
            sundialSlider.gameObject.SetActive(false);
            SunImage.gameObject.SetActive(false);
            ImagePP.gameObject.SetActive(false);

            StartCoroutine(FadeOutAndDisable(moons[SuccsessCount - 1])); // 코루틴 호출
        }

        if (SuccsessCount == 3)
        {
            // 문제 3개를 맞췄을 때 Skybox 전환 시작
            if (skyboxManager != null)
            {
                skyboxManager.StartSkyboxTransition();
            }
        }

        if (SuccsessCount < 3)
        {
            StartMiniGame(); // 정답을 맞췄고, 3번이 되지 않았다면 다음 문제 제시
        }
    }

    private IEnumerator ShowCorrectAnswerUI(string nextQuestion)
    {
        // 정답 맞췄을 때 UI를 2초 동안 활성화
        rightImage.gameObject.SetActive(true);
        SuccsessText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2.0f); // 2초 대기

        // UI 비활성화
        rightImage.gameObject.SetActive(false);
        SuccsessText.gameObject.SetActive(false);

        // 새로운 문제 텍스트 설정
        if (questionText != null)
        {
            questionText.text = nextQuestion;
        }
    }

    private IEnumerator EndGameSequence()
    {
        // 정답 맞췄을 때 UI를 2초 동안 활성화
        rightImage.gameObject.SetActive(true);
        SuccsessText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2.0f); // 2초 대기

        // UI 비활성화
        rightImage.gameObject.SetActive(false);
        SuccsessText.gameObject.SetActive(false);

        GameEND();
    }

    private void GameEND()
    {
        // 정답 처리 후 미니게임 비활성화
        isGameActive = false;
        _isGameOver = true;
        qzText.gameObject.SetActive(false);
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
        // ClockInteraction 스크립트에서 UI와 카메라를 초기 상태로 되돌리도록 호출
        if (clockInteraction != null)
        {
            clockInteraction.ResetMiniGame();
        }
    }

    // 오브젝트가 서서히 사라지고 나면 미니게임 카메라로 복귀하고 UI 활성화
    private IEnumerator FadeOutAndDisable(GameObject obj)
    {
        Renderer objRenderer = obj.GetComponent<Renderer>();
        if (objRenderer != null)
        {
            Material objMaterial = objRenderer.material;

            Color startColor = objMaterial.color; // 초기 색상
            float fadeDuration = 2.0f; // 페이드 아웃 지속 시간
            float fadeTimer = 0.0f;

            while (fadeTimer < fadeDuration)
            {
                fadeTimer += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, fadeTimer / fadeDuration); // 알파 값이 1에서 0으로 변화
                objMaterial.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

                yield return null; // 한 프레임 대기
            }

            obj.SetActive(false); // 페이드 아웃이 끝나면 오브젝트 비활성화

            // 오브젝트가 사라지고 1초 후에 다시 미니게임 카메라로 전환
            yield return new WaitForSeconds(1.0f);

            moonCamera.enabled = false;
            miniGameCamera.enabled = true;

            // 미니게임 UI 다시 활성화 (RightImage는 제외)
            SunImage.gameObject.SetActive(true);
            sundialSlider.gameObject.SetActive(true);
            ImagePP.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Renderer가 없습니다. 페이드 아웃을 적용할 수 없습니다.");
        }
    }
}
