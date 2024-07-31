using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public Transform SpawnPoint;
    private bool _init = false;

    private void Start()
    {
        if (!_init)
        {
            Init(PhotonNetwork.LocalPlayer);
        }
    }

    private void Init(Photon.Realtime.Player player)
    {
        if (!player.IsLocal) { return; }
        _init = true;
        Vector3 spawnPoint = SpawnPoint.position; // SpawnPoint 위치로 설정
        Debug.Log($"스폰 위치: {spawnPoint}");

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
                GameObject playerObject = PhotonNetwork.Instantiate(prefabName, spawnPoint, Quaternion.identity);

                // 생성된 플레이어의 위치 확인
                Debug.Log($"생성된 플레이어 위치: {playerObject.transform.position}");
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
    }
}
