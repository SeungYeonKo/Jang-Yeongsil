using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GlobalInventionManager : MonoBehaviourPunCallbacks
{
    public static GlobalInventionManager Instance { get; private set; }

    // 발명품 상태를 관리하는 딕셔너리들
    public Dictionary<InventionType, bool> InventionState = new Dictionary<InventionType, bool>();
    public Dictionary<InventionType, bool> QuickSlotState = new Dictionary<InventionType, bool>();
    public Dictionary<InventionType, bool> MuseumInventionState = new Dictionary<InventionType, bool>(); // 박물관 활성화 상태

    private void Awake()
    {
        // Singleton 패턴: 이미 인스턴스가 존재하면 중복 생성된 오브젝트 파괴
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject); // 중복된 인스턴스를 제거
            return;
        }

        // 처음 생성된 인스턴스 설정
        Instance = this;
        DontDestroyOnLoad(this.gameObject); // 씬 전환 시 파괴되지 않도록 설정

        // 발명품 상태 초기화
        InitializeInventionStates();
    }

    // 발명품 상태 초기화
    private void InitializeInventionStates()
    {
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

    // 발명품 활성화 동기화
    [PunRPC]
    public void SetInventionActive(InventionType inventionType, bool isActive)
    {
        // 발명품과 퀵슬롯 상태를 갱신
        InventionState[inventionType] = isActive;
        QuickSlotState[inventionType] = isActive;
        MuseumInventionState[inventionType] = isActive; // 박물관 상태도 활성화

        // 퀵슬롯 상태를 모든 클라이언트에 동기화
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncInventionState", RpcTarget.All, inventionType, isActive);
        }
    }

    // 퀵슬롯 상태 저장 및 동기화
    public void SaveQuickSlotState(InventionType inventionType, bool isActive)
    {
        QuickSlotState[inventionType] = isActive;

        // photonView가 null이 아닌지 확인 후 상태 동기화
        if (photonView != null)
        {
            photonView.RPC("SetInventionActive", RpcTarget.AllBuffered, inventionType, isActive); // 모든 클라이언트에 상태 동기화 (Buffered 사용)
        }
        else
        {
            Debug.LogError("PhotonView가 null입니다. SetInventionActive를 호출할 수 없습니다.");
        }
    }

    // 퀵슬롯 상태 불러오기
    public bool GetQuickSlotState(InventionType inventionType)
    {
        return QuickSlotState.ContainsKey(inventionType) && QuickSlotState[inventionType];
    }

    // 박물관 발명품 상태 불러오기
    public bool GetMuseumInventionState(InventionType inventionType)
    {
        return MuseumInventionState.ContainsKey(inventionType) && MuseumInventionState[inventionType];
    }

    // 퀵슬롯 상태를 모든 클라이언트에 동기화하는 RPC
    [PunRPC]
    private void SyncInventionState(InventionType inventionType, bool isActive)
    {
        InventionState[inventionType] = isActive;
        QuickSlotState[inventionType] = isActive;
        MuseumInventionState[inventionType] = isActive;
    }
}
