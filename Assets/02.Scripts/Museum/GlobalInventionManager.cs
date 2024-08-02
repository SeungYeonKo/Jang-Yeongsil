using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalInventionManager : MonoBehaviour
{
    public static Hashtable InventionState = new Hashtable();


    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        InventionState["Sundial"] = false;
        InventionState["ArmillarySphere"] = false;
        InventionState["Cheugugi"] = false;
        InventionState["AstronomicalChart"] = false;
        InventionState["Clepsydra"] = false;
    }
}
