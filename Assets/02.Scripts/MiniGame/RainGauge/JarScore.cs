using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class JarScore : MonoBehaviour
{
    public static JarScore Instance { get; private set; }

    public GameObject Jar1;
    public GameObject Jar2;
    public GameObject Jar3;
    public GameObject Jar4;

    public int Player1score = 0;
    public int Player2score = 0;
    public int Player3score = 0;
    public int Player4score = 0;

    private float jar1Timer;
    private float jar2Timer;
    private float jar3Timer;
    private float jar4Timer;

    private float scoreIncreaseInterval = 1f;
    private int maxScore = 10000;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateJarScores()
    {
        if (IsJarAssigned(1) && Player1score < maxScore)
        {
            jar1Timer += Time.deltaTime;
            if (jar1Timer >= scoreIncreaseInterval)
            {
                Player1score++;
                jar1Timer = 0f;
            }
        }

        if (IsJarAssigned(2) && Player2score < maxScore)
        {
            jar2Timer += Time.deltaTime;
            if (jar2Timer >= scoreIncreaseInterval)
            {
                Player2score++;
                jar2Timer = 0f;
            }
        }

        if (IsJarAssigned(3) && Player3score < maxScore)
        {
            jar3Timer += Time.deltaTime;
            if (jar3Timer >= scoreIncreaseInterval)
            {
                Player3score++;
                jar3Timer = 0f;
            }
        }

        if (IsJarAssigned(4) && Player4score < maxScore)
        {
            jar4Timer += Time.deltaTime;
            if (jar4Timer >= scoreIncreaseInterval)
            {
                Player4score++;
                jar4Timer = 0f;
            }
        }
    }

    private bool IsJarAssigned(int jarNumber)
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("PlayerNumber") && (int)player.CustomProperties["PlayerNumber"] == jarNumber)
            {
                return true;
            }
        }
        return false;
    }


    public void IncreaseScore(int jarNumber, int amount)
    {
        switch (jarNumber)
        {
            case 1:
                Player1score += amount;
                break;
            case 2:
                Player2score += amount;
                break;
            case 3:
                Player3score += amount;
                break;
            case 4:
                Player4score += amount;
                break;
        }
    }

    public void DecreaseScore(int jarNumber, int amount)
    {
        switch (jarNumber)
        {
            case 1:
                Player1score = Mathf.Max(0, Player1score - amount);
                break;
            case 2:
                Player2score = Mathf.Max(0, Player2score - amount);
                break;
            case 3:
                Player3score = Mathf.Max(0, Player3score - amount);
                break;
            case 4:
                Player4score = Mathf.Max(0, Player4score - amount);
                break;
        }
    }

    public void ResetScore(int jarNumber)
    {
        switch (jarNumber)
        {
            case 1:
                Player1score = 0;
                break;
            case 2:
                Player2score = 0;
                break;
            case 3:
                Player3score = 0;
                break;
            case 4:
                Player4score = 0;
                break;
        }
    }

    public void DetermineWinner()
    {
        StartCoroutine(DetermineWinnerWithDelay());
    }


    private IEnumerator DetermineWinnerWithDelay()
    {
        yield return new WaitForSeconds(1f);

        Dictionary<string, int> playerScores = new Dictionary<string, int>();

        foreach (var player in PhotonNetwork.PlayerList)
        {
            string playerName = player.NickName;
            if (!player.CustomProperties.ContainsKey("PlayerNumber"))
            {
                Debug.LogError($"Player {playerName} does not have a PlayerNumber property.");
                continue;
            }
            int playerNumber = (int)player.CustomProperties["PlayerNumber"];
            int playerScore = 0;

            switch (playerNumber)
            {
                case 1:
                    playerScore = Player1score;
                    break;
                case 2:
                    playerScore = Player2score;
                    break;
                case 3:
                    playerScore = Player3score;
                    break;
                case 4:
                    playerScore = Player4score;
                    break;
                default:
                    Debug.LogWarning($"Player {playerName} has an invalid player number: {playerNumber}");
                    continue;
            }

            playerScores[playerName] = playerScore;
        }
        if (playerScores.Count == 0)
        {
            Debug.LogError("No player scores found.");
            yield break;
        }

        int maxScore = playerScores.Values.Max();
        List<string> winners = playerScores.Where(x => x.Value == maxScore).Select(x => x.Key).ToList();

        Debug.Log($"Winner(s) with {maxScore} water: {string.Join(", ", winners)}");

        if (PhotonNetwork.IsMasterClient)
        {
            Hashtable winnersHashtable = new Hashtable { { "Winners", winners.ToArray() } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(winnersHashtable);
            Debug.Log($"{string.Join(", ", winners)} stored as winners.");
        }
    }
}