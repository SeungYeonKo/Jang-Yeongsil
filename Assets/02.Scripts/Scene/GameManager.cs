using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SceneType
{
    Gwanghwamun,
    MiniGame1,
    MiniGame2,
    MiniGame3,
}

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    private Dictionary<string, Personal> playerData = new Dictionary<string, Personal>();
    //private PlayerOptionAbility _localPlayerController;
    private TPSCamera tpsCamera;
    private GameObject _cameraRoot;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //FindLocalPlayer();
        tpsCamera = FindObjectOfType<TPSCamera>();
    }
}
