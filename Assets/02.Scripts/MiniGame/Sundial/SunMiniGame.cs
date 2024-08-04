using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SunMiniGame : MonoBehaviour
{
    public ClockInteraction clockInteraction;
    public Slider sundialSlider; // 슬라이더 오브젝트 참조
    public TextMeshProUGUI questionText; // 문제를 표시할 텍스트 UI

    public float correctValue = 320f; // 정답으로 간주되는 슬라이더의 벨류값
    public float tolerance = 1f; // 정답으로 인정되는 오차 범위
    public float answerHoldTime = 3.0f; // 정답으로 간주되기 위해 플레이어가 슬라이더를 멈추는 시간
    public Image rightImage;

    public bool isGameActive = false;
    private bool isPlayerAnswering = false; // 플레이어가 정답을 맞추기 위해 슬라이더를 조작하고 있는지
    private float answerHoldTimer = 0.0f;

    void Start()
    {
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
    }

    void Update()
    {
        if (isGameActive)
        {
            CheckAnswer();
        }
    }

    public void StartMiniGame()
    {
        if (questionText != null)
        {
            questionText.text = "오후 1시를 표시하세요"; // 문제 텍스트 설정
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
                // 슬라이더가 정답 범위 안에 있으면 타이머를 증가시킴
                answerHoldTimer += Time.deltaTime;

                // 플레이어가 정답 위치에서 3초 동안 멈춰 있으면 정답 처리
                if (answerHoldTimer >= answerHoldTime)
                {
                    isPlayerAnswering = false;
                    OnCorrectAnswer();
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
            Debug.Log("정답 맞춤");
        }

        // 정답 처리 후 미니게임 비활성화
        isGameActive = false;

        // ClockInteraction 스크립트에서 UI와 카메라를 초기 상태로 되돌리도록 호출
        if (clockInteraction != null)
        {
            clockInteraction.ResetMiniGame();
        }
    }
}
