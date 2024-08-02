using UnityEngine;

public class InventionObject : MonoBehaviour
{
    public string InventionName; // 발명품의 고유 이름

    private void Start()
    {
        // 발명품이 이미 수집되었는지 확인
        if (GlobalInventionManager.Instance.GetInventionState(InventionName))
        {
            Destroy(gameObject); // 이미 수집된 발명품이면 오브젝트 제거
        }
    }
}
