using UnityEngine;

public class InventionTrigger : MonoBehaviour
{
    public InventionType inventionType;
    private QuickSlotManager quickSlotManager;

    private void Start()
    {
        quickSlotManager = FindObjectOfType<QuickSlotManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어가 트리거 영역에 들어왔을 때 적절한 슬롯을 활성화
            int slotIndex = (int)inventionType;
            quickSlotManager.ActivateAfterQuickSlot(slotIndex);

            Destroy(this.gameObject);
        }
    }
}
