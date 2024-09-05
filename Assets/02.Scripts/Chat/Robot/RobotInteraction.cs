using UnityEngine;

public class RobotInteraction : MonoBehaviour
{
    public RobotUI robotUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            robotUI.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            robotUI.gameObject.SetActive(false);
        }
    }
}
