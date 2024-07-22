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

    public int Player1score;
    public int Player2score;
    public int Player3score;
    public int Player4score;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        CountJarWater();
    }

    void CountJarWater()
    {
        Player1score = 0;
        Player2score = 0;
        Player3score = 0;
        Player4score = 0;


        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.StartsWith("Jar"))
            {

                if (obj.name == "Jar1")
                {
                    Player1score++;
                    break;
                }
                else if (obj.name == "Jar2")
                {
                    Player2score++;
                    break;
                }
                else if (obj.name == "Jar3")
                {
                    Player3score++;
                    break;
                }
                else if (obj.name == "Jar4")
                {
                    Player4score++;
                    break;
                }
            }

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

        string winner = playerScores.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        int maxScore = playerScores[winner];

        Debug.Log($"Winner is {winner} with {maxScore} water!");

        foreach (var player in PhotonNetwork.PlayerList)
        {

            string playerName = player.NickName;

            if (PhotonNetwork.IsMasterClient && playerName == winner)
            {
                Hashtable firstPlayerName = new Hashtable { { "FirstPlayerName", playerName } };
                PhotonNetwork.CurrentRoom.SetCustomProperties(firstPlayerName);
                Debug.Log($"{firstPlayerName} 저장");
            }
        }
    }
}