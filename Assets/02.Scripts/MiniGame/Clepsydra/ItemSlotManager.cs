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

    public GameObject EndNotice;

    private void Start()
    {
        // ItemType에 따른 이미지 슬롯 설정
        for (int i = 0; i < slotImages.Length; i++)
        {
            itemSlotImages.Add(itemTypes[i], slotImages[i]);
            slotImages[i].color = new Color32(13, 13, 13, 255); // 초기에는 어두운 회색으로 설정
        }

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

            CheckSlotActivated();
        }
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
