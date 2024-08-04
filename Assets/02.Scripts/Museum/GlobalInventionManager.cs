using System.Collections.Generic;
using UnityEngine;

public class GlobalInventionManager : MonoBehaviour
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

    public void SetInventionActive(InventionType inventionType, bool isActive)
    {
        InventionState[inventionType] = isActive;
        QuickSlotState[inventionType] = isActive;
        MuseumInventionState[inventionType] = isActive; // 박물관 상태도 활성화
    }

    public bool IsInventionActive(InventionType inventionType)
    {
        return InventionState.ContainsKey(inventionType) && InventionState[inventionType];
    }

    public void SaveQuickSlotState(InventionType inventionType, bool isActive)
    {
        QuickSlotState[inventionType] = isActive;
    }

    public bool GetQuickSlotState(InventionType inventionType)
    {
        return QuickSlotState.ContainsKey(inventionType) && QuickSlotState[inventionType];
    }

    public bool GetMuseumInventionState(InventionType inventionType)
    {
        return MuseumInventionState.ContainsKey(inventionType) && MuseumInventionState[inventionType];
    }
}
