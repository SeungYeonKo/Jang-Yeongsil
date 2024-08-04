using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalInventionManager : MonoBehaviour
{
    public static Dictionary<string, bool> InventionState = new Dictionary<string, bool>();

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        // 모든 발명품의 초기 상태를 false로 설정
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
            return InventionState[inventionName];
        }
        return false;
    }
}
