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

    private bool isQuizActive = false;
    public bool FPSview = false;

    StartTrigger startTrigger;
    private string _sceneName;
    public bool isMaze = false;

    // 이전 상태를 저장하는 변수
    private bool previousMazeStartState = false;
    private bool previousFPSviewState = false;

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
            if (!FPSview)
            {
                _rotationY = Mathf.Clamp(value, -90f, 90f);
            }
            else
            {
                _rotationY = value;
            }
        }
    }

    private void Awake()
    {
        _sceneName = SceneManager.GetActiveScene().name;
        isMaze = _sceneName == "ClepsydraScene";

        if (isMaze)
        {
            GameObject startTriggerObject = GameObject.Find("StartTrigger");
            if (startTriggerObject != null)
            {
                startTrigger = startTriggerObject.GetComponent<StartTrigger>();
                if (startTrigger == null)
                {
                    Debug.LogError("StartTrigger 컴포넌트를 StartTrigger 오브젝트에서 찾을 수 없습니다.");
                }
            }
            else
            {
                Debug.LogError("StartTrigger 오브젝트를 찾을 수 없습니다.");
            }
        }
    }

    private void Start()
    {
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
            return;
        }

        // 마우스 입력에 따른 카메라 회전
        _rotationX += Input.GetAxis("Mouse X") * sensitivity; // 좌우 회전 (Yaw)
        _rotationY -= Input.GetAxis("Mouse Y") * sensitivity; // 상하 회전 (Pitch)

        // FPS 모드일 때 플레이어 회전 동기화
        if (FPSview)
        {
            // 플레이어 회전 값 변경
            target.parent.Rotate(Vector3.up * Input.GetAxis("Mouse X") * sensitivity);

            // 카메라의 회전 값도 플레이어와 일치시킴
            _rotationX = target.parent.eulerAngles.y; // 플레이어의 Y 회전값에 동기화
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockCursor();
        }

        if (Input.GetMouseButtonDown(0) && !Cursor.visible)
        {
            LockCursor();
        }

        // FPS와 TPS 전환을 위한 토글 기능 추가
        if (isMaze)
        {
            if (startTrigger == null)
            {
                Debug.LogError("StartTrigger가 null입니다. Update 메서드에서 초기화하지 못했습니다.");
                return;
            }

            if (FPSview != previousFPSviewState || startTrigger.isMazeStart != previousMazeStartState)
            {
                if (FPSview && !startTrigger.isMazeStart)
                {
                    Debug.Log("TPS 모드로 전환됨");
                    FindLocalPlayer(); // TPS 모드로 전환
                }
                else if (startTrigger.isMazeStart)
                {
                    Debug.Log("FPS 모드로 전환됨");
                    FindLocalMazePlayer(); // FPS 모드로 전환
                }

                previousFPSviewState = FPSview;
                previousMazeStartState = startTrigger.isMazeStart;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isQuizActive)
        {
            return;
        }

        if (target == null) return;

        if (!FPSview)
        {
            // 3인칭 모드: 카메라가 타겟을 바라보며 플레이어의 뒤를 따라감
            Quaternion targetRotation = Quaternion.Euler(_rotationY, _rotationX, 0);
            Vector3 desiredPosition = target.position + targetRotation * offset;

            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.LookAt(target.position); // 카메라가 타겟을 바라보도록 설정
        }
        else
        {
            // 1인칭 모드: 카메라를 타겟의 위치에 고정하고 플레이어 회전을 따라감
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
