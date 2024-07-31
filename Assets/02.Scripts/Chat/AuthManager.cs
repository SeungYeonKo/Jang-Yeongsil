using UnityEngine;
using System.IO;

public class AuthManager : MonoBehaviour
{
    [SerializeField] private string apiKey;
    [SerializeField] private string organization;
    [SerializeField] private string assistantId;

    public string GetApiKey()
    {
        return apiKey;
    }

    public string GetOrganization()
    {
        return organization;
    }

    public string GetAssistantId()
    {
        return assistantId;
    }

    void Start()
    {
        LoadAuthData();
    }

    void LoadAuthData()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "auth.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            var authData = JsonUtility.FromJson<AuthData>(json);
            apiKey = authData.api_key;
            organization = authData.organization;
            assistantId = authData.assistant_id;
        }
        else
        {
            Debug.LogError("auth.json file not found.");
        }
    }

    private class AuthData
    {
        public string api_key;
        public string organization;
        public string assistant_id;
    }
}
