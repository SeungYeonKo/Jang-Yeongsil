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
    public void JoinList(string nickname, string password)
    {
        Personal personal = new Personal()
        {
            Name = nickname,
            Password = password,
        };
        _personalCollection.InsertOne(personal);
    }
    // 몽고디비에 아이디, 비밀번호가 있는지 체크하는 함수
    public Personal Login(string nickname, string password)
    {
        var filter = Builders<Personal>.Filter.Eq("Name", nickname) & Builders<Personal>.Filter.Eq("Password", password);
        return _personalCollection.Find(filter).FirstOrDefault();
    }
    // 몽고디비에 아이디, 비밀번호가 있는지 체크하는 조건문을 위한 함수
    public bool CheckUser(string nickname, string password)
    {
        var filter = Builders<Personal>.Filter.Eq("Name", nickname) & Builders<Personal>.Filter.Eq("Password", password);
        return _personalCollection.Find(filter).Any();
    }
}
