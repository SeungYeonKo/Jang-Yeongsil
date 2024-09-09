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
        playerAnswers = new int[6]; // 6개의 문제 초기화 (0: 선택 안됨, 1: O버튼 선택, 2: X버튼 선택)

        // 정답, 오답 이미지 비활성화
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

        // O, X 버튼 리스너 설정
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
        // 플레이어의 답변 저장
        playerAnswers[questionIndex] = answer;

        // 클릭된 버튼의 알파값을 255로 설정하고, 다른 버튼의 알파값은 100으로 설정
        if (answer == 1) // O버튼을 클릭했을 때
        {
            SetButtonAlpha(OButtons[questionIndex], 255); // O 버튼을 클릭했으므로 O 버튼의 알파값을 255로 설정
            SetButtonAlpha(XButtons[questionIndex], 100); // X 버튼의 알파값을 100으로 설정
        }
        else if (answer == 2) // X버튼을 클릭했을 때
        {
            SetButtonAlpha(XButtons[questionIndex], 255); // X 버튼을 클릭했으므로 X 버튼의 알파값을 255로 설정
            SetButtonAlpha(OButtons[questionIndex], 100); // O 버튼의 알파값을 100으로 설정
        }

        // 두 버튼 모두 비활성화하여 다시 선택하지 못하도록 설정
        OButtons[questionIndex].interactable = false;
        XButtons[questionIndex].interactable = false;

        // 사운드 이펙트 재생
        SoundManager.instance.PlaySfx(SoundManager.Sfx.Quiz_OX);
    }

    // 버튼의 알파값을 설정하는 메서드
    private void SetButtonAlpha(Button button, byte alpha)
    {
        Color color = button.image.color; // 버튼의 현재 색상 가져오기
        color.a = alpha / 255f; // 알파값 설정 (0~255 값을 0~1로 변환)
        button.image.color = color; // 버튼의 색상 적용
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

    // 선택되지 않은 버튼만 초기화, 이미 선택된 버튼은 알파값 유지
    private void ResetButtons()
    {
        for (int i = 0; i < playerAnswers.Length; i++)
        {
            // 선택되지 않은 버튼만 초기화
            if (playerAnswers[i] == 0)
            {
                OButtons[i].interactable = true;
                XButtons[i].interactable = true;
                SetButtonAlpha(OButtons[i], 100); // O 버튼의 알파값 초기화
                SetButtonAlpha(XButtons[i], 100); // X 버튼의 알파값 초기화
            }
            else if (playerAnswers[i] == 1) // 이전에 O버튼을 클릭한 경우
            {
                SetButtonAlpha(OButtons[i], 255); // O 버튼의 알파값 유지
                SetButtonAlpha(XButtons[i], 100); // X 버튼의 알파값을 100으로 설정
                OButtons[i].interactable = false; // 상호작용 불가
                XButtons[i].interactable = false; // 상호작용 불가
            }
            else if (playerAnswers[i] == 2) // 이전에 X버튼을 클릭한 경우
            {
                SetButtonAlpha(XButtons[i], 255); // X 버튼의 알파값 유지
                SetButtonAlpha(OButtons[i], 100); // O 버튼의 알파값을 100으로 설정
                OButtons[i].interactable = false; // 상호작용 불가
                XButtons[i].interactable = false; // 상호작용 불가
            }
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
