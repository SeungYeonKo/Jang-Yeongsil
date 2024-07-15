using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TPSCamera : MonoBehaviourPunCallbacks
{
    public float distance = 3f; // 카메라와 캐릭터 간의 거리
    public float height = 2f; // 카메라의 높이
    public float smoothSpeed = 0.125f; // 카메라 이동을 부드럽게 하기 위한 속도
    public float sensitivity = 2.0f; // 카메라 회전 감도

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    private Vector3 offset; // 초기 위치

    public Transform target; // 카메라가 따라다닐 대상 캐릭터의 Transform

    private void Start()
    {

        offset = new Vector3(0, height, -distance); // 초기 위치 설정
        Cursor.lockState = CursorLockMode.Locked;

        // 자신의 캐릭터 찾기
        FindLocalPlayer();
    }

    public override void OnJoinedRoom()
    {
        // 방에 들어왔을 때 자신의 캐릭터 다시 찾기
        FindLocalPlayer();
    }

    void Update()
    {
        if (target == null) return; // 타겟이 없으면 리턴

        rotationX += Input.GetAxis("Mouse X") * sensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * sensitivity;
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        rotationY = Mathf.Clamp(rotationY, -90f, 90f); // 상하 회전 각도 제한

        Quaternion targetRotation = Quaternion.Euler(rotationY, rotationX, 0); // 카메라 회전값 계산
        Vector3 targetPosition = target.position + targetRotation * offset; // 타겟 주위의 위치 계산



        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed); // 부드러운 이동 계산
        transform.LookAt(target.position); // 캐릭터를 바라보도록 설정


    }


    private void FindLocalPlayer()
    {
        // 모든 플레이어 오브젝트를 찾음
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // 각 플레이어 오브젝트를 검사
        foreach (GameObject player in players)
        {
            PhotonView photonView = player.GetComponent<PhotonView>();
            string nickname = PlayerPrefs.GetString("LoggedInId");
            // 자신의 캐릭터인지 확인
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
}
