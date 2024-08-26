using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TPSCamera : MonoBehaviourPunCallbacks
{
    public float distance = 3f; // 3인칭 카메라 거리
    public float height = 2f;   // 3인칭 카메라 높이
    public float smoothSpeed = 0.125f; // 3인칭 카메라의 부드러운 이동 속도
    public float sensitivity = 2.0f; // 마우스 감도

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    public Transform thirdPersonTarget; // 3인칭 카메라의 기준점 (CameraRoot)
    public Transform firstPersonTarget; // 1인칭 카메라의 기준점 (CameraRoot2)

    private Transform currentTarget; // 현재 시점에 따라 사용할 기준점
    private Vector3 offset; // 3인칭 카메라 오프셋

    private string _sceneName;

    // 추가된 변수
    private bool isQuizActive = false;
    private bool isFirstPerson = false; // 시점을 확인하는 변수
    public StartTrigger startTrigger; // StartTrigger 스크립트를 참조하기 위한 변수

    private void Awake()
    {
        _sceneName = SceneManager.GetActiveScene().name;
        if (_sceneName == "ClepsydraScene")
        {
            // ClepsydraScene에서 StartTrigger 컴포넌트를 찾음
            startTrigger = FindObjectOfType<StartTrigger>();
        }
    }

    private void Start()
    {
        offset = new Vector3(0, height, -distance); // 3인칭 카메라 오프셋 설정

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        FindLocalPlayer();
    }

    public override void OnJoinedRoom()
    {
        FindLocalPlayer();
    }

    void Update()
    {
        // ClepsydraScene에서만 카메라 시점 전환 로직 실행
        if (_sceneName == "ClepsydraScene" && startTrigger != null)
        {
            // isMazeStart가 true일 때 1인칭, 아닐 때 3인칭으로 전환
            SetFirstPersonView(startTrigger.isMazeStart);
        }

        // 퀴즈가 활성화되면 카메라 이동 중지
        if (isQuizActive)
        {
            return;
        }

        if (currentTarget == null) return;

        rotationX += Input.GetAxis("Mouse X") * sensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * sensitivity;
        rotationY = Mathf.Clamp(rotationY, -90f, 90f);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockCursor();
        }

        if (Input.GetMouseButtonDown(0) && !Cursor.visible)
        {
            LockCursor();
        }

        // 카메라의 회전을 직접 적용
        transform.localEulerAngles = new Vector3(rotationY, rotationX, 0);
    }

    private void FixedUpdate()
    {
        // 퀴즈가 활성화되면 카메라 이동 및 회전 중지
        if (isQuizActive || currentTarget == null)
        {
            return;
        }

        if (isFirstPerson)
        {
            // 1인칭 모드에서는 카메라를 기준점에 고정
            transform.position = currentTarget.position;
        }
        else
        {
            // 3인칭 모드에서는 카메라를 일정 거리 뒤에 배치
            Quaternion targetRotation = Quaternion.Euler(rotationY, rotationX, 0);
            Vector3 targetPosition = currentTarget.position + targetRotation * offset;

            // 카메라의 위치를 부드럽게 이동
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
            transform.LookAt(currentTarget.position);
        }
    }

    private void FindLocalPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            PhotonView photonView = player.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                Transform cameraRoot = player.transform.Find("CameraRoot"); // 3인칭 카메라용 기준점
                Transform cameraRoot2 = player.transform.Find("CameraRoot2"); // 1인칭 카메라용 기준점

                if (cameraRoot != null && cameraRoot2 != null)
                {
                    thirdPersonTarget = cameraRoot;
                    firstPersonTarget = cameraRoot2;
                    SetFirstPersonView(isFirstPerson); // 초기 설정에 따라 카메라 기준점 설정
                }
                else
                {
                    Debug.LogError("CameraRoot or CameraRoot2 not found on player: " + player.name);
                }
                break;
            }
        }
    }

    public void SetFirstPersonView(bool isFirstPersonView)
    {
        isFirstPerson = isFirstPersonView;
        currentTarget = isFirstPerson ? firstPersonTarget : thirdPersonTarget; // 현재 시점에 따라 기준점 설정
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // 퀴즈 활성화 상태 설정 메서드
    public void SetQuizActive(bool isActive)
    {
        isQuizActive = isActive;
        Debug.Log("Quiz Active State: " + isQuizActive);

        if (isActive)
        {
            UnlockCursor(); // 퀴즈가 활성화되면 커서를 보이게 함
        }
        else
        {
            LockCursor(); // 퀴즈가 비활성화되면 커서를 숨김
        }
    }
}
