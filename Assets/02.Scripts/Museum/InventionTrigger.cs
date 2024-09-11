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
            if (playerPhotonView != null && playerPhotonView.IsMine)
            {
                // 퀵슬롯 상태 업데이트
                QuickSlotManager.ActivateAfterQuickSlot(InventionType);

                // 발명품 활성화 처리
                if (GlobalInventionManager.Instance != null)
                {
                    GlobalInventionManager.Instance.SetInventionActive(InventionType, true);
                    Debug.Log($"Invention {InventionType} activated.");
                }

                // 오브젝트 삭제 요청
                PhotonView photonView = GetComponent<PhotonView>();
                if (photonView != null)
                {
                    photonView.RPC("DestroyObject", RpcTarget.AllBuffered); // 모든 클라이언트에서 오브젝트 삭제
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
        PhotonNetwork.Destroy(this.gameObject);
    }
}
