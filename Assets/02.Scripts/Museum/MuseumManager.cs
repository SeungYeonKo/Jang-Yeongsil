using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuseumManager : MonoBehaviourPun
{
    public GameObject[] MuseumInventionObjects;

    private void Start()
    {
        foreach (var obj in MuseumInventionObjects)
        {
            string inventionName = obj.name;

            if (GlobalInventionManager.Instance.GetInventionState(inventionName))
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
