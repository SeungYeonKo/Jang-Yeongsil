using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;

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
    
    public GameObject WinImage;
    public GameObject LoseImage;
    
    private bool _isReadyFinished = false;
    private bool _isGoFinished = false;
    private Dictionary<int, RainGaugePlayer> players = new Dictionary<int, RainGaugePlayer>();

    
    private void Start()
    {
        ReadyImg.gameObject.SetActive(false);
        StartImg.gameObject.SetActive(false);
        ReadyButtonPressed.SetActive(false);
        WinImage.gameObject.SetActive(false);
        LoseImage.gameObject.SetActive(false);
        UpdatePlayerUI();
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
        UpdatePlayerUI();
        CheakReadyButton();
        if (RainGaugeManager.Instance.CurrentGameState == GameState.Over)
        {
            object winnersObj;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Winners", out winnersObj))
            {
                string[] winners = (string[])winnersObj;

                bool isWinner = winners.Contains(PhotonNetwork.LocalPlayer.NickName);

                if (isWinner)
                {
                    StartCoroutine(ShowImage_Coroutine(WinImage));
                }
                else
                {
                    StartCoroutine(ShowImage_Coroutine(LoseImage));
                }
            }
        }
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

    private void UpdatePlayerUI()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("PlayerNumber", out object playerNumberObj))
            {
                int playerNumber = (int)playerNumberObj;
                string playerName = player.NickName;
                int score = GetPlayerScore(playerNumber); // 플레이어의 점수를 가져옵니다.
                
                Debug.Log($"{playerName}");
                SetPlayerUI(playerNumber - 1, playerName, score);
            }
        }
    }

    private void SetPlayerUI(int index, string playerName, int playerScore)
    {
        switch (index)
        {
            case 1:
                PlayNameOne.text = playerName;
                NumberOne.text = playerScore.ToString();
                break;
            case 2:
                PlayNameTwo.text = playerName;
                NumberTwo.text = playerScore.ToString();
                break;
            case 3:
                PlayNameThree.text = playerName;
                NumberThree.text = playerScore.ToString();
                break;
            case 4:
                PlayNameFour.text = playerName;
                NumberFour.text = playerScore.ToString();
                break;
            default:
                Debug.LogError("Invalid player index.");
                break;
        }
    }

    private int GetPlayerScore(int playerNumber)
    {
        switch (playerNumber)
        {
            case 1: return JarScore.Instance.Player1score;
            case 2: return JarScore.Instance.Player2score;
            case 3: return JarScore.Instance.Player3score;
            case 4: return JarScore.Instance.Player4score;
            default: return 0;
        }
    }

    IEnumerator ShowImage_Coroutine(GameObject img)
    {
        yield return new WaitForSeconds(1f);
        img.SetActive(true);
    }
}
