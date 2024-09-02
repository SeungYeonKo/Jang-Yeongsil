using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Photon.Pun;
using ExitGames.Client.Photon; // Photon의 Hashtable을 사용하기 위한 네임스페이스 추가

public class EndTrigger : MonoBehaviour
{
    public GameObject EndTriggerObject;
    public TextMeshProUGUI FailText;
    public Image FadeInImage;

    private ItemSlotManager _itemSlotManager;

    private void Start()
    {
        // ItemSlotManager 초기화
        _itemSlotManager = FindObjectOfType<ItemSlotManager>();

        if (_itemSlotManager == null)
        {
            Debug.LogError("ItemSlotManager를 찾을 수 없습니다. 씬에 추가되어 있는지 확인하십시오.");
        }

        // FailText가 할당되었는지 확인 후 비활성화
        if (FailText != null)
        {
            FailText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("FailText가 할당되지 않았습니다.");
        }

        // FadeInImage 초기 설정 및 Null 체크
        if (FadeInImage != null)
        {
            FadeInImage.gameObject.SetActive(false);
            FadeInImage.color = new Color(FadeInImage.color.r, FadeInImage.color.g, FadeInImage.color.b, 0);
        }
        else
        {
            Debug.LogError("FadeInImage가 할당되지 않았습니다.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_itemSlotManager != null && _itemSlotManager.AreAllSlotsActivated())
            {
                // 모든 슬롯이 활성화된 경우, Fade-in 효과를 실행
                FadeInEffect();
            }
            else
            {
                // 모든 슬롯이 활성화되지 않았으면 FailText 활성화
                ActivateFailText();
            }
        }
    }

    private void FadeInEffect()
    {
        // FadeInImage를 활성화
        if (FadeInImage != null)
        {
            FadeInImage.gameObject.SetActive(true);

            // FadeInImage의 알파 값을 0에서 1로 변화시키고, 완료되면 메인 씬으로 이동
            FadeInImage.DOFade(1f, 1f).OnComplete(() =>
            {
                if (PhotonNetwork.InRoom) // PhotonNetwork가 룸에 있는지 확인
                {
                    PhotonNetwork.LoadLevel("MainScene");
                }
                else
                {
                    Debug.LogError("PhotonNetwork에 연결되어 있지 않습니다.");
                }
            });

            if (PhotonNetwork.IsMasterClient)
            {
                ExitGames.Client.Photon.Hashtable waterClockMiniGameProps = new ExitGames.Client.Photon.Hashtable { { "WaterClockMiniGameOver", true } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(waterClockMiniGameProps);
            }
        }
        else
        {
            Debug.LogError("FadeInImage가 null입니다.");
        }
    }

    private void ActivateFailText()
    {
        if (FailText != null)
        {
            FailText.gameObject.SetActive(true); // FailText를 활성화
            FailText.DOFade(0f, 1f).SetDelay(1.5f).OnComplete(() =>
            {
                FailText.gameObject.SetActive(false); // 페이드 아웃이 완료되면 비활성화
                FailText.color = new Color(FailText.color.r, FailText.color.g, FailText.color.b, 1); // 알파 값을 다시 1로 설정
            });
        }
        else
        {
            Debug.LogError("FailText가 null입니다.");
        }
    }
}
