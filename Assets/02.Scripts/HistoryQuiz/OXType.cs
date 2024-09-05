using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OXType : MonoBehaviour
{
    public Button[] OButtons; 
    public Button[] XButtons; 

    public Image[] CorrectImages;
    public Image[] WrongImages;

    public TextMeshProUGUI TimerText;

    public Button CloseButton;
    public Button SubmitButton;
    public Button ResetButton;

    private int[] playerAnswers; // 플레이어의 선택 저장 (0: 선택 안함, 1: O버튼, 2: X버튼)
    private int[] correctAnswers = { 1, 2, 2, 1, 1, 2 };

    private void OnEnable()
    {
        StartCoroutine(ResetButtonsCoroutine());
    }

    void Start()
    {
        playerAnswers = new int[6]; // 6개의 문제 초기화

        // 정답,오답 이미지 비활성화
        foreach (Image img in CorrectImages)
        {
            img.gameObject.SetActive(false);
        }
        foreach (Image img in WrongImages)
        {
            img.gameObject.SetActive(false);
        }

        // 버튼 리스너
        CloseButton.onClick.AddListener(CloseButtonClick);
        SubmitButton.onClick.AddListener(SubmitButtonClick);
        ResetButton.onClick.AddListener(ResetButtonClick);

        // O, X 버튼 리스너
        for (int i = 0; i < OButtons.Length; i++)
        {
            int index = i; // 람다 캡처 문제 해결을 위해 인덱스를 로컬 변수로 저장
            OButtons[i].onClick.AddListener(() => OnAnswerSelected(index, 1)); // 1은 O버튼
            XButtons[i].onClick.AddListener(() => OnAnswerSelected(index, 2)); // 2는 X버튼
        }

        // 초기화
        ResetButtons();

        // 타이머 시작
        StartCoroutine(StartTimer(60));
    }

    // 정답 선택 처리
    private void OnAnswerSelected(int questionIndex, int answer)
    {
        playerAnswers[questionIndex] = answer;

        // 모든 버튼 비활성화하여 다시 선택하지 못하도록 함
        OButtons[questionIndex].interactable = false;
        XButtons[questionIndex].interactable = false;
    }

    // 제출 버튼
    public void SubmitButtonClick()
    {
        for (int i = 0; i < playerAnswers.Length; i++)
        {
            if (playerAnswers[i] == correctAnswers[i])
            {
                CorrectImages[i].gameObject.SetActive(true);
            }
            else
            {
                WrongImages[i].gameObject.SetActive(true);
            }
        }
    }

    public void CloseButtonClick()
    {
        this.gameObject.SetActive(false); // 퀴즈가 종료되면 다시 커서를 잠금 상태로 설정
        LockCursor();
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ResetButtonClick()
    {
        foreach (Image img in CorrectImages)
        {
            img.gameObject.SetActive(false);
        }

        foreach (Image img in WrongImages)
        {
            img.gameObject.SetActive(false);
        }

        // 모든 버튼 초기화
        ResetButtons();
    }

    private void ResetButtons()
    {
        for (int i = 0; i < playerAnswers.Length; i++)
        {
            playerAnswers[i] = 0; // 플레이어의 답변 초기화
            OButtons[i].interactable = true; // O 버튼 활성화
            XButtons[i].interactable = true; // X 버튼 활성화
        }
    }

    private IEnumerator ResetButtonsCoroutine()
    {
        yield return null;
        ResetButtons();
    }

    private IEnumerator StartTimer(int seconds)
    {
        int currentTime = seconds;
        while (currentTime >= 0)
        {
            TimerText.text = currentTime.ToString();
            yield return new WaitForSeconds(1);
            currentTime--;
        }

        // 타이머가 끝났을 때
        TimerText.text = "시간 종료!";
    }
}
