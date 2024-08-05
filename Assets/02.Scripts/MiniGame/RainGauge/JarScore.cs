using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class JarScore : MonoBehaviourPunCallbacks
{
    public static JarScore Instance { get; private set; }

    public GameObject Jar1;
    public GameObject Jar2;
    public GameObject Jar3;
    public GameObject Jar4;

    private int player1score;
    private int player2score;
    private int player3score;
    private int player4score;

    private float jar1Timer;
    private float jar2Timer;
    private float jar3Timer;
    private float jar4Timer;

    private float scoreIncreaseInterval = 1f;
    private int maxScore = 10000;

    private int _winnerNumber = -1; //승자 정보 저장할 변수

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateJarScores()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (IsJarAssigned(1) && player1score < maxScore)
            {
                jar1Timer += Time.deltaTime;
                if (jar1Timer >= scoreIncreaseInterval)
                {
                    IncreaseScore(1, 1);
                    jar1Timer = 0f;
                }
            }
            if (IsJarAssigned(2) && player2score < maxScore)
            {
                jar2Timer += Time.deltaTime;
                if (jar2Timer >= scoreIncreaseInterval)
                {
                    IncreaseScore(2, 1);
                    jar2Timer = 0f;
                }
            }
            if (IsJarAssigned(3) && player3score < maxScore)
            {
                jar3Timer += Time.deltaTime;
                if (jar3Timer >= scoreIncreaseInterval)
                {
                    IncreaseScore(3, 1);
                    jar3Timer = 0f;
                }
            }
            if (IsJarAssigned(4) && player4score < maxScore)
            {
                jar4Timer += Time.deltaTime;
                if (jar4Timer >= scoreIncreaseInterval)
                {
                    IncreaseScore(4, 1);
                    jar4Timer = 0f;
                }
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


    [PunRPC]
    public void IncreaseScore(int playerNumber, int amount)
    {
        switch (playerNumber)
        {
            case 1: player1score += amount; break;
            case 2: player2score += amount; break;
            case 3: player3score += amount; break;
            case 4: player4score += amount; break;
        }
        UpdateScore(playerNumber);
    }

    [PunRPC]
    public void ResetScore(int playerNumber)
    {
        switch (playerNumber)
        {
            case 1: player1score = 0; break;
            case 2: player2score = 0; break;
            case 3: player3score = 0; break;
            case 4: player4score = 0; break;
        }
        UpdateScore(playerNumber);
    }

    private void UpdateScore(int playerNumber)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Hashtable scores = new Hashtable();
            scores[$"Player{playerNumber}score"] = GetPlayerScore(playerNumber);
            PhotonNetwork.CurrentRoom.SetCustomProperties(scores);
        }
    }

    private int GetPlayerScore(int playerNumber)
    {
        switch (playerNumber)
        {
            case 1: return player1score;
            case 2: return player2score;
            case 3: return player3score;
            case 4: return player4score;
            default: return 0;
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("Player1score"))
        {
            player1score = (int)propertiesThatChanged["Player1score"];
        }
        if (propertiesThatChanged.ContainsKey("Player2score"))
        {
            player2score = (int)propertiesThatChanged["Player2score"];
        }
        if (propertiesThatChanged.ContainsKey("Player3score"))
        {
            player3score = (int)propertiesThatChanged["Player3score"];
        }
        if (propertiesThatChanged.ContainsKey("Player4score"))
        {
            player4score = (int)propertiesThatChanged["Player4score"];
        }
    }


    public int DetermineWinner()
    {
        StartCoroutine(DetermineWinnerWithDelay());
        return _winnerNumber; // DetermineWinnerWithDelay에서 설정된 winnerNumber 반환
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
            int playerScore = GetPlayerScore(playerNumber);

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