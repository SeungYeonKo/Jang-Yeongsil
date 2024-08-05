using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SundialPortal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PhotonView photonView = other.GetComponent<PhotonView>();
        if (other.CompareTag("Player") && photonView.IsMine)
        {
            if (gameObject.name == "SunGaugePortal")
            {
                PhotonManager.Instance.LeaveAndLoadRoom("MiniGame2");
            }
        }
    }
}
