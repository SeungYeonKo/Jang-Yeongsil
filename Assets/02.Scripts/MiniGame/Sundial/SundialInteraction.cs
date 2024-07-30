using UnityEngine;

public class SundialInteraction : MonoBehaviour
{
    public SunGameManager gameManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.IsNearSundial = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.IsNearSundial = false;
        }
    }
}
