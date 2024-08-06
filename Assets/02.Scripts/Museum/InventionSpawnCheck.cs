using Photon.Pun;
using UnityEngine;

public class InventionSpawnCheck : MonoBehaviour
{
    private GameObject Sundial;

    public Transform Spawner;
    private void Start()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("SunMiniGameOver"))
        {
            Sundial = Resources.Load<GameObject>(Sundial.name);
            PhotonNetwork.Instantiate(Sundial.name, Spawner.position, Spawner.rotation);
        }
    }
}
