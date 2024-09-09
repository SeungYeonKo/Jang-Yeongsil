using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // 버튼 사용을 위해 추가

public enum QuizType
{
    OXS,
    BlankS
}

public class QuizTrigger : MonoBehaviour
{
    public QuizType QuizType;
    public GameObject OXS;
    public GameObject BlankS;

    private Button[] oxButtons; // OX 퀴즈 버튼들

    void Start()
    {
        OXS.gameObject.SetActive(false);
        BlankS.gameObject.SetActive(false);

        // OX 버튼들을 초기화 (OX 퀴즈가 활성화될 때 사용하기 위해)
        oxButtons = OXS.GetComponentsInChildren<Button>();
        foreach (Button button in oxButtons)
        {
            button.onClick.AddListener(() => OnOXButtonClicked(button));
        }
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

            if (QuizType == QuizType.OXS)
            {
                Debug.Log("OX퀴즈형 트리거");
                OXS.gameObject.SetActive(true);
                BlankS.gameObject.SetActive(false);
            }
            else if (QuizType == QuizType.BlankS)
            {
                Debug.Log("빈칸선택형 트리거");
                OXS.gameObject.SetActive(false);
                BlankS.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OXS.gameObject.SetActive(false);
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

    private void OnOXButtonClicked(Button clickedButton)
    {
        foreach (Button button in oxButtons)
        {
            var color = button.image.color;
            if (button == clickedButton)
            {
                color.a = 1f; // 클릭된 버튼의 알파 값을 255로 설정
            }
            else
            {
                color.a = 0.5f; // 클릭되지 않은 버튼의 알파 값을 128로 설정 (시각적 피드백을 주기 위해)
            }
            button.image.color = color;
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
