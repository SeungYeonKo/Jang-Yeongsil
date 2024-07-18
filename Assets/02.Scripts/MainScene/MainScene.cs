using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class MainScene : MonoBehaviourPunCallbacks
{
    public List<Transform> SpawnPoints;

    private bool localPlayerInitialized = false;


    void Start()
    {
        if (PhotonNetwork.InRoom && !localPlayerInitialized)
        {
            InitializePlayer(PhotonNetwork.LocalPlayer);
        }
    }

    private void InitializePlayer(Photon.Realtime.Player player)
    {
        Debug.Log("캐릭터 불러오기 로직");
        if (!player.IsLocal) return;
        Vector3 spawnPoint = GetRandomSpawnPoint();

        // 캐릭터 인덱스 값을 받아와서
        string userId = PlayerPrefs.GetString("LoggedInId", string.Empty);
        if (!string.IsNullOrEmpty(userId))
        {
            CharacterGender? gender = PersonalManager.Instance.ReloadGender(userId);
            if (gender != null)
            {
                Debug.Log($"Loaded Gender: {gender}");

                string prefabName = gender == CharacterGender.Male ? "Player_male" : "Player_female";

                // 캐릭터 생성
                Debug.Log($"{prefabName}");
                PhotonNetwork.Instantiate(prefabName, spawnPoint, Quaternion.identity);
            }
            else
            {
                Debug.Log("User not found or gender not set.");
            }
        }
        else
        {
            Debug.LogError("User ID is not set in PlayerPrefs");
        }

        localPlayerInitialized = true; // 로컬 플레이어가 생성되었음을 표시
    }

    public Vector3 GetRandomSpawnPoint()
    {
        int randomIndex = Random.Range(0, SpawnPoints.Count);
        return SpawnPoints[randomIndex].position;
    }


}