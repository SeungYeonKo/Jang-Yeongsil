using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // 텍스트 프로 매쉬를 사용하기 위한 네임스페이스

public class TextPopup : MonoBehaviour
{
    public TextMeshProUGUI textPopup; // 텍스트 프로 매쉬 UI 참조
    public bool GameReady;
    private bool isPlayerInTrigger;

    void Start()
    {
        if (textPopup != null)
        {
            textPopup.gameObject.SetActive(false); // 시작 시 텍스트 비활성화
            GameReady = false;
            isPlayerInTrigger = false;
        }
    }

    private void Update()
    {
        if (GameReady && Input.GetKeyDown(KeyCode.Q))
        {
            if (textPopup != null)
            {
                textPopup.gameObject.SetActive(false); // 텍스트 비활성화
                GameReady = false; // 상태 리셋
                isPlayerInTrigger = false; // 트리거 상태 초기화
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPlayerInTrigger) // 플레이어 태그를 가진 객체와 충돌 시
        {
            if (textPopup != null)
            {
                textPopup.gameObject.SetActive(true); // 텍스트 활성화
                GameReady = true;
                isPlayerInTrigger = true; // 플레이어가 트리거 안에 있음
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // 플레이어 태그를 가진 객체가 콜라이더를 벗어날 때
        {
            if (textPopup != null)
            {
                textPopup.gameObject.SetActive(false); // 텍스트 비활성화
                GameReady = false;
                isPlayerInTrigger = false; // 플레이어가 트리거를 벗어남
            }
        }
    }
}
