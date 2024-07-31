using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public Image[] InventionMentImages;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (TriggerType)
            {
                case TriggerType.SundialTrigger:
                    ActivateImage(0);
                    break;
                case TriggerType.CheugugiTrigger:
                    ActivateImage(1);
                    break;
                case TriggerType.ClepsydraSundialTrigger:
                    ActivateImage(2);
                    break;
                case TriggerType.ArmillarySphereTrigger:
                    ActivateImage(3);
                    break;
                case TriggerType.AstronomicalChartTrigger:
                    ActivateImage(4);
                    break;
                default:
                    DeactivateAllImages();
                    break;
            }
        }
    }

    private void ActivateImage(int index)
    {
        for (int i = 0; i < InventionMentImages.Length; i++)
        {
            InventionMentImages[i].gameObject.SetActive(i == index);
        }
    }


    // 나머지 다 비활성화
    private void DeactivateAllImages()
    {
        foreach (var image in InventionMentImages)
        {
            image.gameObject.SetActive(false);
        }
    }
}
