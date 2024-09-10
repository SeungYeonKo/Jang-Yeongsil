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
        // Singleton 패턴: 이미 인스턴스가 존재하면 새로 생성된 오브젝트 파괴
        if (Instance != null && Instance != this)
        {
            Debug.Log("GlobalInventionManager 중복 생성, 기존 인스턴스 사용.");
            Destroy(this.gameObject); // 중복된 인스턴스를 제거
            return;
        }

        // 처음 생성된 인스턴스 설정
        Instance = this;
        DontDestroyOnLoad(this.gameObject); // 씬 전환 시 파괴되지 않도록 설정
        Debug.Log("GlobalInventionManager 인스턴스 생성됨.");

        // PhotonView가 유효한지 확인
        if (photonView == null)
        {
            Debug.LogError("PhotonView가 할당되지 않았습니다.");
        }

        // 초기화 및 데이터 로드
        //LoadQuickSlotData();

        // 발명품 상태 초기화
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

        //SaveQuickSlotData(); // 상태가 변경될 때마다 저장
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

        //SaveQuickSlotData(); // 상태가 변경될 때마다 저장
    }

    public bool GetQuickSlotState(InventionType inventionType)
    {
        return QuickSlotState.ContainsKey(inventionType) && QuickSlotState[inventionType];
    }

    public bool GetMuseumInventionState(InventionType inventionType)
    {
        return MuseumInventionState.ContainsKey(inventionType) && MuseumInventionState[inventionType];
    }

   /* public void SaveQuickSlotData()
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
                QuickSlotState[invention] = false; // 기본값을 비활성화 상태로 설정
            }
        }
    }*/
}
