using UnityEngine;
using UnityEngine.UI;

public class MultipleSelectType : MonoBehaviour
{
    public ToggleGroup[] toggleGroups; 
    public Toggle[] answerToggles; 
    public Image[] correctImages;
    public Image[] wrongImages;

    private void OnEnable()
    {
        // 시작할 때마다 모든 토글 초기화(는잘안됨 왜지?)
        foreach (ToggleGroup group in toggleGroups)
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
        foreach (Image img in correctImages)
        {
            img.gameObject.SetActive(false);
        }

        foreach (Image img in wrongImages)
        {
            img.gameObject.SetActive(false);
        }
    }

    // 제출 버튼
    public void Submit()
    {
        for (int i = 0; i < toggleGroups.Length; i++)
        {
            Toggle selectedToggle = GetSelectedToggle(toggleGroups[i]);
            if (selectedToggle == answerToggles[i])
            {
                correctImages[i].gameObject.SetActive(true);
            }
            else
            {
                wrongImages[i].gameObject.SetActive(true);
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
