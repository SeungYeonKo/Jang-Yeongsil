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

    private void Start()
    {
        offset = new Vector3(0, height, -distance);
        FindLocalPlayer();
    }

    public override void OnJoinedRoom()
    {
        FindLocalPlayer();
    }

    void Update()
    {
        if (target == null) return;

        rotationX += Input.GetAxis("Mouse X") * sensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * sensitivity;
        rotationY = Mathf.Clamp(rotationY, -90f, 90f);
    }

    private void FixedUpdate()
    {
        if (target == null) return;

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
}
