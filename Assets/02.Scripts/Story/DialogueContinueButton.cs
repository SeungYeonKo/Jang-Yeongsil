using UnityEngine;
using Yarn.Unity;

public class DialogueContinueButton : MonoBehaviour
{
    public DialogueRunner dialogueRunner;

    public void ContinueDialogue()
    {
        if (dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.Dialogue.SetNode("Start");
            dialogueRunner.Dialogue.Continue();
        }
    }
}
