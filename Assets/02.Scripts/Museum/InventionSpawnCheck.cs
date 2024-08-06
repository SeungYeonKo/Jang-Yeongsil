using Photon.Pun;
using UnityEngine;

public class InventionSpawnCheck : MonoBehaviour
{
    public GameObject Sundial;
    public GameObject Cheugugi;

    private void Start()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            bool sunMiniGameOver = false;
            bool rainMiniGameOver = false;

            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("SunMiniGameOver"))
            {
                sunMiniGameOver = (bool)PhotonNetwork.LocalPlayer.CustomProperties["SunMiniGameOver"];
                Sundial.SetActive(sunMiniGameOver);
                Debug.Log($"SunMiniGameOver 상태: {sunMiniGameOver}");
            }
            else
            {
                Sundial.SetActive(sunMiniGameOver);
                Debug.Log("아직 Sundial 값이 없습니다.");
            }

            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("RainMiniGameOver"))
            {
                rainMiniGameOver = (bool)PhotonNetwork.LocalPlayer.CustomProperties["RainMiniGameOver"];
                Cheugugi.SetActive(rainMiniGameOver);
                Debug.Log($"RainMiniGameOver 상태: {rainMiniGameOver}");
            }
            else
            {
                Cheugugi.SetActive(rainMiniGameOver);
                Debug.Log("아직 Cheugugi 값이 없습니다.");
            }
        }
        else
        {
            Debug.LogError("CurrentRoom이 null입니다. PhotonNetwork에 연결되어 있는지 확인하십시오.");
        }
    }
}
