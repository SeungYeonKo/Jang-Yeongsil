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
    private StoneHitScore _stoneHitScore;
    private UI_StoneManager _uiStoneManager;
    public LineRenderer trajectoryLine; // 궤적을 그릴 Line Renderer
    public int lineSegmentCount = 20; // 궤적의 세그먼트 수
    public float throwForceMultiplier = 0.1f; // 던질 때 힘의 크기

    void Start()
    {
        _uiStoneManager = FindObjectOfType<UI_StoneManager>();
        _stoneHitScore = FindObjectOfType<StoneHitScore>();
        Transform _playerSpwan = GameObject.Find("PlayerPosition").transform;
        // 플레이어 오브젝트와 Rigidbody를 찾습니다.
        _player = GameObject.FindWithTag("Player");
        if (_player != null)
        {
            playerRb = _player.GetComponent<Rigidbody>();
            trajectoryLine.positionCount = 0; // 라인 렌더러 초기화
            _player.transform.position = _playerSpwan.position;
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
            _stoneHitScore.IsThrown = true;
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
            _uiStoneManager.StoneOut.SetActive(true);
            _uiStoneManager.StoneGet.SetActive(false);
        }
    }

    void FindAndPlaceStoneInHand()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 마우스 포인트에서 레이를 쏘는 경우
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 30f)) // 30 유닛 범위 내에서 충돌 확인
        {
            // 충돌한 오브젝트가 "Stone" 태그를 가지고 있고 리지드바디가 있는지 확인
            if (hit.collider.CompareTag("Stone") && hit.collider.GetComponent<Rigidbody>() != null)
            {
                // 현재 스톤으로 설정
                currentStone = hit.collider.gameObject;
                playerHand = GameObject.Find("StonePosition").transform;

                // 리지드바디를 가져오고 돌의 위치를 손으로 옮김
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
                // 비석에 플레이어 정보를 전달
                currentStone.GetComponent<StoneHitScore>().OnPickedUpByPlayer(_player.transform);
                _uiStoneManager.StoneGet.SetActive(true);
                _uiStoneManager.StoneOut.SetActive(false);
                Debug.Log("스톤이 손으로 들어왔습니다.");
            }
            else
            {
                Debug.LogWarning("레이가 충돌한 오브젝트가 'Stone' 태그를 가지고 있지 않거나 리지드바디가 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("레이가 충돌한 오브젝트가 없습니다.");
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
