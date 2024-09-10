using UnityEngine;
using Photon.Pun;

public class InventionTrigger : MonoBehaviour
{
    public InventionType InventionType;
    private QuickSlotManager QuickSlotManager;

    private void Start()
    {
        QuickSlotManager = FindObjectOfType<QuickSlotManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView playerPhotonView = other.GetComponent<PhotonView>();

            // 로컬 플레이어인지 확인
            if (playerPhotonView.IsMine)
            {
                QuickSlotManager.ActivateAfterQuickSlot(InventionType);

                // GlobalInventionManager가 존재하는지 확인 후 활성화 처리
                if (GlobalInventionManager.Instance != null)
                {
                    GlobalInventionManager.Instance.SetInventionActive(InventionType, true);
                }
                else
                {
                    Debug.LogError("GlobalInventionManager instance is missing.");
                }

                // PhotonView를 가져와서 RPC 호출
                PhotonView photonView = PhotonView.Get(this);
                if (photonView != null)
                {
                    photonView.RPC("DestroyObject", RpcTarget.AllBuffered); // Buffered로 설정
                }
                else
                {
                    Debug.LogError("PhotonView를 찾을 수 없습니다.");
                }
            }
        }
    }

    [PunRPC]
    private void DestroyObject()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(this.gameObject); // PhotonNetwork.Destroy 사용
        }
    }
}
