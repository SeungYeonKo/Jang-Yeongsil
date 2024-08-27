using System.Collections;
using System.Collections.Generic;
using _02.Scripts.RockGame;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_StoneManager : MonoBehaviourPun
{
    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI PlayerScore;
    public TextMeshProUGUI Timer;
    public GameObject Warnning;
    public GameObject Bouns;
    public GameObject StartImg;
    public GameObject GameOverImg;
    public Slider TimeSlider;
    public GameObject Female;
    public GameObject Male;
    public Image FillImage; 
    private StoneTimeAttack _stoneTimeAttack;
    private StoneScoreManager _stoneScoreManager;
    private StoneGameManager _stoneGameManager;
    private bool _isBlinkEnd = true;
    private bool _isShow = true;
    void Start()
    {
        _stoneGameManager = FindObjectOfType<StoneGameManager>();
        _stoneTimeAttack = FindObjectOfType<StoneTimeAttack>();
        _stoneScoreManager = FindObjectOfType<StoneScoreManager>();
        string nickname = PhotonNetwork.LocalPlayer.NickName;
        PlayerName.text = nickname;
        Warnning.SetActive(false);
        Bouns.SetActive(false);
        TimeSlider.maxValue = _stoneTimeAttack.TimesUP; 
        CharacterGender? gender = PersonalManager.Instance.ReloadGender(nickname);
        if (gender == CharacterGender.Male)
        {
            Male.SetActive(true);
            Female.SetActive(false);
        }
        else
        {
            Female.SetActive(true);
            Male.SetActive(false);
        }
    }

    void Update()
    {
        Timer.text = ((int)_stoneTimeAttack.TimesUP).ToString();
        string nickname = PhotonNetwork.LocalPlayer.NickName;
        PlayerScore.text = _stoneScoreManager.GetCurrentScore(nickname).ToString();
        TimeSlider.value = _stoneTimeAttack.TimesUP;
        
        HandleGameStateUI();
        HandleTimeRelatedUI();
        if (_stoneGameManager.CurrentState == StoneGameState.GameOver)
        {
            PlayerScore.text = _stoneScoreManager.GetFinalScore(nickname).ToString();
        }
    }

    void HandleGameStateUI()
    {
        if (_stoneGameManager.CurrentState == StoneGameState.Start && _isShow)
        {
            StartCoroutine(ShowState(StartImg));
            _isShow = false;
            GameOverImg.SetActive(false);
        }
        else if (_stoneGameManager.CurrentState == StoneGameState.Go)
        {
            StartImg.SetActive(false);
            GameOverImg.SetActive(false);
            _isShow = true;
        }
        else if (_stoneGameManager.CurrentState == StoneGameState.GameOver && _isShow)
        {
            StartCoroutine(ShowState(GameOverImg));
            _isShow = false;
        }
    }

    void HandleTimeRelatedUI()
    {
        if (_stoneTimeAttack.IsWarnningStart)
        {
            if (_isBlinkEnd)
            {
                StartCoroutine(Blink(Warnning, 2f));
                _isBlinkEnd = false;
            }
            FillImage.color = Color.red;
        }
        else if (_stoneTimeAttack.IsBounsTimeStart)
        {
            if (_isBlinkEnd)
            {
                StartCoroutine(Blink(Bouns, 2f));
                _isBlinkEnd = false;
            }
            FillImage.color = Color.yellow;
        }
    }

    IEnumerator Blink(GameObject obj, float duration)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            obj.SetActive(true); // 오브젝트 활성화
            yield return new WaitForSeconds(0.5f); // 0.5초 대기

            obj.SetActive(false); // 오브젝트 비활성화
            yield return new WaitForSeconds(0.5f); // 0.5초 대기

            elapsedTime += 1f; // 0.5초 활성화 + 0.5초 비활성화로 1초 증가
        }

        // 코루틴 종료 후 오브젝트를 완전히 비활성화
        obj.SetActive(false);
        _isBlinkEnd = false;
    }

    IEnumerator ShowState(GameObject obj)
    {
        obj.SetActive(true);
        yield return new WaitForSeconds(1f);
        obj.SetActive(false);
    }
}
