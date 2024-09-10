using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GlobalInventionManager : MonoBehaviourPunCallbacks
{
    public static GlobalInventionManager Instance { get; private set; }

    public Dictionary<InventionType, bool> InventionState = new Dictionary<InventionType, bool>();
    public Dictionary<InventionType, bool> QuickSlotState = new Dictionary<InventionType, bool>();
    public Dictionary<InventionType, bool> MuseumInventionState = new Dictionary<InventionType, bool>(); // 박물관 활성화 상태

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        // photonView 초기화 확인
        if (photonView == null)
        {
            Debug.LogError("PhotonView가 할당되지 않았습니다.");
        }

        LoadQuickSlotData(); // 퀵슬롯 데이터 불러오기

        // 초기 상태 설정
        foreach (InventionType invention in System.Enum.GetValues(typeof(InventionType)))
        {
            if (!InventionState.ContainsKey(invention))
                InventionState[invention] = false;

            if (!QuickSlotState.ContainsKey(invention))
                QuickSlotState[invention] = false;

            if (!MuseumInventionState.ContainsKey(invention)) // 박물관 초기 상태 설정
                MuseumInventionState[invention] = false;
        }
    }

    [PunRPC]
    public void SetInventionActive(InventionType inventionType, bool isActive)
    {
        InventionState[inventionType] = isActive;
        QuickSlotState[inventionType] = isActive;
        MuseumInventionState[inventionType] = isActive; // 박물관 상태도 활성화

        SaveQuickSlotData(); // 상태가 변경될 때마다 저장
    }

    public void SaveQuickSlotState(InventionType inventionType, bool isActive)
    {
        QuickSlotState[inventionType] = isActive;

        // photonView가 null이 아닌지 확인 후 RPC 호출
        if (photonView != null)
        {
            photonView.RPC("SetInventionActive", RpcTarget.AllBuffered, inventionType, isActive); // 모든 클라이언트에 상태 동기화 (Buffered 사용)
        }
        else
        {
            Debug.LogError("PhotonView가 null입니다. SetInventionActive를 호출할 수 없습니다.");
        }

        SaveQuickSlotData(); // 상태가 변경될 때마다 저장
    }

    public bool GetQuickSlotState(InventionType inventionType)
    {
        return QuickSlotState.ContainsKey(inventionType) && QuickSlotState[inventionType];
    }

    public bool GetMuseumInventionState(InventionType inventionType)
    {
        return MuseumInventionState.ContainsKey(inventionType) && MuseumInventionState[inventionType];
    }

    public void SaveQuickSlotData()
    {
        foreach (var entry in QuickSlotState)
        {
            PlayerPrefs.SetInt(entry.Key.ToString(), entry.Value ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    public void LoadQuickSlotData()
    {
        foreach (InventionType invention in System.Enum.GetValues(typeof(InventionType)))
        {
            if (PlayerPrefs.HasKey(invention.ToString()))
            {
                QuickSlotState[invention] = PlayerPrefs.GetInt(invention.ToString()) == 1;
            }
            else
            {
                QuickSlotState[invention] = false;
            }
        }
    }
}
