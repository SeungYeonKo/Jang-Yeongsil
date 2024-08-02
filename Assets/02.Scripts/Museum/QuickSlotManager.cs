using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class QuickSlotManager : MonoBehaviour
{
    public static QuickSlotManager Instance; // 싱글턴 인스턴스

    public GameObject[] BeforeQuickSlots;
    public GameObject[] AfterQuickSlots;

    public TextMeshProUGUI InventionReleasedText;

    private Dictionary<InventionType, bool> quickSlotState = new Dictionary<InventionType, bool>();

    // InventionType을 한글로 변환하기 위한 딕셔너리
    private Dictionary<InventionType, string> inventionNameTranslations = new Dictionary<InventionType, string>()
    {
        { InventionType.ArmillarySphere, "혼천의" },
        { InventionType.Sundial, "해시계" },
        { InventionType.Cheugugi, "측우기" },
        { InventionType.AstronomicalChart, "천문도" },
        { InventionType.Clepsydra, "자격루" }
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    private void Start()
    {
        // 슬롯 상태 초기화
        foreach (var invention in System.Enum.GetValues(typeof(InventionType)))
        {
            quickSlotState[(InventionType)invention] = false; // 초기에는 모두 비활성화
        }
        UpdateQuickSlotUI(); // 초기 UI 업데이트
    }

    public void ActivateAfterQuickSlot(InventionType inventionType)
    {
        if (quickSlotState.ContainsKey(inventionType))
        {
            quickSlotState[inventionType] = true;
        }
        UpdateQuickSlotUI();
    }

    private void UpdateQuickSlotUI()
    {
        foreach (var kvp in quickSlotState)
        {
            int index = (int)kvp.Key;
            if (index < BeforeQuickSlots.Length && index < AfterQuickSlots.Length)
            {
                BeforeQuickSlots[index].SetActive(!kvp.Value);
                AfterQuickSlots[index].SetActive(kvp.Value);
            }
        }
    }

    public void SetInventionText(InventionType inventionType)
    {
        if (quickSlotState[inventionType] && inventionNameTranslations.TryGetValue(inventionType, out string translatedName))
        {
            InventionReleasedText.text = $"{translatedName}이(가) 해금되었습니다. 박물관에서 확인하세요!";
            StartCoroutine(HideInventionTextAfterDelay(2f));
        }
    }

    private IEnumerator HideInventionTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        InventionReleasedText.text = ""; // 텍스트 비우기
    }
}
