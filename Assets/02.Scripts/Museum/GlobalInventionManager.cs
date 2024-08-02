using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class GlobalInventionManager : MonoBehaviourPunCallbacks
{
    public static GlobalInventionManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateInventionState(string inventionName, bool state)
    {
        // 현재 플레이어의 Custom Properties 업데이트
        Hashtable inventionState = new Hashtable();
        inventionState[inventionName] = state;
        PhotonNetwork.LocalPlayer.SetCustomProperties(inventionState);
    }

    public bool GetInventionState(string inventionName)
    {
        // 현재 플레이어의 Custom Properties에서 상태 가져오기
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(inventionName, out object value))
        {
            return (bool)value;
        }
        return false;
    }
}
