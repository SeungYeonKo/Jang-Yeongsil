using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class BlankSelectType : MonoBehaviour
{
    public Button[] AnswerButtons;
    public Transform[] AnswerPositions;
    public Image[] CorrectImages;
    public Image[] WrongImages;

    public Button ResetButton;
    public Button SubmitButton;
    public Button CloseButton;

    private Vector3[] _initialPositions;
    private Transform[] _initialParents; // 초기 부모를 저장
    private Dictionary<Button, Transform> buttonPositionMap; // 버튼과 포지션 매핑

    void Start()
    {
        _initialPositions = new Vector3[AnswerButtons.Length];
        _initialParents = new Transform[AnswerButtons.Length];
        buttonPositionMap = new Dictionary<Button, Transform>();

        for (int i = 0; i < AnswerButtons.Length; i++)
        {
            int index = i; // 로컬 복사본 생성
            _initialPositions[index] = AnswerButtons[index].transform.position;
            _initialParents[index] = AnswerButtons[index].transform.parent; // 초기 부모 저장
            AnswerButtons[index].onClick.AddListener(() => OnAnswerButtonClick(AnswerButtons[index]));
        }

        // 이미지 비활성화
        foreach (Image img in CorrectImages)
        {
            img.gameObject.SetActive(false);
        }

        foreach (Image img in WrongImages)
        {
            img.gameObject.SetActive(false);
        }

        ResetButton.onClick.AddListener(ResetButtonClick);
        SubmitButton.onClick.AddListener(SubmitButtonClick);
        CloseButton.onClick.AddListener(CloseButtonClick);
    }

    // 정답 버튼
    public void OnAnswerButtonClick(Button button)
    {
        for (int i = 0; i < AnswerPositions.Length; i++)
        {
            if (AnswerPositions[i].childCount == 0) // 비어있는 포지션 찾기
            {
                button.transform.SetParent(AnswerPositions[i]);
                button.transform.DOLocalMove(Vector3.zero, 0.3f).SetEase(Ease.Linear);
                buttonPositionMap[button] = AnswerPositions[i];
                return; // 첫 번째 빈 포지션에 배치하면 루프를 종료
            }
        }
    }

    // 다시하기 버튼
    public void ResetButtonClick()
    {
        for (int i = 0; i < AnswerButtons.Length; i++)
        {
            AnswerButtons[i].transform.SetParent(_initialParents[i]); // 초기 부모로 되돌리기 : 다시 클릭 가능하게 하기 위함
            AnswerButtons[i].transform.DOMove(_initialPositions[i], 0.25f).SetEase(Ease.Linear); // DoTween 애니메이션 추가
        }
        buttonPositionMap.Clear();

        // 모든 CorrectImage와 WrongImage 비활성화
        foreach (Image img in CorrectImages)
        {
            img.gameObject.SetActive(false);
        }

        foreach (Image img in WrongImages)
        {
            img.gameObject.SetActive(false);
        }
    }

    // 제출하기 버튼
    public void SubmitButtonClick()
    {
        bool allCorrect = true;

        for (int i = 1; i < AnswerPositions.Length; i++) // i = 1부터 시작
        {
            if (AnswerPositions[i].childCount > 0)
            {
                Button placedButton = AnswerPositions[i].GetChild(0).GetComponent<Button>();
                int buttonIndex = System.Array.IndexOf(AnswerButtons, placedButton);

                if (buttonIndex != i - 1) // 정답이 아닐 경우 (i-1이 버튼의 올바른 위치)
                {
                    allCorrect = false;
                    WrongImages[i].gameObject.SetActive(true);
                    CorrectImages[i].gameObject.SetActive(false);
                }
                else // 정답일 경우
                {
                    WrongImages[i].gameObject.SetActive(false);
                    CorrectImages[i].gameObject.SetActive(true);
                }
            }
            else // 포지션에 버튼이 없는 경우
            {
                allCorrect = false;
                WrongImages[i].gameObject.SetActive(true);
                CorrectImages[i].gameObject.SetActive(false);
            }
        }

        if (allCorrect)
        {
            // 모든 정답이 맞으면 뭐할지는 나중에
        }
    }

    public void CloseButtonClick()
    {
        this.gameObject.SetActive(false);
    }
}
