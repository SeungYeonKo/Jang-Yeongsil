using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerOptionAbility : PlayerAbility
{
    public void Pause()
    {
        if (!_owner.photonView.IsMine) return;
        photonView.RPC("RPC_Pause", RpcTarget.AllBuffered, photonView.ViewID);
        _owner.photonView.GetComponent<Animator>().SetFloat("Move", 0f);
    }
    public void Continue()
    {
        if (!_owner.photonView.IsMine) return;
        photonView.RPC("RPC_Continue", RpcTarget.AllBuffered, photonView.ViewID);
    }
    [PunRPC]
    private void RPC_Pause(int viewID)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null && targetView.IsMine)
        {
            if (targetView.gameObject.TryGetComponent<PlayerMoveAbility>(out var moveAbility))
            {
                moveAbility.enabled = false;
                Debug.Log($"{moveAbility}");
            }
        }
    }
    
    [PunRPC]
    private void RPC_Continue(int viewID)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null && targetView.IsMine)
        {
            if (targetView.gameObject.TryGetComponent<PlayerMoveAbility>(out var moveAbility))
            {
                moveAbility.enabled = true;
            }
        }
    }
}
