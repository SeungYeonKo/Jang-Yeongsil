using System.Collections;
using System.Collections.Generic;
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
        if (Input.GetMouseButtonDown(0) && IsInPrepareThrowPose())
        {
            isDragging = true;
            animator.SetBool("IsDragging", true); // 드래그 시작
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            animator.SetBool("IsDragging", false); // 드래그 종료, Throw 상태로 전환
        }
    }

    bool IsInPrepareThrowPose()
    {
        // Animator의 현재 상태가 PrepareThrow일 때만 드래그를 허용
        return animator.GetCurrentAnimatorStateInfo(0).IsName("PrepareThrow");
    }
}
