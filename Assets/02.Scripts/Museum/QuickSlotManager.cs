using UnityEngine;

public class QuickSlotManager : MonoBehaviour
{
    public GameObject[] beforeQuickSlots;
    public GameObject[] afterQuickSlots;

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
    }
}
