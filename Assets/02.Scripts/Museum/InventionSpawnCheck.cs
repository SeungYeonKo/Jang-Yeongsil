using Photon.Pun;
using UnityEngine;

public class InventionSpawnCheck : MonoBehaviour
{
    public Transform sundialSpawnPoint;
    public Transform cheugugiSpawnPoint;
    public Transform astronomicalChartSpawnPoint;
    public Transform armillarySphereSpawnPoint;
    public Transform clepsydraSpawnPoint;

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
                if (sunMiniGameOver)
                {
                    // 프리팹의 기본 스케일 유지
                    PhotonNetwork.Instantiate("Sundial", sundialSpawnPoint.position, Quaternion.identity);
                    Debug.Log($"해시계가 생성됨, 위치: {sundialSpawnPoint.position}");
                }
            }

            // 측우기 미니게임
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("RainMiniGameOver"))
            {
                rainMiniGameOver = (bool)PhotonNetwork.LocalPlayer.CustomProperties["RainMiniGameOver"];
                if (rainMiniGameOver)
                {
                    PhotonNetwork.Instantiate("Cheugugi", cheugugiSpawnPoint.position, Quaternion.identity);
                    Debug.Log($"측우기가 생성됨, 위치: {cheugugiSpawnPoint.position}");
                }
            }

            // 천문도와 혼천의 미니게임
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("StarMiniGameOver"))
            {
                starMiniGameOver = (bool)PhotonNetwork.LocalPlayer.CustomProperties["StarMiniGameOver"];
                if (starMiniGameOver)
                {
                    PhotonNetwork.Instantiate("AstronomicalChart", astronomicalChartSpawnPoint.position, Quaternion.identity);
                    PhotonNetwork.Instantiate("ArmillarySphere", armillarySphereSpawnPoint.position, Quaternion.identity);
                    Debug.Log($"천문도와 혼천의가 생성됨, 위치: {astronomicalChartSpawnPoint.position}, {armillarySphereSpawnPoint.position}");
                }
            }

            // 자격루 미니게임
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("WaterClockMiniGameOver"))
            {
                waterClockMiniGameOver = (bool)PhotonNetwork.LocalPlayer.CustomProperties["WaterClockMiniGameOver"];
                if (waterClockMiniGameOver)
                {
                    PhotonNetwork.Instantiate("Clepsydra", clepsydraSpawnPoint.position, Quaternion.identity);
                    Debug.Log($"자격루가 생성됨, 위치: {clepsydraSpawnPoint.position}");
                }
            }
        }
        else
        {
            Debug.LogError("CurrentRoom이 null입니다. PhotonNetwork에 연결되어 있는지 확인하십시오.");
        }
    }
}
