using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // 버튼 사용을 위해 필요

public class FireballTrigger : MonoBehaviourPunCallbacks
{
    public GameObject SphereTrigger;
    public ParticleSystem Fireball;
    private bool _isTrigger = false;

    void Start()
    {
        // 로컬에서 저장된 _isTrigger 상태 불러오기
        _isTrigger = PlayerPrefs.GetInt("FireballTriggerState", 0) == 1;

        if (!_isTrigger)
        {
            SphereTrigger.SetActive(true);
            Fireball.gameObject.SetActive(false);
        }
        else
        {
            SphereTrigger.SetActive(false);
            Fireball.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ResetTriggerState();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PhotonView photonView = other.GetComponent<PhotonView>();
        if (other.CompareTag("Player") && photonView.IsMine)
        {
            _isTrigger = true; // 상태를 먼저 설정합니다.
            SphereTrigger.SetActive(false);
            Fireball.gameObject.SetActive(true);

            // 로컬에 상태 저장
            PlayerPrefs.SetInt("FireballTriggerState", _isTrigger ? 1 : 0);
            PlayerPrefs.Save();

            Debug.Log("씬 이동");
            SceneManager.LoadScene("IntroFireball");
        }
    }

    // 로컬 상태 초기화 메서드
    void ResetTriggerState()
    {
        PlayerPrefs.DeleteKey("FireballTriggerState");
        PlayerPrefs.Save();

        Debug.Log("로컬 상태 초기화됨");
    }
}