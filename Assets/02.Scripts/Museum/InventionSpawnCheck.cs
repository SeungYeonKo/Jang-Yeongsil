using Photon.Pun;
using UnityEngine;

public class InventionSpawnCheck : MonoBehaviour
{

    public Transform Spawner;
    private void Start()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("SunMiniGameOver"))
        {
            string SundialName = "Sundial";
            Debug.Log($"불러오는 중: {SundialName}"); 
            PhotonNetwork.Instantiate(SundialName, Spawner.position, Spawner.rotation); 
            Debug.Log("Sundial instantiated successfully.");

        }
        else
        {
            Debug.Log("아직 Sundial 값이 없습니다.");
        }
    }
}