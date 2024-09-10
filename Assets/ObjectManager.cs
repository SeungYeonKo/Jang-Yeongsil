using UnityEngine;
using Photon.Pun;

public class ObjectManager : MonoBehaviourPun
{
    // 오브젝트를 활성화/비활성화하는 함수
    public void SetObjectActive(GameObject targetObject, bool isActive)
    {
        if (PhotonNetwork.IsMasterClient) // 마스터 클라이언트만 RPC를 호출
        {
            photonView.RPC("SyncObjectActiveState", RpcTarget.AllBuffered, targetObject.GetComponent<PhotonView>().ViewID, isActive);
        }
    }

    // RPC로 활성화 상태 동기화
    [PunRPC]
    public void SyncObjectActiveState(int objectViewID, bool isActive)
    {
        PhotonView targetPhotonView = PhotonView.Find(objectViewID); // ViewID로 PhotonView 찾기
        if (targetPhotonView != null)
        {
            targetPhotonView.gameObject.SetActive(isActive); // 오브젝트 활성화/비활성화
        }
        else
        {
            Debug.LogError("PhotonView를 찾을 수 없습니다.");
        }
    }
}
