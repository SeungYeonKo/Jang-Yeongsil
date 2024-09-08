using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Photon.Pun; // PhotonNetwork를 사용하기 위한 네임스페이스 추가

public class EndTrigger : MonoBehaviour
{
    public GameObject EndTriggerObject;
    public TextMeshProUGUI FailText;
    public Image FadeInImage;

    private ItemSlotManager _itemSlotManager;

    private void Start()
    {
        _itemSlotManager = FindObjectOfType<ItemSlotManager>();

        FailText.gameObject.SetActive(false);

        // FadeInImage를 처음에 비활성화 상태로 설정
        FadeInImage.gameObject.SetActive(false);
        FadeInImage.color = new Color(FadeInImage.color.r, FadeInImage.color.g, FadeInImage.color.b, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_itemSlotManager.AreAllSlotsActivated())
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
        FadeInImage.gameObject.SetActive(true);

        // FadeInImage의 알파 값을 0에서 1로 변화시키고, 완료되면 메인 씬으로 이동
        FadeInImage.DOFade(1f, 1f).OnComplete(() =>
        {
            PhotonNetwork.LoadLevel("MainScene");
        });
    }

    private void ActivateFailText()
    {
        FailText.gameObject.SetActive(true); // FailText를 활성화
        FailText.DOFade(0f, 1f).SetDelay(2.5f).OnComplete(() =>
        {
            FailText.gameObject.SetActive(false); // 페이드 아웃이 완료되면 비활성화
            FailText.color = new Color(FailText.color.r, FailText.color.g, FailText.color.b, 1); // 알파 값을 다시 1로 설정
        });
    }
}
