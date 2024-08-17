using Photon.Pun;
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
            UnlockCursor();

            // 씬에서 TPSCamera 스크립트 찾기
            TPSCamera cameraScript = FindObjectOfType<TPSCamera>();
            if (cameraScript != null)
            {
                Debug.Log("카메라 스크립트 불러와짐");

                cameraScript.SetQuizActive(true); // 퀴즈 활성화 시 카메라 회전 멈춤
            }
            else
            {
                Debug.LogWarning("TPSCamera 스크립트를 찾을 수 없습니다.");
            }

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

            // 씬에서 TPSCamera 스크립트 찾기
            TPSCamera cameraScript = FindObjectOfType<TPSCamera>();
            if (cameraScript != null)
            {
                cameraScript.SetQuizActive(false); // 퀴즈 비활성화 시 카메라 회전 재개
                Debug.Log("퀴즈 종료 - 카메라 회전 재개");
            }
            else
            {
                Debug.LogWarning("TPSCamera 스크립트를 찾을 수 없습니다.");
            }

            LockCursor(); // 커서를 다시 잠금
        }
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
