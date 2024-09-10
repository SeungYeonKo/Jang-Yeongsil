using UnityEngine;

public class ResetManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GlobalInventionManager.Instance.ResetInventionState();
            FindObjectOfType<QuickSlotManager>().ResetQuickSlots();

            // 오브젝트 활성화 코드 추가 가능
            ResetAllInventionObjects();
        }
    }

    private void ResetAllInventionObjects()
    {
        GameObject[] inventionObjects = {
            FindObjectOfType<InventionSpawnCheck>().Sundial,
            FindObjectOfType<InventionSpawnCheck>().Cheugugi,
            FindObjectOfType<InventionSpawnCheck>().AstronomicalChart,
            FindObjectOfType<InventionSpawnCheck>().ArmillarySphere,
            FindObjectOfType<InventionSpawnCheck>().Clepsydra
        };

        foreach (GameObject inventionObject in inventionObjects)
        {
            if (inventionObject != null)
            {
                inventionObject.SetActive(true); // 오브젝트 활성화
            }
        }
    }
}
