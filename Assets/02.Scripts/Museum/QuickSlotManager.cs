using TMPro;
using UnityEngine;

public class QuickSlotManager : MonoBehaviour
{
    public GameObject[] beforeQuickSlots;
    public GameObject[] afterQuickSlots;

    public TextMeshProUGUI InventionReleasedText;

    private void Start()
    {
        for (int i = 0; i < beforeQuickSlots.Length; i++)
        {
            beforeQuickSlots[i].SetActive(true);
            afterQuickSlots[i].SetActive(false);
        }

       
    }

    public void ActivateAfterQuickSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= beforeQuickSlots.Length)
        {
            Debug.LogError("Invalid slot index");
            return;
        }

        // beforeQuickSlots[slotIndex].SetActive(false); // beforeQuickSlots는 그대로 두고
        afterQuickSlots[slotIndex].SetActive(true); // afterQuickSlots만 활성화


       // 슬롯 인덱스에 따라 텍스트 변경
        string[] inventionMessages = new string[]
        {
            "혼천의가 해금되었습니다. 박물관에서 확인하세요!",
            "해시계가 해금되었습니다. 박물관에서 확인하세요!",
            "측우기가 해금되었습니다. 박물관에서 확인하세요!",
            "천문도가 해금되었습니다. 박물관에서 확인하세요!",
            "자격루가 해금되었습니다. 박물관에서 확인하세요!"
        };

        if (slotIndex < inventionMessages.Length)
        {
            InventionReleasedText.text = inventionMessages[slotIndex];
        }
        else
        {
            Debug.LogError("No message available for the given slot index");
        }
    }
}