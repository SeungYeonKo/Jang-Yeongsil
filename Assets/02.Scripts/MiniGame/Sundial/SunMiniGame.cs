using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun;
using Photon.Realtime;

public class SunMiniGame : MonoBehaviour
{
    public ClockInteraction clockInteraction;
    public PlayerMoveAbility playerMoveAbility;
    public Slider sundialSlider; // 슬라이더 오브젝트 참조
    public TextMeshProUGUI questionText; // 문제를 표시할 텍스트 UI

    private float tolerance = 2f; // 정답으로 인정되는 오차 범위
    public float answerHoldTime = 3.0f; // 정답으로 간주되기 위해 플레이어가 슬라이더를 멈추는 시간
    public Image rightImage;

    public bool isGameActive = false;
    private bool isPlayerAnswering = false; // 플레이어가 정답을 맞추기 위해 슬라이더를 조작하고 있는지
    private float answerHoldTimer = 0.0f;
    public bool PlayerWin = false;

    private float SuccsessTimer = 5f;
    public float Timer = 0.0f;

    private Dictionary<string, float> questionAnswerPairs; // 문제와 답을 저장할 딕셔너리
    private float correctValue; // 현재 문제에 대한 정답 값

    public TextMeshProUGUI SuccseccText;
    public TextMeshProUGUI SceneChangeText;

    void Start()
    {
        // 문제와 정답의 경우의 수를 설정합니다.
        questionAnswerPairs = new Dictionary<string, float>
        {
            {"오전 9시를 표시하세요", 105f},
            {"오후 1시를 표시하세요", 320f},
            {"오후 3시를 표시하세요", 389f},
            {"오전 11시를 표시하세요", 181f}
            // 필요한 만큼 추가하세요
        };

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

        SuccseccText.gameObject.SetActive(false);
        SceneChangeText.gameObject.SetActive(false);

        SoundManager.instance.StopBgm();
        SoundManager.instance.PlayBgm(SoundManager.Bgm.SundialScene);
    }

    void Update()
    {
        if (isGameActive)
        {
            CheckAnswer();
        }
        else if (PlayerWin && Timer >= 0) // 게임이 끝난 후 타이머가 동작하도록 수정
        {
            Timer += Time.deltaTime;
            SceneChangeText.gameObject.SetActive(true);
            if (Timer >= SuccsessTimer)
            {
                Timer = 0;
                PhotonManager.Instance.LeaveAndLoadRoom("Main");
                
                
            }
        }
    }

    public void StartMiniGame()
    {
        if (playerMoveAbility != null)
        {
            playerMoveAbility.DisableMovement();
        }

        // 랜덤하게 문제를 선택
        int randomIndex = Random.Range(0, questionAnswerPairs.Count);
        KeyValuePair<string, float> selectedQuestion = new KeyValuePair<string, float>();

        int currentIndex = 0;
        foreach (var pair in questionAnswerPairs)
        {
            if (currentIndex == randomIndex)
            {
                selectedQuestion = pair;
                break;
            }
            currentIndex++;
        }

        if (questionText != null)
        {
            questionText.text = selectedQuestion.Key; // 문제 텍스트 설정
        }

        correctValue = selectedQuestion.Value; // 정답 값을 설정

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
        if (questionText != null)
        {
            questionText.text = "정답입니다!";
            rightImage.gameObject.SetActive(true);
            PlayerWin = true;
            Debug.Log("정답 맞춤");
            SuccseccText.gameObject.SetActive(true);
        }

        // 정답 처리 후 미니게임 비활성화
        isGameActive = false;

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
