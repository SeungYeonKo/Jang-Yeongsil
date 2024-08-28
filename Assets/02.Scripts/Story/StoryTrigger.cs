using UnityEngine;
using Yarn.Unity;
public class StoryTrigger : MonoBehaviour
{
    public DialogueRunner dialogueRunner;  // DialogueRunner를 참조합니다.
    public string startNode = "Start";     // 시작할 노드의 이름을 지정합니다.

    // 플레이어가 트리거에 진입할 때 호출됩니다.
    private void OnTriggerEnter(Collider other)
    {
        // 들어온 객체가 "Player" 태그를 가진 경우에만 실행
        if (other.CompareTag("Player"))
        {
            // 대화가 시작되지 않았을 때만 실행
            if (dialogueRunner != null && !dialogueRunner.IsDialogueRunning)
            {
                // 대화를 시작합니다.
                dialogueRunner.StartDialogue(startNode);
            }
        }
    }
}
