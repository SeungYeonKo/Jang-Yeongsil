using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum SceneType
{
    Gwanghwamun,
    MiniGame1,
    MiniGame2,
    MiniGame3,
    MuseumScene,
}

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    private Dictionary<string, Personal> playerData = new Dictionary<string, Personal>();

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
        tpsCamera = FindObjectOfType<TPSCamera>();
    }

    GameObject FindCameraRoot(Transform playerTransform)
    {
        Transform cameraRootTransform = playerTransform.Find("CameraRoot");

        if (cameraRootTransform != null)
        {
            Debug.Log($"CameraRoot 오브젝트 찾음: {cameraRootTransform.name}");
            return cameraRootTransform.gameObject;
        }
        else
        {
            Debug.LogError("CameraRoot 오브젝트를 찾지 못했습니다.");
            return null;
        }
    }

    public void BackToGwanghwamun()
    {
        if (SceneManager.GetActiveScene().name != "Gwanghwamun")
        {
            if (photonView.IsMine)
            {
                PhotonManager.Instance.LeaveAndLoadRoom("Gwanghwamun");
            }
            else
            {
                Debug.LogError("권한 없는 플레이어가 방 전환을 시도하였습니다.");
            }
        }
        else
        {
            return;
        }
    }

    public void GameOver()
    {
        PhotonNetwork.LeaveRoom();

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
