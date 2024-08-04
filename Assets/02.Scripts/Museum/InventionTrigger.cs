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
            QuickSlotManager.ActivateAfterQuickSlot(InventionType);
            GlobalInventionManager.Instance.SetInventionActive(InventionType, true);
            Destroy(this.gameObject);
        }
    }
}
