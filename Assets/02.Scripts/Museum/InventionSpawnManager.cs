using System.Collections;
using UnityEngine;

public class InventionSpawnManager : MonoBehaviour
{
    public GameObject cheugugiPrefab; // 드랍할 발명품의 프리팹 (비활성화된 상태로 씬에 존재)
    private bool hasDropped = false;  // 한 번 드랍했는지 여부

    private void Awake()
    {
        Debug.Log("InventionSpawnManager 불러오기 성공");
        DontDestroyOnLoad(this.gameObject); // 씬 전환 시 오브젝트 유지
    }

    private void Start()
    {
        Debug.Log("InventionSpawnManager 불러오기 성공");
        if (PlayerPrefs.HasKey("WinnerPlayerNumber") && !hasDropped)
        {
            int winnerPlayerNumber = PlayerPrefs.GetInt("WinnerPlayerNumber");
            RainGaugePlayer winner = FindWinnerInScene(winnerPlayerNumber);

            if (winner != null)
            {
                StartCoroutine(DropCheugugiAfterDelay(winner.transform.position)); // 2초 후에 드랍
            }

            PlayerPrefs.DeleteKey("WinnerPlayerNumber"); // 사용 후 저장된 정보 삭제
        }
        else
        {
            Debug.LogWarning("WinnerPlayerNumber not found or has already dropped");
        }
    }

    private RainGaugePlayer FindWinnerInScene(int playerNumber)
    {
        RainGaugePlayer[] allPlayers = FindObjectsOfType<RainGaugePlayer>();
        foreach (RainGaugePlayer player in allPlayers)
        {
            if (player.MyNum == playerNumber)
            {
                return player;
            }
        }
        return null;
    }

    private IEnumerator DropCheugugiAfterDelay(Vector3 position)
    {
        yield return new WaitForSeconds(2f); // 2초 대기

        if (!hasDropped) // 이미 드랍한 적이 없는 경우에만 드랍
        {
            DropCheugugi(position);
            hasDropped = true;
        }
    }

    private void DropCheugugi(Vector3 position)
    {
        if (cheugugiPrefab != null)
        {
            Debug.Log("발명품 오브젝트 활성화 및 위치 설정");
            Vector3 dropPosition = position + new Vector3(0, 1, 0); // 승자 앞에 발명품을 드랍

            // 위치를 승자 앞 위치로 이동
            cheugugiPrefab.transform.position = dropPosition;

            // 오브젝트 활성화
            cheugugiPrefab.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Cheugugi prefab is not assigned.");
        }
    }
}
