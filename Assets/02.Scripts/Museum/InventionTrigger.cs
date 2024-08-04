using UnityEngine;

public class InventionTrigger : MonoBehaviour
{
    public InventionType InventionType;
    private QuickSlotManager QuickSlotManager;

    private void Start()
    {
        QuickSlotManager = FindObjectOfType<QuickSlotManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 퀵슬롯에 발명품 활성화
            QuickSlotManager.ActivateAfterQuickSlot(InventionType);

            // 발명품 상태를 GlobalInventionManager에 저장
            GlobalInventionManager.SetInventionActive(InventionType.ToString(), true);

            Destroy(this.gameObject);
        }
    }
}
