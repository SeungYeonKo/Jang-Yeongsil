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

            if (GlobalInventionManager.IsInventionActive(inventionName))
            {
                obj.SetActive(true);
            }
            else
            {
                obj.SetActive(false);
            }
        }
    }
}
