using UnityEngine;

public class InventionSpawnManager : MonoBehaviour
{
    public GameObject cheugugiPrefab; // 드랍할 발명품의 프리팹

    private void Start()
    {
        // 이전 씬에서 승자 정보를 가져옴
        if (PlayerPrefs.HasKey("WinnerPlayerNumber"))
        {
            int winnerPlayerNumber = PlayerPrefs.GetInt("WinnerPlayerNumber");
            RainGaugePlayer winner = FindWinnerInScene(winnerPlayerNumber);

            if (winner != null)
            {
                DropCheugugi(winner.transform.position); // 승자의 위치에 Cheugugi 드랍
            }

            // 사용한 정보 삭제
            PlayerPrefs.DeleteKey("WinnerPlayerNumber");
        }
    }

    private RainGaugePlayer FindWinnerInScene(int playerNumber)
    {
        // 메인 씬에서 승자를 찾는 로직 구현 (필요시 조정)
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

    private void DropCheugugi(Vector3 position)
    {
        if (cheugugiPrefab != null)
        {
            Vector3 dropPosition = position + new Vector3(0, 1, 0); // 승자 앞에 Cheugugi를 드랍
            Instantiate(cheugugiPrefab, dropPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Cheugugi prefab is not assigned.");
        }
    }
}
