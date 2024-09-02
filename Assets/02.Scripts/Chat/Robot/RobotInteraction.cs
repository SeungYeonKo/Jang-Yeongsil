using UnityEngine;

public class RobotInteraction : MonoBehaviour
{
    public RobotManager robotManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            robotManager.ToggleChatUI(); // UI 활성화
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            robotManager.ToggleChatUI(); // UI 비활성화
        }
    }
}
