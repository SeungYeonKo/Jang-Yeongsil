using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerMoveAbility))]


public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    public PhotonView PhotonView { get; private set; }

    private void Awake()
    {
        PhotonView = GetComponent<PhotonView>();
        if (PhotonView.IsMine)
        {
            //UI_Minimap.Instance.MyPlayer = this;
        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }

    // 임시 이동 코드
    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "VillageScene")
        {
            if (Input.GetKeyDown(KeyCode.Alpha8)) // 타일 MiniGame1
            {
                this.transform.position = new Vector3(-84, 13, 110);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9)) // 짭가이즈 MiniGame2
            {
                this.transform.position = new Vector3(75, 10, 20);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0)) // 타워 MiniGame3
            {
                this.transform.position = new Vector3(-80, 10, -105);
            }
        }
    }
}
