using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ItemSlotManager : MonoBehaviour
{
    // 슬롯 이미지를 저장하는 딕셔너리
    public Dictionary<ItemType, Image> itemSlotImages = new Dictionary<ItemType, Image>();
    // 아이템 설명을 저장하는 딕셔너리
    public Dictionary<ItemType, string> itemDescriptions = new Dictionary<ItemType, string>();

    // Inspector에서 할당할 수 있도록 아이템 슬롯 이미지들 배열
    public Image[] slotImages;
    public ItemType[] itemTypes;

    public GameObject EndNotice;

    public TextMeshProUGUI ItemDescriptionTexts;

    private void Start()
    {
        // ItemType에 따른 이미지 슬롯 설정
        for (int i = 0; i < slotImages.Length; i++)
        {
            itemSlotImages.Add(itemTypes[i], slotImages[i]);
            slotImages[i].color = new Color32(13, 13, 13, 255); // 초기에는 어두운 회색으로 설정
        }

        // 아이템 설명 텍스트
        itemDescriptions.Add(ItemType.Pasuho1, "물을 담는 큰 항아리인 대 파수호를 얻었어!");
        itemDescriptions.Add(ItemType.Pasuho2, "물을 담는 큰 항아리인 중 파수호를 얻었어!");
        itemDescriptions.Add(ItemType.Pasuho3, "물을 담는 큰 항아리인 소 파수호를 얻었어!");
        itemDescriptions.Add(ItemType.Susuho, "파수호에서 흘러오는 물을 담는 수수호를 얻었어!");
        itemDescriptions.Add(ItemType.Stick, "지렛대 장치를 건들이는 긴 막대인 잣대를 얻었어!");
        itemDescriptions.Add(ItemType.Drum, "시각을 알려주는 북 인형을 얻었어!");
        itemDescriptions.Add(ItemType.Jing, "시각을 알려주는 징 인형을 얻었어!");
        itemDescriptions.Add(ItemType.Bell, "시각을 알려주는 종 인형을 얻었어!");
        itemDescriptions.Add(ItemType.Bead, "일정한 시간이 되면 굴러가는 구슬을 얻었어!");
        itemDescriptions.Add(ItemType.Pulley, "부품 중 하나인 도르래를 얻었어!");

        EndNotice.gameObject.SetActive(false);
    }
    private void Update()
    {
        // Q 키가 눌렸는지 감지
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // 모든 슬롯 활성화
            ActivateAllSlots();
        }
    }
    private void ActivateAllSlots()
    {
        foreach (var itemType in itemTypes)
        {
            ActivateSlot(itemType); // 각 슬롯을 활성화
        }
    }

    // 아이템 타입에 맞는 슬롯 이미지를 활성화하는 메서드
    public void ActivateSlot(ItemType itemType)
    {
        if (itemSlotImages.ContainsKey(itemType))
        {
            itemSlotImages[itemType].color = new Color32(255, 255, 255, 255);

            // 아이템 설명을 표시
            if (itemDescriptions.ContainsKey(itemType))
            {
                ItemDescriptionTexts.gameObject.SetActive(true);

                // 코루틴을 이용하여 한 글자씩 텍스트 표시
                StartCoroutine(ShowTextWithTypingEffect(itemDescriptions[itemType], 0.08f));

                StartCoroutine(HideItemInfoTextAfterDelay(4f)); // 4초 후 텍스트 숨김
            }

            CheckSlotActivated();
        }
    }

    // 한 글자씩 텍스트를 출력하는 코루틴
    private IEnumerator ShowTextWithTypingEffect(string fullText, float delay)
    {
        ItemDescriptionTexts.text = ""; // 텍스트 초기화

        foreach (char letter in fullText.ToCharArray())
        {
            ItemDescriptionTexts.text += letter; // 한 글자씩 추가
            yield return new WaitForSeconds(delay); // 각 글자마다 딜레이
        }
    }

    private IEnumerator HideItemInfoTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ItemDescriptionTexts.gameObject.SetActive(false); 
    }

    private void CheckSlotActivated()
    {
        foreach (var slotImage in itemSlotImages.Values)
        {
            if (slotImage.color != new Color32(255, 255, 255, 255))
            {
                // 하나라도 활성화되지 않은 슬롯이 있으면 종료
                return;
            }
        }

        // 모든 슬롯이 활성화되었으면 EndNotice 활성화
        EndNotice.gameObject.SetActive(true);
    }

    // 모든 슬롯이 활성화되었는지 여부를 반환하는 메서드
    public bool AreAllSlotsActivated()
    {
        foreach (var slotImage in itemSlotImages.Values)
        {
            if (slotImage.color != new Color32(255, 255, 255, 255))
            {
                // 하나라도 활성화되지 않은 슬롯이 있으면 false 반환
                return false;
            }
        }

        // 모든 슬롯이 활성화되었으면 true 반환
        return true;
    }
}
