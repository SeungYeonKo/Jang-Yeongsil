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
                GlobalInventionManager.Instance.SetInventionActive(InventionType, true);
                PhotonView photonView = PhotonView.Get(this);
                photonView.RPC("DestroyObject", RpcTarget.AllBuffered); // Buffered로 설정
            }
        }
    }

    [PunRPC]
    private void DestroyObject()
    {
        Destroy(this.gameObject);
    }
}
