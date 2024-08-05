using UnityEngine;

public class InventionSpawnManager : MonoBehaviour
{
    public GameObject cheugugiPrefab; // 드랍할 발명품의 프리팹

    private void Start()
    {
        if (PlayerPrefs.HasKey("WinnerPlayerNumber"))
        {
            int winnerPlayerNumber = PlayerPrefs.GetInt("WinnerPlayerNumber");
            RainGaugePlayer winner = FindWinnerInScene(winnerPlayerNumber);

            if (winner != null)
            {
                DropCheugugi(winner.transform.position); // 승자의 위치에 발명품 드랍
            }

            PlayerPrefs.DeleteKey("WinnerPlayerNumber"); // 사용 후 저장된 정보 삭제
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

    private void DropCheugugi(Vector3 position)
    {
        if (cheugugiPrefab != null)
        {
            Debug.Log("발명품 오브젝트 드랍");
            Vector3 dropPosition = position + new Vector3(0, 1, 0); // 승자 앞에 발명품을 드랍
            Instantiate(cheugugiPrefab, dropPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Cheugugi prefab is not assigned.");
        }
    }
}
