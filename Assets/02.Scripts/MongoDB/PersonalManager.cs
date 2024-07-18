using MongoDB.Driver;
using MongoDB.Bson;
using Photon.Pun;
using System.Collections.Generic;
using ExitGames.Client.Photon;
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
        Debug.Log("몽고디비접속");
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
        var user = _personalCollection.Find(filter).FirstOrDefault();

        if (user != null)
        {
            _cachedUserName = user.Name;
            PlayerPrefs.SetString("CachedUserName", _cachedUserName);
        }

        return user;
    }
    // 몽고디비에 아이디, 비밀번호가 있는지 체크하는 조건문을 위한 함수
    public bool CheckUser(string nickname, string password)
    {
        var filter = Builders<Personal>.Filter.Eq("Name", nickname) & Builders<Personal>.Filter.Eq("Password", password);
        return _personalCollection.Find(filter).Any();
    }
    public void UpdateGender(string userId, CharacterGender gender)
    {
        var filter = Builders<Personal>.Filter.Eq("Name", userId);
        var update = Builders<Personal>.Update.Set("SelectCharacter", gender);
        _personalCollection.UpdateOne(filter, update);
        Debug.Log($"{gender}정보가 저장되었습니다.");
    }

    public CharacterGender? ReloadGender(string userId)
    {
        var filter = Builders<Personal>.Filter.Eq("Name", userId);
        var user = _personalCollection.Find(filter).FirstOrDefault();

        if (user != null)
        {
            return user.SelectCharacter;
        }
        else
        {
            return null; // 사용자 정보를 찾을 수 없는 경우
        }
    }

    public int ChoiceGender()
    {
        string name = GetCachedUserName();
        if (string.IsNullOrEmpty(name)) return -1;

        var filter = Builders<Personal>.Filter.Eq(p => p.Name, name);
        var user = _personalCollection.Find(filter).FirstOrDefault();
    
        // user가 null이 아니고, SelectCharacter 값이 유효한 경우 해당 값을 반환합니다.
        // 그렇지 않으면 -1을 반환합니다.
        return user != null ? (int)user.SelectCharacter : -1;
    }

    // 사용자 이름을 캐시에서 가져오는 메서드
    public string GetCachedUserName()
    {
        _cachedUserName = PlayerPrefs.GetString("CachedUserName", string.Empty);
        return _cachedUserName;
    }
}
