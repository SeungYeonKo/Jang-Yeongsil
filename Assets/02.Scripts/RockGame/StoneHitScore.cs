using System;
using System.Collections;
using _02.Scripts.RockGame;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class StoneHitScore : MonoBehaviour
{
    private StoneScoreManager _stoneScoreManager;
    private string _playerName;
    private StoneFalldownCheck _stoneFalldownCheck;
    private UI_StoneManager _uiStoneManager;
    private float _timer;
    private float _spawnTimer = 15;
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    private GameObject _StonePrefeb;
    private StoneTimeAttack _stoneTimeAttack;
    private int _poolSize = 20;
    private void Start()
    {
        _StonePrefeb = this.gameObject;
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
        _stoneTimeAttack = FindObjectOfType<StoneTimeAttack>();
        _uiStoneManager = FindObjectOfType<UI_StoneManager>();
    }

    private void Update()
    {
        if (_stoneTimeAttack.TimesUP == 0)
        {
            return;
        }
        _timer += Time.deltaTime;
        if (_timer >= _spawnTimer)
        {
            Instantiate(_StonePrefeb, _originalPosition, _originalRotation);
            //PhotonNetwork.Instantiate("Stone", _originalPosition, _originalRotation);
            _timer = 0;
        }
    }

    // 비석이 플레이어에게 종속될 때 호출
    public void OnPickedUpByPlayer(Transform playerTransform)
    {
        // 플레이어의 PhotonView를 찾습니다.
        PhotonView playerPhotonView = playerTransform.GetComponentInParent<PhotonView>();

        if (playerPhotonView != null && playerPhotonView.IsMine)
        {
            // 플레이어의 닉네임을 저장합니다.
            _playerName = playerPhotonView.Owner.NickName;
            _stoneScoreManager = FindObjectOfType<StoneScoreManager>();
            Debug.Log("비석이 " + _playerName + " 플레이어에게 종속되었습니다.");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 비석이 충돌했을 때 점수 처리
        if (collision.gameObject.CompareTag("Pillar"))
        {
            if (_stoneScoreManager != null && !string.IsNullOrEmpty(_playerName))
            {
                if (_stoneTimeAttack.IsBounsTimeStart)
                {
                    _stoneScoreManager.AddScoreForPlayer(_playerName, 100);
                    Debug.Log("100점 추가");
                }
                else
                {
                    _stoneScoreManager.AddScoreForPlayer(_playerName, 30);
                    Debug.Log("30점 추가");
                }
            }

            StartCoroutine(ShowAfter2S(_uiStoneManager.GreatText.gameObject));
        }
        else
        {
            StartCoroutine(ShowAfter2S(_uiStoneManager.MissText.gameObject));
        }
    }

    IEnumerator ShowAfter2S(GameObject obj)
    {
        obj.SetActive(true);
        obj.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        yield return new WaitForSeconds(1f);
        obj.SetActive(false);
    }
}