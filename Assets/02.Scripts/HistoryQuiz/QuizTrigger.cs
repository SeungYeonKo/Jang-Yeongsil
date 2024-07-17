using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuizType
{
    MultipleS,
    BlankS
}
public class QuizTrigger : MonoBehaviour
{
    public QuizType QuizType;
    public GameObject MultipleS;
    public GameObject BlankS;

    void Start()
    {
        MultipleS.gameObject.SetActive(false);
        BlankS.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 커서 잠금 해제
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (QuizType == QuizType.MultipleS)
            {
                Debug.Log("4지선다형 트리거");
                MultipleS.gameObject.SetActive(true);
                BlankS.gameObject.SetActive(false);
            }
            else if (QuizType == QuizType.BlankS)
            {
                Debug.Log("빈칸선택형 트리거");
                MultipleS.gameObject.SetActive(false);
                BlankS.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MultipleS.gameObject.SetActive(false);
            BlankS.gameObject.SetActive(false);

            // 커서 다시 잠금
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
