using MongoDB.Driver;
using MongoDB.Bson;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class PersonalManager : MonoBehaviour
{
    private List<Personal> _personal = new List<Personal>();
    public List<Personal> Personals => _personal;

    private IMongoCollection<Personal> _personalCollection;
    public static PersonalManager Instance { get; private set; }

    private string _cachedUserName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 시작하자마자 몽고디비 서버 접속
    public void Init()
    {
        string connectionString = "mongodb+srv://giroo27:giroo27@master.7avgald.mongodb.net/?retryWrites=true&w=majority&appName=Master";
        MongoClient mongoClient = new MongoClient(connectionString);
        IMongoDatabase db = mongoClient.GetDatabase("LoginsInfo");
        _personalCollection = db.GetCollection<Personal>("Login");
    }

}
