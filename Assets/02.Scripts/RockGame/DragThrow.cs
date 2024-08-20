using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragThrow : MonoBehaviour
{
    public LineRenderer trajectoryLine;
    public float throwForceMultiplier = 0.1f;
    public int lineSegmentCount = 20;
    private Rigidbody selectedRigidbody;
    private Vector3 dragStartPos;
    private Vector3 dragEndPos;
    private Vector3 throwDirection;
    private bool isDragging = false;

    void Update()
    {
        HandleMouseInput();
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                selectedRigidbody = hit.collider.GetComponent<Rigidbody>();
                if (selectedRigidbody != null)
                {
                    dragStartPos = Input.mousePosition;
                    isDragging = true;
                }
            }
        }

        if (Input.GetMouseButton(0) && isDragging && selectedRigidbody != null)
        {
            dragEndPos = Input.mousePosition;
            Vector3 difference = dragEndPos - dragStartPos;
            
            // 드래그 방향을 반대로 설정
            throwDirection = new Vector3(-difference.x, 0, -difference.y); // Z축을 깊이로 사용

            // 궤적 업데이트
            UpdateTrajectory(throwDirection * throwForceMultiplier);
        }

        if (Input.GetMouseButtonUp(0) && isDragging && selectedRigidbody != null)
        {
            isDragging = false;
            selectedRigidbody.velocity = CalculateLaunchVelocity();
            trajectoryLine.positionCount = 0; // 궤적 초기화
            selectedRigidbody = null;
        }
    }

    void UpdateTrajectory(Vector3 launchVelocity)
    {
        Vector3[] trajectoryPoints = new Vector3[lineSegmentCount];
        Vector3 currentPosition = selectedRigidbody.position;
        Vector3 currentVelocity = launchVelocity;

        for (int i = 0; i < lineSegmentCount; i++)
        {
            trajectoryPoints[i] = currentPosition;
            currentPosition += currentVelocity * Time.fixedDeltaTime;
            currentVelocity += Physics.gravity * Time.fixedDeltaTime;
        }

        trajectoryLine.positionCount = lineSegmentCount;
        trajectoryLine.SetPositions(trajectoryPoints);
    }

    Vector3 CalculateLaunchVelocity()
    {
        Vector3 difference = dragEndPos - dragStartPos;

        // 드래그 방향을 반대로 설정
        Vector3 direction = new Vector3(-difference.x, 0, -difference.y); // Z축을 깊이로 사용
        return direction * throwForceMultiplier;
    }
}
