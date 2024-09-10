using UnityEngine;

public class JangTrigger : MonoBehaviour
{
    public JangChatChoice JangChatChoice;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("발동");
            JangChatChoice.TriggerStart();
        }
    }
}
