using System.Collections;
using TMPro;
using UnityEngine;

public class TalkingNPC : MonoBehaviour
{
    public string[] dialogue;
    private int currentDialogueIndex = 0;
    public TextMeshProUGUI Talk;
    private Coroutine dialogueCoroutine;
    private bool playerInRange = false; // 플레이어가 범위 내에 있는지 확인하는 플래그

    private void Start()
    {
        Talk.gameObject.SetActive(false);
    }

    void Update()
    {
        // E 키를 눌렀고, 플레이어가 범위 내에 있으며, 대화가 시작되지 않았을 때만 대화를 시작
        if (Input.GetKeyDown(KeyCode.E) && playerInRange && dialogueCoroutine == null)
        {
            dialogueCoroutine = StartCoroutine(StartDialogue());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Talk.gameObject.SetActive(true);
            playerInRange = true; // 플레이어가 범위에 들어왔음을 표시
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Talk.gameObject.SetActive(false);
            playerInRange = false; // 플레이어가 범위를 벗어났음을 표시

            // 대화 초기화
            if (dialogueCoroutine != null)
            {
                StopCoroutine(dialogueCoroutine);
                dialogueCoroutine = null;
            }
            currentDialogueIndex = 0;
        }
    }

    IEnumerator StartDialogue()
    {
        while (currentDialogueIndex < dialogue.Length)
        {
            Talk.text = dialogue[currentDialogueIndex];
            currentDialogueIndex++;
            yield return new WaitForSeconds(3f); // 3초 대기
        }

        // 대화가 끝나면 텍스트를 지우고 코루틴을 초기화
        Talk.text = string.Empty;
        dialogueCoroutine = null;
    }
}
