using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TPSCamera : MonoBehaviourPunCallbacks
{
    public float distance = 3f;
    public float height = 2f;
    public float smoothSpeed = 0.125f;
    public float sensitivity = 2.0f;

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    private Vector3 offset;

    public Transform target;

    // 추가된 변수
    private bool isQuizActive = false;

    private void Start()
    {
        offset = new Vector3(0, height, -distance);

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
        if (target == null || isQuizActive) return; // 퀴즈가 활성화되면 카메라 회전 중지

        rotationX += Input.GetAxis("Mouse X") * sensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * sensitivity;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockCursor();
        }

        if (Input.GetMouseButtonDown(0) && !Cursor.visible)
        {
            LockCursor();
        }
    }

    private void FixedUpdate()
    {
        if (target == null || isQuizActive) return; // 퀴즈가 활성화되면 카메라 이동 중지

        rotationY = Mathf.Clamp(rotationY, -90f, 90f);

        Quaternion targetRotation = Quaternion.Euler(rotationY, rotationX, 0);
        Vector3 targetPosition = target.position + targetRotation * offset;

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        transform.LookAt(target.position);
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
                }
                else
                {
                    Debug.LogError("CameraRoot not found on player: " + player.name);
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
