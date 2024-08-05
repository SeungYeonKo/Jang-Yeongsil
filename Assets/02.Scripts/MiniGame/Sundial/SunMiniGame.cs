using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class SunMiniGame : MonoBehaviour
{
    public ClockInteraction clockInteraction;
    public PlayerMoveAbility playerMoveAbility;
    public Slider sundialSlider; // 슬라이더 오브젝트 참조
    public TextMeshProUGUI questionText; // 문제를 표시할 텍스트 UI

    public float tolerance = 1f; // 정답으로 인정되는 오차 범위
    public float answerHoldTime = 3.0f; // 정답으로 간주되기 위해 플레이어가 슬라이더를 멈추는 시간
    public Image rightImage;
    public TextMeshProUGUI qzText; // 문제 텍스트
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

    void Start()
    {
        // 문제와 정답의 경우의 수를 설정합니다.
        questionAnswerPairs = new Dictionary<string, float>
        {
            {"오전 9시를 표시하세요", 105f},
            {"오후 1시를 표시하세요", 320f},
            {"오후 3시를 표시하세요", 389f},
            {"오전 11시를 표시하세요", 181f},
            {"오후 2시30분을 표시하세요", 370f},
            {"오전 8시30분을 표시하세요", 85f},
            {"오후 12시를 표시하세요", 265f},
            {"오전 10시30분을 표시하세요", 165f}
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
        qzText.gameObject.SetActive(false);
        if (playerMoveAbility != null)
        {
            playerMoveAbility.EnableMovement();
        }

        // ClockInteraction 스크립트에서 UI와 카메라를 초기 상태로 되돌리도록 호출
        if (clockInteraction != null)
        {
            clockInteraction.ResetMiniGame();
        }
    }
}
