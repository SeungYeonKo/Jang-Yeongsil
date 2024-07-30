using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePortal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PhotonView photonView = other.GetComponent<PhotonView>();
        if (other.CompareTag("Player") && photonView.IsMine)
        {
            if (gameObject.name == "RainGaugePortal")
            {
                PhotonManager.Instance.LeaveAndLoadRoom("MiniGame1");
            }
            if (gameObject.name == "MuseumPortal")
            {
                PhotonManager.Instance.LeaveAndLoadRoom("MuseumScene");
            }
        }
    }
}
