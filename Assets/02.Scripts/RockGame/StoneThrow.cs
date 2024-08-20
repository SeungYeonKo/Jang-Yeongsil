using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class StoneThrow : MonoBehaviour
{
    private Transform playerHand; // 손의 위치를 참조하는 변수
    private GameObject currentStone; // 현재 스톤 오브젝트
    private Rigidbody stoneRb;
    private Rigidbody playerRb; // 플레이어의 Rigidbody를 참조하는 변수
    private Vector3 dragStartPos;
    private Vector3 dragEndPos;
    private bool isDragging = false;
    private GameObject _player;
    public LineRenderer trajectoryLine; // 궤적을 그릴 Line Renderer
    public int lineSegmentCount = 20; // 궤적의 세그먼트 수
    public float throwForceMultiplier = 0.1f; // 던질 때 힘의 크기

    void Start()
    {
        // 플레이어 오브젝트와 Rigidbody를 찾습니다.
        _player = GameObject.FindWithTag("Player");
        if (_player != null)
        {
            playerRb = _player.GetComponent<Rigidbody>();
            trajectoryLine.positionCount = 0; // 라인 렌더러 초기화
        }
    }

    void Update()
    {
        HandleMouseInput();
    }

    void HandleMouseInput()
    {
        // 마우스를 클릭하여 스톤을 손으로 가져오기
        if (Input.GetMouseButtonDown(1) && currentStone == null)
        {
            FindAndPlaceStoneInHand();
        }

        if (Input.GetMouseButtonDown(0) && currentStone != null && !isDragging)
        {
            isDragging = true;
            dragStartPos = Input.mousePosition;
            trajectoryLine.positionCount = 0; // 궤적 초기화
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            dragEndPos = Input.mousePosition;
            Vector3 difference = dragEndPos - dragStartPos;
            Vector3 throwDirection = new Vector3(-difference.x, -Mathf.Abs(difference.y), -difference.y);

            // 궤적 시각화
            UpdateTrajectory(throwDirection * throwForceMultiplier);
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;

            // 돌을 손에서 분리하고 물리 엔진 활성화
            currentStone.transform.SetParent(null);
            stoneRb.isKinematic = false;

            // 드래그한 힘을 돌에 적용하여 던지기
            Vector3 difference = dragEndPos - dragStartPos;
            Vector3 throwDirection = new Vector3(-difference.x, -Mathf.Abs(difference.y), -difference.y); // 드래그 방향을 반대로 설정
            stoneRb.velocity = throwDirection * throwForceMultiplier;

            // 돌을 던진 후, 플레이어의 중력 다시 활성화
            if (playerRb != null)
            {
                playerRb.useGravity = true;
                playerRb.isKinematic = false;
            }

            currentStone = null;
            trajectoryLine.positionCount = 0;
        }
    }

    void FindAndPlaceStoneInHand()
    {
        // "Stone" 태그를 가진 오브젝트를 찾아서 현재 스톤으로 설정
        currentStone = GameObject.FindWithTag("Stone");
        playerHand = GameObject.Find("StonePosition").transform;
        
        if (currentStone != null)
        {
            // Rigidbody를 가져오고 돌의 위치를 손으로 옮김
            stoneRb = currentStone.GetComponent<Rigidbody>();
            currentStone.transform.position = playerHand.position;
            currentStone.transform.rotation = playerHand.rotation;

            // 손의 자식 오브젝트로 설정하여 손에 고정
            currentStone.transform.SetParent(playerHand);

            // 물리적 힘 제거
            stoneRb.velocity = Vector3.zero;
            stoneRb.angularVelocity = Vector3.zero;
            stoneRb.isKinematic = true; // 던지기 전까지 물리 효과를 비활성화

            // 돌을 손에 들고 있을 때 플레이어의 중력을 비활성화
            if (playerRb != null)
            {
                playerRb.useGravity = false;
                playerRb.isKinematic = true;
            }

            Debug.Log("스톤이 손으로 들어왔습니다.");
        }
        else
        {
            Debug.LogWarning("Stone 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }
    }
    void UpdateTrajectory(Vector3 launchVelocity)
    {
        Vector3[] trajectoryPoints = new Vector3[lineSegmentCount];
        Vector3 currentPosition = currentStone.transform.position;
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
}
