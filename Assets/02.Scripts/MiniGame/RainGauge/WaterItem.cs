using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterItem : MonoBehaviour
{
    public float fallSpeed = 5f;
    public float deactivateHeight = -10f;

    private void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        if (transform.position.y <= deactivateHeight)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Jar"))
        {
            int jarNumber = GetJarNumber(other.gameObject);
            if (jarNumber != -1)
            {
                Debug.Log($"Increasing score for jar number: {jarNumber}");

                if (JarScore.Instance != null)
                {
                    PhotonView photonView = PhotonView.Get(JarScore.Instance);
                    photonView.RPC("IncreaseScore", Photon.Pun.RpcTarget.All, jarNumber, 100);
                }
                else
                {
                    Debug.LogError("JarScore.Instance is null");
                }
                gameObject.SetActive(false);
            }
        }
    }

    private int GetJarNumber(GameObject jar)
    {
        if (jar == JarScore.Instance.Jar1)
            return 1;
        else if (jar == JarScore.Instance.Jar2)
            return 2;
        else if (jar == JarScore.Instance.Jar3)
            return 3;
        else if (jar == JarScore.Instance.Jar4)
            return 4;
        else
            return -1;
    }
}
