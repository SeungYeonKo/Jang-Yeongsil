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
            Debug.Log("Player entered the trigger zone.");

            PhotonView playerPhotonView = other.GetComponent<PhotonView>();
            if (playerPhotonView.IsMine)
            {
                Debug.Log("Trigger event for local player.");

                QuickSlotManager.ActivateAfterQuickSlot(InventionType);

                // GlobalInventionManager가 존재하는지 확인 후 발명품 활성화 처리
                if (GlobalInventionManager.Instance != null)
                {
                    GlobalInventionManager.Instance.SetInventionActive(InventionType, true);
                    Debug.Log($"Invention {InventionType} activated in the museum.");
                }
                else
                {
                    Debug.LogError("GlobalInventionManager instance is missing.");
                }

                // PhotonView를 가져와서 RPC 호출
                PhotonView photonView = PhotonView.Get(this);
                if (photonView != null)
                {
                    Debug.Log("PhotonView found, calling DestroyObject RPC.");
                    photonView.RPC("DestroyObject", RpcTarget.AllBuffered); // Buffered로 설정하여 나중에 방에 들어온 플레이어도 반영
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
            Debug.Log("Destroying object as the MasterClient.");
            PhotonNetwork.Destroy(this.gameObject); // PhotonNetwork.Destroy 사용
        }
        else
        {
            Debug.Log("Not the MasterClient, skipping destroy.");
        }
    }
}
