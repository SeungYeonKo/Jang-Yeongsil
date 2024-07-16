using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RainGaugePlayer : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        if (SceneManager.GetActiveScene().name != "RainGauge")
        {
            this.enabled = false;
            return;
        }
        if (!photonView.IsMine) return;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
