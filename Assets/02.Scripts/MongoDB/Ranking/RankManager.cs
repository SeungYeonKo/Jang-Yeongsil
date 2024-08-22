using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Driver;
using UnityEngine;

public class RankManager : MonoBehaviour
{
    private List<Rank> _ranks = new List<Rank>();
    public List<Rank> Ranks => _ranks;

    private IMongoCollection<Rank> _ranksCollection;

    private string _cachedUserName;

    void Start()
    {
        Init();
    }
    public void Init()
    {
        string connectionString = "mongodb+srv://giroo27:giroo27@master.7avgald.mongodb.net/?retryWrites=true&w=majority&appName=Master";
        MongoClient mongoClient = new MongoClient(connectionString);
        IMongoDatabase db = mongoClient.GetDatabase("Ranking");
        _ranksCollection = db.GetCollection<Rank>("Rank");
        Debug.Log("Ranking System on");
    }
    public void AddOrUpdateRanking(string playerName, int score)
    {
        if (CheckNickName(playerName))
        {
            if (IsNewHighScore(playerName, score))
            {
                UpdateRanking(playerName, score);
            }
        }
        else
        {
            AddPlayerToRanking(playerName, score);
        }
    }
    public void AddPlayerToRanking(string playerName, int score)
    {
        Rank rank = new Rank()
        {
            Name = playerName,
            Score = score,
            DateTime = DateTime.Now
        };
        _ranksCollection.InsertOne(rank);
    }
    public bool CheckNickName(string nickname)
    {
        var filter = Builders<Rank>.Filter.Eq("Name", nickname);
        return _ranksCollection.Find(filter).Any();
    }
    public bool IsNewHighScore(string playerName, int newScore)
    {
        var filter = Builders<Rank>.Filter.Eq("Name", playerName);
        var existingRank = _ranksCollection.Find(filter).FirstOrDefault();

        if (existingRank != null)
        {
            return newScore > existingRank.Score;
        }

        return true; // 플레이어가 없다면 새로운 최고 점수로 간주
    }
    public void UpdateRanking(string playerName, int score)
    {
        var filter = Builders<Rank>.Filter.Eq("Name", playerName);
        var update = Builders<Rank>.Update
            .Set("Score", score)
            .Set("DateTime", DateTime.Now);
        
        _ranksCollection.UpdateOne(filter, update);
    }
}
