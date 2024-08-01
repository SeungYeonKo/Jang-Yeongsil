using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class UI_RainGaugeManager : MonoBehaviourPunCallbacks
{
    public GameObject ReadyImg;
    public GameObject StartImg;
    public GameObject ReadyButtonPressed;
    public GameObject ReadyButton;

    public TMP_Text NumberOne;
    public TMP_Text NumberTwo;
    public TMP_Text NumberThree;
    public TMP_Text NumberFour;

    public TMP_Text PlayNameOne;
    public TMP_Text PlayNameTwo;
    public TMP_Text PlayNameThree;
    public TMP_Text PlayNameFour;
    
    private bool _isReadyFinished = false;
    private bool _isGoFinished = false;
    private Dictionary<int, RainGaugePlayer> players = new Dictionary<int, RainGaugePlayer>();
    private void Start()
    {
        ReadyImg.gameObject.SetActive(false);
        StartImg.gameObject.SetActive(false);
        ReadyButtonPressed.SetActive(false);

        UpdateScore();
        UpdatePlayerNames();
    }

    void Update()
    {
        if (RainGaugeManager.Instance.CurrentGameState == GameState.Loading)
        {
            if (!_isReadyFinished)
            {
                StartCoroutine(Show_Coroutine(ReadyImg));
                _isReadyFinished = true;
                _isGoFinished = false;
            }
        }
        else if (RainGaugeManager.Instance.CurrentGameState == GameState.Go)
        {
            if (!_isGoFinished)
            {
                StartCoroutine(Show_Coroutine(StartImg));
                _isGoFinished = true;
                _isReadyFinished = false;
            }
        }
        UpdateScore();
        CheakReadyButton();
    }

    private IEnumerator Show_Coroutine(GameObject obj)
    {
        obj.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        obj.gameObject.SetActive(false);
    }

    void CheakReadyButton()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("IsReady_RainGauge", out object isReady))
        {
            bool isReadyValue = (bool)isReady;
            //Debug.Log("IsReady: " + isReadyValue);
            ReadyButtonPressed.gameObject.SetActive(isReadyValue);
            ReadyButton.gameObject.SetActive(!isReadyValue);
        }
    }

    private void UpdateScore()
    {
        NumberOne.text = JarScore.Instance.Player1score.ToString();
        NumberTwo.text = JarScore.Instance.Player2score.ToString();
        NumberThree.text = JarScore.Instance.Player3score.ToString();
        NumberFour.text = JarScore.Instance.Player4score.ToString();
    }
    
    private void UpdatePlayerNames()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("PlayerNumber", out object playerNumberObj))
            {
                int playerNumber = (int)playerNumberObj;
                string playerName = player.NickName;

                SetPlayerName(playerNumber, playerName);
            }
        }
    }

    private void SetPlayerName(int playerNumber, string playerName)
    {
        switch (playerNumber)
        {
            case 1:
                PlayNameOne.text = playerName;
                break;
            case 2:
                PlayNameTwo.text = playerName;
                break;
            case 3:
                PlayNameThree.text = playerName;
                break;
            case 4:
                PlayNameFour.text = playerName;
                break;
            default:
                Debug.LogError("Invalid player number.");
                break;
        }
    }
}
