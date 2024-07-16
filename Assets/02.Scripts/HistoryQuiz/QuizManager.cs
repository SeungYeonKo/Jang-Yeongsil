using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    public ToggleGroup[] toggleGroups; // 여섯 문제의 ToggleGroup 배열
    public Toggle[] answerToggles; // 정답인 토글들
    public Image[] correctImages; // 정답 시 활성화되는 이미지 배열
    public Image[] wrongImages; // 오답 시 활성화되는 이미지 배열

    // Start is called before the first frame update
    void Start()
    {
        // 모든 정답/오답 이미지를 비활성화
        foreach (Image img in correctImages)
        {
            img.gameObject.SetActive(false);
        }

        foreach (Image img in wrongImages)
        {
            img.gameObject.SetActive(false);
        }
    }

    // 제출 버튼을 눌렀을 때 실행되는 함수
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

    // 선택된 토글을 반환하는 함수
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
