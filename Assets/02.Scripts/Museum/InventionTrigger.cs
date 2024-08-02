using UnityEngine;

public class InventionTrigger : MonoBehaviour
{
    public string inventionName; // 발명품의 고유 이름

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 발명품 상태를 수집된 상태로 업데이트
            GlobalInventionManager.Instance.UpdateInventionState(inventionName, true);

            // 퀵슬롯 및 기타 처리
            QuickSlotManager.Instance.ActivateAfterQuickSlot((InventionType)System.Enum.Parse(typeof(InventionType), inventionName));

            // 발명품 오브젝트 제거
            Destroy(gameObject);
        }
    }
}
