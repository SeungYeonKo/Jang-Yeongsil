using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UIElements;

public class TPSCamera : MonoBehaviourPunCallbacks
{
    public float distance = 3f; // ī�޶�� ĳ���� ���� �Ÿ�
    public float height = 2f; // ī�޶��� ����
    public float smoothSpeed = 0.125f; // ī�޶� �̵��� �ε巴�� �ϱ� ���� �ӵ�
    public float sensitivity = 2.0f; // ī�޶� ȸ�� ����

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    private Vector3 offset; // �ʱ� ��ġ

    public Transform target; // ī�޶� ����ٴ� ��� ĳ������ Transform

    private void Start()
    {

        offset = new Vector3(0, height, -distance); // �ʱ� ��ġ ����
        Cursor.lockState = CursorLockMode.Locked;

        // �ڽ��� ĳ���� ã��
        FindLocalPlayer();
    }

    public override void OnJoinedRoom()
    {
        // �濡 ������ �� �ڽ��� ĳ���� �ٽ� ã��
        FindLocalPlayer();
    }

    void Update()
    {
        if (target == null) return; // Ÿ���� ������ ����

        rotationX += Input.GetAxis("Mouse X") * sensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * sensitivity;
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        rotationY = Mathf.Clamp(rotationY, -90f, 90f); // ���� ȸ�� ���� ����

        Quaternion targetRotation = Quaternion.Euler(rotationY, rotationX, 0); // ī�޶� ȸ���� ���
        Vector3 targetPosition = target.position + targetRotation * offset; // Ÿ�� ������ ��ġ ���



        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed); // �ε巯�� �̵� ���
        transform.LookAt(target.position); // ĳ���͸� �ٶ󺸵��� ����


    }


    private void FindLocalPlayer()
    {
        // ��� �÷��̾� ������Ʈ�� ã��
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // �� �÷��̾� ������Ʈ�� �˻�
        foreach (GameObject player in players)
        {
            PhotonView photonView = player.GetComponent<PhotonView>();
            string nickname = PlayerPrefs.GetString("LoggedInId");
            // �ڽ��� ĳ�������� Ȯ��
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
