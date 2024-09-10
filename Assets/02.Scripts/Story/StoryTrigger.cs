using UnityEngine;
using Yarn.Unity;
public class StoryTrigger : MonoBehaviour
{
    public DialogueRunner dialogueRunner; 
    public string startNode = "Start"; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (dialogueRunner != null && !dialogueRunner.IsDialogueRunning)
            {
                dialogueRunner.StartDialogue(startNode);
            }
        }
    }

    public void StartStory()
    {
        if (dialogueRunner != null && !dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.StartDialogue(startNode);
        }
    }
}
