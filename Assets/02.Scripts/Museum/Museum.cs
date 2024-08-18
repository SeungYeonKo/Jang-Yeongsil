using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun; // PhotonNetwork를 사용하기 위해 추가

public enum TriggerType
{
    SundialTrigger,
    CheugugiTrigger,
    ClepsydraTrigger,
    ArmillarySphereTrigger,
    AstronomicalChartTrigger,
}

public class Museum : MonoBehaviour
{
    public TriggerType TriggerType;

    public Image[] InventionMentImages;

    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        DeactivateAllImages();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView playerPhotonView = other.GetComponent<PhotonView>();

            // 로컬 플레이어인지 확인
            if (playerPhotonView.IsMine)
            {
                // 트리거 타입에 따른 이미지 활성화
                switch (TriggerType)
                {
                    case TriggerType.SundialTrigger:
                        Debug.Log("해시계트리거");
                        ActivateImage(0);
                        break;
                    case TriggerType.CheugugiTrigger:
                        Debug.Log("측우기트리거");
                        ActivateImage(1);
                        break;
                    case TriggerType.ClepsydraTrigger:
                        Debug.Log("자격루트리거");
                        ActivateImage(2);
                        break;
                    case TriggerType.ArmillarySphereTrigger:
                        Debug.Log("혼천의트리거");
                        ActivateImage(3);
                        break;
                    case TriggerType.AstronomicalChartTrigger:
                        Debug.Log("천문도트리거");
                        ActivateImage(4);
                        break;
                    default:
                        DeactivateAllImages();
                        break;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView playerPhotonView = other.GetComponent<PhotonView>();

            // 로컬 플레이어인지 확인
            if (playerPhotonView.IsMine)
            {
                Debug.Log("트리거 벗어남");
                DeactivateAllImages();
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

    private void DeactivateAllImages()
    {
        foreach (var image in InventionMentImages)
        {
            image.gameObject.SetActive(false);
        }
    }
}
