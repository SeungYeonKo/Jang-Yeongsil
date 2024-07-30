using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerType
{
    SundialTrigger,
    CheugugiTrigger,
    AstronomicalChartTrigger,
    ArmillarySphereTrigger,
    ClepsydraSundialTrigger
}

public class Museum : MonoBehaviour
{
    public TriggerType TriggerType;

    private void OnTriggerEnter(Collider other)
    {
        if (CompareTag("Player"))
        {
          
        }
    }

}
