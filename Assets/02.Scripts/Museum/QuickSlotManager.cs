using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class QuickSlotManager : MonoBehaviour
{
    public GameObject[] BeforeQuickSlots;
    public GameObject[] AfterQuickSlots;

    public TextMeshProUGUI InventionReleasedText;

    private Dictionary<InventionType, int> inventionSlotMap = new Dictionary<InventionType, int>()
    {
        { InventionType.ArmillarySphere, 0 },
        { InventionType.Sundial, 1 },
        { InventionType.Cheugugi, 2 },
        { InventionType.AstronomicalChart, 3 },
        { InventionType.Clepsydra, 4 }
    };

    private void Start()
    {
        // 씬 전환 시에는 퀵슬롯 상태를 복원
        RestoreQuickSlotStateFromPlayerPrefs();
    }

    private void OnApplicationQuit()
    {
        // 게임이 완전히 종료될 때 퀵슬롯 상태 초기화
        ResetQuickSlotState();
    }

    private void RestoreQuickSlotStateFromPlayerPrefs()
    {
        foreach (var entry in inventionSlotMap)
        {
            InventionType inventionType = entry.Key;
            int slotIndex = entry.Value;

            // PlayerPrefs에서 상태 불러오기
            if (PlayerPrefs.HasKey(inventionType.ToString()))
            {
                bool isActive = PlayerPrefs.GetInt(inventionType.ToString()) == 1;

                if (isActive)
                {
                    AfterQuickSlots[slotIndex]?.SetActive(true);
                    BeforeQuickSlots[slotIndex]?.SetActive(false);
                }
                else
                {
                    AfterQuickSlots[slotIndex]?.SetActive(false);
                    BeforeQuickSlots[slotIndex]?.SetActive(true);
                }
            }
            else
            {
                AfterQuickSlots[slotIndex]?.SetActive(false);
                BeforeQuickSlots[slotIndex]?.SetActive(true);
            }
        }
    }

    public void SaveQuickSlotStateWithPlayerPrefs(InventionType inventionType, bool isActive)
    {
        PlayerPrefs.SetInt(inventionType.ToString(), isActive ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log($"PlayerPrefs에 {inventionType} 상태 저장됨: {isActive}");
    }

    private void ResetQuickSlotState()
    {
        // 게임을 완전히 종료할 때 PlayerPrefs 초기화
        foreach (var entry in inventionSlotMap)
        {
            PlayerPrefs.DeleteKey(entry.Key.ToString());
        }
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs 퀵슬롯 상태가 초기화되었습니다.");
    }

    public void ActivateAfterQuickSlot(InventionType inventionType)
    {
        if (inventionSlotMap.TryGetValue(inventionType, out int slotIndex))
        {
            AfterQuickSlots[slotIndex].SetActive(true);
            BeforeQuickSlots[slotIndex].SetActive(false);

            // 퀵슬롯 상태 저장
            SaveQuickSlotStateWithPlayerPrefs(inventionType, true);
        }
    }
}
