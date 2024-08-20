using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class ThrowAnim : MonoBehaviour
{
    public Animator animator;
    private bool isDragging = false;

    void Update()
    {
        HandleMouseInput();
    }

    void HandleMouseInput()
    {
        // Animator의 현재 상태가 "Idle"일 때만 드래그 가능
        if (IsInIdleState() && Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            animator.SetBool("IsDragging", true); // 드래그 시작
            Debug.Log("드래그 중");
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            animator.SetBool("IsDragging", false); // 드래그 종료, Throw 상태로 전환
            Debug.Log("드래그 끝");
        }
    }

    // 현재 Animator 상태가 "Idle"인지 확인하는 함수
    bool IsInIdleState()
    {
        // "Base Layer"의 0번째 레이어에서 현재 애니메이션 상태를 가져옵니다.
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
    }
}
