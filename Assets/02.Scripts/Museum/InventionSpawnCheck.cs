using Photon.Pun;
using UnityEngine;

public class InventionSpawnCheck : MonoBehaviour
{
    public GameObject Sundial;
    public GameObject Cheugugi;
    public GameObject AstronomicalChart;
    public GameObject ArmillarySphere;
    public GameObject Clepsydra;

    private void Start()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            bool sunMiniGameOver = false;
            bool rainMiniGameOver = false;
            bool starMiniGameOver = false;
            bool waterClockMiniGameOver = false;

            // 해시계 미니게임
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

            // 측우기 미니게임
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

            // 천문도, 혼천의 미니게임
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("StarMiniGameOver"))
            {
                starMiniGameOver = (bool)PhotonNetwork.LocalPlayer.CustomProperties["StarMiniGameOver"];
                AstronomicalChart.SetActive(starMiniGameOver);
                ArmillarySphere.SetActive(starMiniGameOver);
                Debug.Log($"StarMiniGameOver 상태: {starMiniGameOver}");
            }
            else
            {
                AstronomicalChart.SetActive(starMiniGameOver);
                ArmillarySphere.SetActive(starMiniGameOver);
                Debug.Log("아직 AstronomicalChart 값이 없습니다.");
                Debug.Log("아직 ArmillarySphere 값이 없습니다.");
            }

            // 자격루 미니게임
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("WaterClockMiniGameOver"))
            {
                waterClockMiniGameOver = (bool)PhotonNetwork.LocalPlayer.CustomProperties["WaterClockMiniGameOver"];
                Clepsydra.SetActive(waterClockMiniGameOver);
                Debug.Log($"WaterClockMiniGameOver 상태: {waterClockMiniGameOver}");
            }
            else
            {
                Clepsydra.SetActive(waterClockMiniGameOver);
                Debug.Log("아직 Clepsydra 값이 없습니다.");
            }
        }
        else
        {
            Debug.LogError("CurrentRoom이 null입니다. PhotonNetwork에 연결되어 있는지 확인하십시오.");
        }
    }
}
