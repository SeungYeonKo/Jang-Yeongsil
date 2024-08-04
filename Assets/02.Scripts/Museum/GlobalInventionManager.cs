using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalInventionManager : MonoBehaviour
{
    public static Dictionary<InventionType, bool> QuickSlotState = new Dictionary<InventionType, bool>();

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        // 초기 상태 설정 (필요에 따라 기본값 설정)
        QuickSlotState[InventionType.Sundial] = false;
        QuickSlotState[InventionType.ArmillarySphere] = false;
        QuickSlotState[InventionType.Cheugugi] = false;
        QuickSlotState[InventionType.AstronomicalChart] = false;
        QuickSlotState[InventionType.Clepsydra] = false;
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
