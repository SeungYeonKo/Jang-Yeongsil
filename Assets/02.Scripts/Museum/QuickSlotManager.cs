using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

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

    private Dictionary<InventionType, string> inventionMessages = new Dictionary<InventionType, string>()
    {
        { InventionType.ArmillarySphere, "혼천의가 해금되었습니다. 박물관에서 확인하세요!" },
        { InventionType.Sundial, "해시계가 해금되었습니다. 박물관에서 확인하세요!" },
        { InventionType.Cheugugi, "측우기가 해금되었습니다. 박물관에서 확인하세요!" },
        { InventionType.AstronomicalChart, "천문도가 해금되었습니다. 박물관에서 확인하세요!" },
        { InventionType.Clepsydra, "자격루가 해금되었습니다. 박물관에서 확인하세요!" }
    };

    private void Awake()
    {
        // 퀵슬롯 관련 데이터만 초기화
        foreach (var entry in inventionSlotMap)
        {
            PlayerPrefs.DeleteKey(entry.Key.ToString()); // 퀵슬롯 상태만 초기화
        }

        PlayerPrefs.Save(); // PlayerPrefs 초기화 후 저장
    }


    private void Start()
    {
        if (GlobalInventionManager.Instance == null)
        {
            Debug.LogError("GlobalInventionManager.Instance가 null입니다. 메인 씬에서 GlobalInventionManager가 생성되었는지 확인하세요.");
            return;
        }

        // PlayerPrefs에서 퀵슬롯 상태 복원
        RestoreQuickSlotStateFromPlayerPrefs();
    }

    private void RestoreQuickSlotStateFromPlayerPrefs()
    {
        foreach (var entry in inventionSlotMap)
        {
            InventionType inventionType = entry.Key;
            int slotIndex = entry.Value;

            // PlayerPrefs에 상태가 없으면 기본값을 before 상태로 설정
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
                // PlayerPrefs에 데이터가 없을 때는 기본적으로 before 상태로 설정
                AfterQuickSlots[slotIndex]?.SetActive(false);
                BeforeQuickSlots[slotIndex]?.SetActive(true);
            }
        }
    }


    public void ActivateAfterQuickSlot(InventionType inventionType)
    {
        if (inventionSlotMap.TryGetValue(inventionType, out int slotIndex))
        {
            if (slotIndex < BeforeQuickSlots.Length)
            {
                AfterQuickSlots[slotIndex].SetActive(true);
                BeforeQuickSlots[slotIndex].SetActive(false);

                // 발명품을 획득하면 PlayerPrefs에 상태 저장
                SaveQuickSlotStateWithPlayerPrefs(inventionType, true);
            }

            if (inventionMessages.TryGetValue(inventionType, out string message))
            {
                InventionReleasedText.text = message;
                StartCoroutine(HideInventionTextAfterDelay(2f));
            }
        }
    }


    public void SaveQuickSlotStateWithPlayerPrefs(InventionType inventionType, bool isActive)
    {
        // PlayerPrefs에 상태 저장
        PlayerPrefs.SetInt(inventionType.ToString(), isActive ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log($"PlayerPrefs에 {inventionType} 상태 저장됨: {isActive}");
    }

    private IEnumerator HideInventionTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        InventionReleasedText.text = "";
    }

    public void ResetQuickSlots()
    {
        foreach (var entry in inventionSlotMap)
        {
            int slotIndex = entry.Value;
            BeforeQuickSlots[slotIndex].SetActive(true);
            AfterQuickSlots[slotIndex].SetActive(false);
        }

        // 텍스트도 초기화
        InventionReleasedText.text = "";
    }
}
