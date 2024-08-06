using Photon.Pun;
using UnityEngine;

public class InventionSpawnCheck : MonoBehaviour
{
    public GameObject Sundial;

    private void Start()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            bool sunMiniGameOver = false;
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("SunMiniGameOver"))
            {
                sunMiniGameOver = (bool)PhotonNetwork.LocalPlayer.CustomProperties["SunMiniGameOver"];
                Debug.Log($"SunMiniGameOver 상태: {sunMiniGameOver}");
            }

            else
            {
                Sundial.SetActive(sunMiniGameOver);
                Debug.Log("아직 Sundial 값이 없습니다.");
            }
        }
        else
        {
            Debug.LogError("CurrentRoom이 null입니다. PhotonNetwork에 연결되어 있는지 확인하십시오.");
        }
    }
}