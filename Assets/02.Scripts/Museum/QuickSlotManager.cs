using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuickSlotManager : MonoBehaviour
{
    public GameObject[] BeforeQuickSlots;
    public GameObject[] AfterQuickSlots;

    public TextMeshProUGUI InventionReleasedText;

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
        RestoreQuickSlotState();
    }

    private void RestoreQuickSlotState()
    {
        foreach (var entry in inventionSlotMap)
        {
            InventionType inventionType = entry.Key;
            int slotIndex = entry.Value;

            if (GlobalInventionManager.Instance.GetQuickSlotState(inventionType))
            {
                AfterQuickSlots[slotIndex].SetActive(true);
                BeforeQuickSlots[slotIndex].SetActive(false);
            }
            else
            {
                AfterQuickSlots[slotIndex].SetActive(false);
                BeforeQuickSlots[slotIndex].SetActive(true);
            }
        }
    }

    public void ActivateAfterQuickSlot(InventionType inventionType)
    {
        if (inventionSlotMap.TryGetValue(inventionType, out int slotIndex))
        {
            if (slotIndex < BeforeQuickSlots.Length)
            {
                AfterQuickSlots[slotIndex].SetActive(true);
                BeforeQuickSlots[slotIndex].SetActive(false);
                GlobalInventionManager.Instance.SaveQuickSlotState(inventionType, true);
            }

            if (inventionMessages.TryGetValue(inventionType, out string message))
            {
                InventionReleasedText.text = message;
                StartCoroutine(HideInventionTextAfterDelay(2f));
            }
        }
    }

    private IEnumerator HideInventionTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        InventionReleasedText.text = "";
    }
}
