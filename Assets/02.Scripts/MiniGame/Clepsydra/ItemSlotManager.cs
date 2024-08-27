using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotManager : MonoBehaviour
{
    // 슬롯 이미지를 저장하는 딕셔너리
    public Dictionary<ItemType, Image> itemSlotImages = new Dictionary<ItemType, Image>();

    // Inspector에서 할당할 수 있도록 아이템 슬롯 이미지들 배열
    public Image[] slotImages; 
    public ItemType[] itemTypes; 

    private void Start()
    {
        // ItemType에 따른 이미지 슬롯 설정
        for (int i = 0; i < slotImages.Length; i++)
        {
            itemSlotImages.Add(itemTypes[i], slotImages[i]);
            slotImages[i].gameObject.SetActive(false); // 초기에는 비활성화
        }
    }

    // 아이템 타입에 맞는 슬롯 이미지를 활성화하는 메서드
    public void ActivateSlot(ItemType itemType)
    {
        if (itemSlotImages.ContainsKey(itemType))
        {
            itemSlotImages[itemType].gameObject.SetActive(true);
        }
    }
}
