using UnityEngine;

public class RobotInteraction : MonoBehaviour
{
    public GameObject robotUI;

    private void Start()
    {
        robotUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            robotUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            robotUI.SetActive(false);
        }
    }
}
