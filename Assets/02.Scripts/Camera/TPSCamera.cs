using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TPSCamera : MonoBehaviourPunCallbacks
{
    public float distance = 3f;
    public float height = 2f;
    public float smoothSpeed = 0.125f;
    public float sensitivity = 2.0f;

    // Private fields
    private float _rotationX = 0.0f;
    private float _rotationY = 0.0f;

    private Vector3 offset;

    public Transform target;

    // 추가된 변수
    private bool isQuizActive = false;

    public bool isMaze = false;
    public bool FPSview = false;

    StartTrigger startTrigger;

    // Public properties to access rotationX and rotationY
    public float RotationX
    {
        get { return _rotationX; }
        set { _rotationX = value; }
    }

    public float RotationY
    {
        get { return _rotationY; }
        set
        {
            if (!FPSview) // FPS 모드가 아닐 때만 제한을 둔다
            {
                _rotationY = Mathf.Clamp(value, -90f, 90f);
            }
            else
            {
                _rotationY = value;
            }
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "ClepsydraScene")
        {
            isMaze = true;
        }
        startTrigger = GetComponent<StartTrigger>();

        offset = new Vector3(0, height, -distance);
        FindLocalPlayer();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("카메라 스타트코드 실행");
    }

    public override void OnJoinedRoom()
    {
        FindLocalPlayer();
    }

    void Update()
    {
        if (target == null) return;

        if (isQuizActive)
        {
            Debug.Log("이거 트루임");
            return;
        }

        // 마우스 입력에 따른 카메라 회전
        _rotationX += Input.GetAxis("Mouse X") * sensitivity;
        _rotationY -= Input.GetAxis("Mouse Y") * sensitivity;

        // FPS 모드일 때는 제한을 두지 않기 때문에 Clamp를 사용하지 않습니다.

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockCursor();
        }

        if (Input.GetMouseButtonDown(0) && !Cursor.visible)
        {
            LockCursor();
        }

        // FPS와 TPS 전환을 위한 토글 기능 추가
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (FPSview)
            {
                FindLocalPlayer(); // TPS 모드로 전환
            }
            else
            {
                FindLocalMazePlayer(); // FPS 모드로 전환
            }
        }
    }

    private void FixedUpdate()
    {
        // 퀴즈가 활성화되면 카메라 이동 및 회전 중지
        if (isQuizActive)
        {
            return;
        }

        if (target == null) return;

        if (!FPSview)
        {
            Quaternion targetRotation = Quaternion.Euler(_rotationY, _rotationX, 0);
            Vector3 desiredPosition = target.position + targetRotation * offset;

            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.LookAt(target.position); // 카메라가 타겟을 바라보도록 설정
        }
        else if (FPSview)
        {
            // 1인칭 모드: 카메라를 타겟의 위치에 고정
            transform.position = target.position; // 타겟의 위치에 카메라 고정
            transform.rotation = Quaternion.Euler(_rotationY, _rotationX, 0); // 카메라 회전
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
                Transform cameraRoot = player.transform.Find("CameraRoot");
                if (cameraRoot != null)
                {
                    target = cameraRoot;
                    offset = new Vector3(0, height, -distance);
                    FPSview = false;
                    Debug.Log("TPS 모드로 전환됨: " + player.name);
                }
                else
                {
                    Debug.LogError("CameraRoot not found on player: " + player.name);
                }
                break;
            }
        }
    }

    private void FindLocalMazePlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            PhotonView photonView = player.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                Transform cameraRoot2 = player.transform.Find("CameraRoot2"); // CameraRoot2를 찾음
                if (cameraRoot2 != null)
                {
                    target = cameraRoot2; // 타겟을 CameraRoot2로 설정
                    offset = new Vector3(0, 1.65f, 0.1f); // FPS 모드에서의 카메라 위치
                    FPSview = true;
                    Debug.Log("FPS 모드로 전환됨: " + player.name);
                }
                else
                {
                    Debug.LogError("CameraRoot2 not found on player: " + player.name);
                }
                break;
            }
        }
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

    // 퀴즈 활성화/비활성화 상태 설정 메서드
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
