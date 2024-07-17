using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FillInBlankQuiz : MonoBehaviour
{
    public Button[] answerButtons; // 정답 버튼들
    public Transform[] answerSlots; // 빈 게임 오브젝트의 위치
    public GameObject[] correctImages; // 정답 이미지
    public GameObject[] wrongImages; // 오답 이미지
    private Button[] originalPositions; // 버튼의 원래 위치 저장

    void Start()
    {
        originalPositions = new Button[answerButtons.Length];
        for (int i = 0; i < answerButtons.Length; i++)
        {
            originalPositions[i] = answerButtons[i];
            int index = i;
            answerButtons[i].onClick.AddListener(() => MoveButtonToSlot(index));
        }
    }

    void MoveButtonToSlot(int index)
    {
        Button selectedButton = answerButtons[index];
        Transform targetSlot = FindEmptySlot();
        if (targetSlot != null)
        {
            // DoTween을 사용하여 버튼을 빈 슬롯 위치로 이동
            selectedButton.transform.DOMove(targetSlot.position, 0.5f).OnComplete(() =>
            {
                // 이동 완료 후에 필요한 추가 행동
                selectedButton.interactable = false; // 이동 후 버튼 클릭 비활성화
            });
        }
    }

    Transform FindEmptySlot()
    {
        foreach (Transform slot in answerSlots)
        {
            // 해당 슬롯에 이미 버튼이 있는지 확인
            if (slot.childCount == 0) // 자식 오브젝트가 없다면 비어있는 것
            {
                return slot;
            }
        }
        return null; // 모든 슬롯이 찼을 경우
    }

    public void ResetButtonPosition(int buttonIndex)
    {
        Button buttonToReset = originalPositions[buttonIndex];
        // 버튼을 원래 위치로 이동
        buttonToReset.transform.DOMove(buttonToReset.transform.parent.position, 0.5f).OnComplete(() =>
        {
            buttonToReset.interactable = true; // 원위치로 돌아간 후 다시 클릭 가능하게 설정
        });
    }

    public void SubmitAnswers()
    {
        for (int i = 0; i < answerSlots.Length; i++)
        {
            bool isCorrect = CheckAnswer(i); // 정답 검사
            if (isCorrect)
            {
                correctImages[i].SetActive(true);
                wrongImages[i].SetActive(false);
            }
            else
            {
                correctImages[i].SetActive(false);
                wrongImages[i].SetActive(true);
            }
        }
    }

    bool CheckAnswer(int slotIndex)
    {
        // 정답 검사 로직 구현, 임시로 true/false 반환
        return true; // 임시로 모든 답이 정답으로 처리
    }
}
