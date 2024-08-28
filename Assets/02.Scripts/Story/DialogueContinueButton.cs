using UnityEngine;
using Yarn.Unity;

public class DialogueContinueButton : MonoBehaviour
{
    public DialogueRunner dialogueRunner;

    public void ContinueDialogue()
    {
        if (dialogueRunner != null && dialogueRunner.IsDialogueRunning)
        {
            // 선택지가 있을 경우 자동으로 다음 선택창으로 넘어가는 것을 방지
            if (dialogueRunner.CurrentNodeName != null)
            {
                dialogueRunner.Dialogue.Continue();
            }
        }
    }
}
