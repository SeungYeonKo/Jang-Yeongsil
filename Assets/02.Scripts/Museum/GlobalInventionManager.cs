using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
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
        ExitGames.Client.Photon.Hashtable inventionState = new ExitGames.Client.Photon.Hashtable();
        inventionState[inventionName] = state;
        PhotonNetwork.LocalPlayer.SetCustomProperties(inventionState);

        // 커스텀 프로퍼티 변경 이벤트를 수동으로 발생시킴
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(1, inventionState, options, SendOptions.SendReliable);
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

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 1) // 이벤트 코드 1로 설정
        {
            ExitGames.Client.Photon.Hashtable changedProps = (ExitGames.Client.Photon.Hashtable)photonEvent.CustomData;

            foreach (System.Collections.DictionaryEntry prop in changedProps)
            {
                string inventionName = prop.Key as string;
                bool state = (bool)prop.Value;

                // 박물관 오브젝트 상태 업데이트 로직을 여기에 추가
                // 필요한 경우 해당 씬에서 오브젝트를 찾아 상태를 업데이트
                Debug.Log($"Invention '{inventionName}' state updated to: {state}");
            }
        }
    }
}
