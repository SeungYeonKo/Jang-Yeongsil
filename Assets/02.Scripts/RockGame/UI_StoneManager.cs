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
    private StoneTimeAttack _stoneTimeAttack;
    private StoneScoreManager _stoneScoreManager;
    void Start()
    {
        _stoneTimeAttack = FindObjectOfType<StoneTimeAttack>();
        _stoneScoreManager = FindObjectOfType<StoneScoreManager>();
        PlayerName.text = PhotonNetwork.LocalPlayer.NickName;
        PlayerScore.text = "0";
        Warnning.SetActive(false);
        Bouns.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Timer.text = ((int)_stoneTimeAttack.TimesUP).ToString();
        string nickname = PhotonNetwork.LocalPlayer.NickName;
        Debug.Log("Nickname passed to GetCurrentScore: " + nickname);

        PlayerScore.text = _stoneScoreManager.GetCurrentScore(nickname).ToString();

        if (_stoneTimeAttack.IsWarnningStart)
        {
            Warnning.SetActive(true);
        }
        else if (_stoneTimeAttack.IsBounsTimeStart)
        {
            Bouns.SetActive(true);
        }
    }
}
