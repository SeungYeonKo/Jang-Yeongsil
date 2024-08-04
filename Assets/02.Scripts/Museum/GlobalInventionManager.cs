using System.Collections.Generic;
using UnityEngine;

public class GlobalInventionManager : MonoBehaviour
{
    public static Dictionary<InventionType, bool> InventionState = new Dictionary<InventionType, bool>();
    public static Dictionary<InventionType, bool> QuickSlotState = new Dictionary<InventionType, bool>();

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        // 초기 상태 설정 (필요에 따라 기본값 설정)
        InventionState[InventionType.Sundial] = false;
        InventionState[InventionType.ArmillarySphere] = false;
        InventionState[InventionType.Cheugugi] = false;
        InventionState[InventionType.AstronomicalChart] = false;
        InventionState[InventionType.Clepsydra] = false;
    }

    public static void SetInventionActive(InventionType inventionType, bool isActive)
    {
        if (InventionState.ContainsKey(inventionType))
        {
            InventionState[inventionType] = isActive;
        }
        else
        {
            InventionState.Add(inventionType, isActive);
        }
    }

    public static bool IsInventionActive(InventionType inventionType)
    {
        if (InventionState.ContainsKey(inventionType))
        {
            return InventionState[inventionType];
        }
        return false;
    }

    public static void SaveQuickSlotState(InventionType inventionType, bool isActive)
    {
        if (QuickSlotState.ContainsKey(inventionType))
        {
            QuickSlotState[inventionType] = isActive;
        }
        else
        {
            QuickSlotState.Add(inventionType, isActive);
        }
    }

    public static bool GetQuickSlotState(InventionType inventionType)
    {
        if (QuickSlotState.ContainsKey(inventionType))
        {
            return QuickSlotState[inventionType];
        }
        return false;
    }
}
