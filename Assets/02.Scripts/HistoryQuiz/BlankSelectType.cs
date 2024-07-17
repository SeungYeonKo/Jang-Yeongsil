using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BlankSelectType : MonoBehaviour
{
    public Button[] AnswerButtons;
    public Transform[] AnswerSlots;
    public Image[] CorrectImages;
    public Image[] WrongImages;
    private Transform[] currentPositions; // 현재 버튼의 위치를 추적
    private bool[] slotOccupied; // 슬롯의 점유 상태를 추적

    void Start()
    {
        currentPositions = new Transform[AnswerButtons.Length];
        slotOccupied = new bool[AnswerSlots.Length];

        for (int i = 0; i < AnswerButtons.Length; i++)
        {
            int index = i;
            AnswerButtons[i].onClick.AddListener(() => MoveButtonToSlot(index));
        }

        foreach (Image img in CorrectImages)
        {
            img.gameObject.SetActive(false);
        }

        foreach (Image img in WrongImages)
        {
            img.gameObject.SetActive(false);
        }
    }

    void MoveButtonToSlot(int index)
    {
        Button selectedButton = AnswerButtons[index];
        Transform targetSlot = FindEmptySlot(selectedButton.transform);
        if (targetSlot != null)
        {
            selectedButton.transform.DOMove(targetSlot.position, 0.5f).OnComplete(() =>
            {
                selectedButton.interactable = false; // 이동 후 버튼 클릭 비활성화
                UpdateSlotOccupancy(currentPositions[index], targetSlot, index);  // 슬롯 점유 상태 업데이트
            });
        }
    }

    Transform FindEmptySlot(Transform currentButtonPosition)
    {
        // 현재 버튼이 있는 슬롯의 인덱스를 찾아 그 다음 인덱스부터 검색 시작
        int startIndex = currentButtonPosition ? System.Array.IndexOf(AnswerSlots, currentButtonPosition) + 1 : 0;

        // 시작 인덱스부터 슬롯 배열의 끝까지 검색
        for (int i = startIndex; i < AnswerSlots.Length; i++)
        {
            if (!slotOccupied[i])
            {
                return AnswerSlots[i];
            }
        }

        // 처음부터 시작 인덱스 바로 전까지 검색 (환형 검색)
        for (int i = 0; i < startIndex; i++)
        {
            if (!slotOccupied[i])
            {
                return AnswerSlots[i];
            }
        }

        return null; // 모든 슬롯이 찼을 경우
    }

    void UpdateSlotOccupancy(Transform oldSlot, Transform newSlot, int buttonIndex)
    {
        // 이전 슬롯의 점유 상태 해제
        if (oldSlot)
        {
            int oldIndex = System.Array.IndexOf(AnswerSlots, oldSlot);
            slotOccupied[oldIndex] = false;
        }

        // 새 슬롯의 점유 상태 설정
        int newIndex = System.Array.IndexOf(AnswerSlots, newSlot);
        slotOccupied[newIndex] = true;
        currentPositions[buttonIndex] = newSlot; // 버튼의 현재 위치 업데이트
    }

    public void ResetButtonPosition(int buttonIndex)
    {
        Button buttonToReset = AnswerButtons[buttonIndex];
        Transform originalPosition = currentPositions[buttonIndex] ?? buttonToReset.transform.parent;
        buttonToReset.transform.DOMove(originalPosition.position, 0.5f).OnComplete(() =>
        {
            buttonToReset.interactable = true; // 원위치로 돌아간 후 다시 클릭 가능하게 설정
            currentPositions[buttonIndex] = null; // 위치 초기화
        });
    }

    public void SubmitAnswers()
    {
        for (int i = 0; i < AnswerSlots.Length; i++)
        {
            bool isCorrect = Submit(i);
            if (isCorrect)
            {
                CorrectImages[i].gameObject.SetActive(true);
                WrongImages[i].gameObject.SetActive(false);
            }
            else
            {
                CorrectImages[i].gameObject.SetActive(false);
                WrongImages[i].gameObject.SetActive(true);
            }
        }
    }

    bool Submit(int slotIndex)
    {
        // 정답 검사 로직
        return true; // 임시로 모든 답을 정답 처리해둠
    }


}