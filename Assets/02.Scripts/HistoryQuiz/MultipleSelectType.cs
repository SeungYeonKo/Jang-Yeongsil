using UnityEngine;
using UnityEngine.UI;

public class MultipleSelectType : MonoBehaviour
{
    public ToggleGroup[] ToggleGroups; 
    public Toggle[] AnswerToggles; 
    public Image[] CorrectImages;
    public Image[] WrongImages;

    private void OnEnable()
    {
        // 시작할 때마다 모든 토글 초기화(는잘안됨 왜지?)
        foreach (ToggleGroup group in ToggleGroups)
        {
            foreach (Toggle toggle in group.GetComponentsInChildren<Toggle>())
            {
                // 토글을 강제로 초기화해보겠음->안됨,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,
                toggle.isOn = true;
                toggle.isOn = false;
            }
        }
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
    }

    // 제출 버튼
    public void Submit()
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
}
