using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuickSlotManager : MonoBehaviour
{
    public GameObject[] BeforeQuickSlots;
    public GameObject[] AfterQuickSlots;

    public TextMeshProUGUI InventionReleasedText;

    // InventionType에 따라 메시지와 슬롯 인덱스를 매핑하기 위한 딕셔너리
    private Dictionary<InventionType, int> inventionSlotMap = new Dictionary<InventionType, int>()
    {
        { InventionType.ArmillarySphere, 0 },
        { InventionType.Sundial, 1 },
        { InventionType.Cheugugi, 2 },
        { InventionType.AstronomicalChart, 3 },
        { InventionType.Clepsydra, 4 }
    };

    private Dictionary<InventionType, string> inventionMessages = new Dictionary<InventionType, string>()
    {
        { InventionType.ArmillarySphere, "혼천의가 해금되었습니다. 박물관에서 확인하세요!" },
        { InventionType.Sundial, "해시계가 해금되었습니다. 박물관에서 확인하세요!" },
        { InventionType.Cheugugi, "측우기가 해금되었습니다. 박물관에서 확인하세요!" },
        { InventionType.AstronomicalChart, "천문도가 해금되었습니다. 박물관에서 확인하세요!" },
        { InventionType.Clepsydra, "자격루가 해금되었습니다. 박물관에서 확인하세요!" }
    };

    private void Start()
    {
        for (int i = 0; i < BeforeQuickSlots.Length; i++)
        {
            BeforeQuickSlots[i].SetActive(true);
            AfterQuickSlots[i].SetActive(false);
        }
    }

    public void ActivateAfterQuickSlot(InventionType inventionType)
    {
        if (inventionSlotMap.TryGetValue(inventionType, out int slotIndex))
        {
            // 슬롯 인덱스가 유효한 경우에만 활성화
            if (slotIndex < BeforeQuickSlots.Length)
            {
                AfterQuickSlots[slotIndex].SetActive(true);
            }

            // 텍스트 메시지 변경 및 사라지게 하는 코루틴 시작
            if (inventionMessages.TryGetValue(inventionType, out string message))
            {
                InventionReleasedText.text = message;
                StartCoroutine(HideInventionTextAfterDelay(2f)); // 2초 후 텍스트 숨김
            }
            else
            {
                Debug.LogError("No message available for the given invention type");
            }
        }
        else
        {
            Debug.LogError("Invalid invention type");
        }
    }

    private IEnumerator HideInventionTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        InventionReleasedText.text = ""; // 텍스트 비우기
    }
}
