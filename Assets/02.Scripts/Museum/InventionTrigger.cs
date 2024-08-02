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
            // 발명품 오브젝트 활성화
            QuickSlotManager.ActivateAfterQuickSlot(InventionType);

            // 해시테이블에 상태 저장
            GlobalInventionManager.InventionState[InventionType.ToString()] = true;

            // 이 오브젝트 제거
            Destroy(this.gameObject);
        }
    }
}
