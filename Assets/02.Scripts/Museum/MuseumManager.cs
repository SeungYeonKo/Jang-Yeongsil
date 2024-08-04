using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuseumManager : MonoBehaviour
{
    public GameObject[] MuseumInventionObjects;

    private void Start()
    {
        foreach (var obj in MuseumInventionObjects)
        {
            string inventionName = obj.name;

            // 발명품 상태를 확인하여 활성화
            if (GlobalInventionManager.IsInventionActive(inventionName))
            {
                obj.SetActive(true);
            }
            else
            {
                obj.SetActive(false);
            }

            Debug.Log($"Invention: {inventionName}, Active: {GlobalInventionManager.IsInventionActive(inventionName)}");
        }
    }
}
