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
    private int maxScore = 50;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateJarScores()
    {
        if (Player1score < maxScore)
        {
            jar1Timer += Time.deltaTime;
            if (jar1Timer >= scoreIncreaseInterval)
            {
                Player1score++;
                jar1Timer = 0f;
            }
        }

        if (Player2score < maxScore)
        {
            jar2Timer += Time.deltaTime;
            if (jar2Timer >= scoreIncreaseInterval)
            {
                Player2score++;
                jar2Timer = 0f;
            }
        }

        if (Player3score < maxScore)
        {
            jar3Timer += Time.deltaTime;
            if (jar3Timer >= scoreIncreaseInterval)
            {
                Player3score++;
                jar3Timer = 0f;
            }
        }

        if (Player4score < maxScore)
        {
            jar4Timer += Time.deltaTime;
            if (jar4Timer >= scoreIncreaseInterval)
            {
                Player4score++;
                jar4Timer = 0f;
            }
        }
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

    public void DetermineWinner()
    {
        Dictionary<string, int> playerScores = new Dictionary<string, int>();

        foreach (var player in PhotonNetwork.PlayerList)
        {
            string playerName = player.NickName;
            int playerNumber = (int)player.CustomProperties["PlayerNumber"];

            switch (playerNumber)
            {
                case 1:
                    playerScores[playerName] = Player1score;
                    break;
                case 2:
                    playerScores[playerName] = Player2score;
                    break;
                case 3:
                    playerScores[playerName] = Player3score;
                    break;
                case 4:
                    playerScores[playerName] = Player4score;
                    break;
            }
        }

        int maxScore = playerScores.Values.Max();
        var winners = playerScores.Where(x => x.Value == maxScore).Select(x => x.Key).ToList();

        Debug.Log($"Winner(s) with {maxScore} water: {string.Join(", ", winners)}");

        foreach (var player in PhotonNetwork.PlayerList)
        {

            string playerName = player.NickName;

            if (PhotonNetwork.IsMasterClient && winners.Contains(playerName))
            {
                Hashtable firstPlayerName = new Hashtable { { "FirstPlayerName", playerName } };
                PhotonNetwork.CurrentRoom.SetCustomProperties(firstPlayerName);
                Debug.Log($"{firstPlayerName} 저장");
            }
        }
    }
}