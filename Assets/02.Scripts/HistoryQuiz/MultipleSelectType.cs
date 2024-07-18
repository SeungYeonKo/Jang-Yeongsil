using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MultipleSelectType : MonoBehaviour
{
    public ToggleGroup[] ToggleGroups;
    public Toggle[] AnswerToggles;
    public Image[] CorrectImages;
    public Image[] WrongImages;

    public Button CloseButton;
    public Button SubmitButton;
    public Button ResetButton;

    private void OnEnable()
    {
        // 시작할 때 모든 토글 초기화
        StartCoroutine(ResetTogglesCoroutine());
    }

    void Start()
    {
        // 이미지 비활성화
        foreach (Image img in CorrectImages)
        {
            img.gameObject.SetActive(false);
        }

        foreach (Image img in WrongImages)
        {
            img.gameObject.SetActive(false);
        }

        CloseButton.onClick.AddListener(CloseButtonClick);
        SubmitButton.onClick.AddListener(SubmitButtonClick);
        ResetButton.onClick.AddListener(ResetButtonClick);

        // 시작할 때 모든 토글 초기화
        ResetToggles();
    }

    // 제출 버튼
    public void SubmitButtonClick()
    {
        for (int i = 0; i < ToggleGroups.Length; i++)
        {
            Toggle selectedToggle = GetSelectedToggle(ToggleGroups[i]);
            if (selectedToggle == AnswerToggles[i])
            {
                CorrectImages[i].gameObject.SetActive(true);
            }
            else
            {
                WrongImages[i].gameObject.SetActive(true);
            }
        }
    }

    // 선택된 토글을 반환
    private Toggle GetSelectedToggle(ToggleGroup group)
    {
        foreach (Toggle toggle in group.GetComponentsInChildren<Toggle>())
        {
            if (toggle.isOn)
            {
                return toggle;
            }
        }
        return null;
    }

    public void CloseButtonClick()
    {
        this.gameObject.SetActive(false);
    }

    public void ResetButtonClick()
    {
        // 이미지 비활성화
        foreach (Image img in CorrectImages)
        {
            img.gameObject.SetActive(false);
        }

        foreach (Image img in WrongImages)
        {
            img.gameObject.SetActive(false);
        }

        // 모든 토글 체크 해제
        ResetToggles();
    }

    private void ResetToggles()
    {
        foreach (ToggleGroup group in ToggleGroups)
        {
            group.allowSwitchOff = true; // 모든 토글을 해제할 수 있도록 설정

            foreach (Toggle toggle in group.GetComponentsInChildren<Toggle>())
            {
                toggle.isOn = false;
                // 상태 변경 이벤트 수동 트리거
                toggle.onValueChanged.Invoke(toggle.isOn);
            }

            group.allowSwitchOff = false; // 원래 상태로 되돌림
        }
    }

    private IEnumerator ResetTogglesCoroutine()
    {
        yield return null; // 프레임이 한 번 업데이트된 후 실행
        ResetToggles();
    }
}
