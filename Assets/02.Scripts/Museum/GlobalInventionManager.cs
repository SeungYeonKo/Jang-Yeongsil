using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalInventionManager : MonoBehaviour
{
    public static Hashtable InventionState = new Hashtable();
    public static Hashtable QuickSlotState = new Hashtable(); // 퀵슬롯 상태 저장

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        // 초기 상태 설정 (필요에 따라 기본값 설정)
        InventionState["Sundial"] = false;
        InventionState["ArmillarySphere"] = false;
        InventionState["Cheugugi"] = false;
        InventionState["AstronomicalChart"] = false;
        InventionState["Clepsydra"] = false;
    }

    public static void SetInventionActive(string inventionName, bool isActive)
    {
        if (InventionState.ContainsKey(inventionName))
        {
            InventionState[inventionName] = isActive;
        }
    }

    public static bool IsInventionActive(string inventionName)
    {
        if (InventionState.ContainsKey(inventionName))
        {
            return (bool)InventionState[inventionName];
        }
        return false;
    }

    public static void SaveQuickSlotState(InventionType inventionType, bool isActive)
    {
        QuickSlotState[inventionType] = isActive;
    }

    public static bool GetQuickSlotState(InventionType inventionType)
    {
        if (QuickSlotState.ContainsKey(inventionType))
        {
            return (bool)QuickSlotState[inventionType];
        }
        return false;
    }
}
