using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToGwanghwamunPortal : MonoBehaviour
{
    public GameObject BacktoGwanghwamunPortal;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("광장으로 돌아가기 트리거");
            PhotonNetwork.LoadLevel("MainScene");
            return;
        }
    }
}
