using System.Collections.Generic;
using UnityEngine;

public class MuseumManager : MonoBehaviour
{
    public GameObject[] MuseumInventionObjects;

    private Dictionary<string, InventionType> inventionNameMap = new Dictionary<string, InventionType>()
    {
        { "ArmillarySphere", InventionType.ArmillarySphere },
        { "Sundial", InventionType.Sundial },
        { "Cheugugi", InventionType.Cheugugi },
        { "AstronomicalChart", InventionType.AstronomicalChart },
        { "Clepsydra", InventionType.Clepsydra }
    };

    private void Start()
    {
        foreach (var obj in MuseumInventionObjects)
        {
            string inventionName = obj.name;
            if (inventionNameMap.TryGetValue(inventionName, out InventionType inventionType))
            {
                obj.SetActive(GlobalInventionManager.Instance.GetMuseumInventionState(inventionType));
            }
        }
    }
}
