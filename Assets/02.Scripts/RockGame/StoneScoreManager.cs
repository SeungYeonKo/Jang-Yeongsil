using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class StoneScoreManager : MonoBehaviourPun
{
    private RankManager _rankManager;
    private Dictionary<string, int> playerScores = new Dictionary<string, int>();

    void Start()
    {
        _rankManager = FindObjectOfType<RankManager>();
    }

    public void AddScoreForPlayer(string playerName, int score)
    {
        if (playerScores.TryGetValue(playerName, out int currentScore))
        {
            playerScores[playerName] = currentScore + score;
        }
        else
        {
            playerScores[playerName] = score;
        }

        Debug.Log(playerName + "의 현재 점수: " + playerScores[playerName]);
    }

    public void SubmitScore(string playerName)
    {
        if (photonView.IsMine)
        {
            if (_rankManager != null && playerScores.TryGetValue(playerName, out int finalScore))
            {
                _rankManager.AddOrUpdateRanking(playerName, finalScore);
                Debug.Log(playerName + "의 점수가 제출되었습니다: " + finalScore);
            }
            else
            {
                Debug.LogWarning("RankManager를 찾을 수 없거나, 플레이어 점수를 찾을 수 없습니다.");
            }
        }
    }
}