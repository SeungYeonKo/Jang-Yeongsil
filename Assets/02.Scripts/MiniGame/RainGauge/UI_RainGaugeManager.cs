using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;
using UnityEngine.UI;

public class UI_RainGaugeManager : MonoBehaviourPunCallbacks
{
    public GameObject ReadyImg;
    public GameObject StartImg;
    public GameObject ReadyButtonPressed;
    public GameObject ReadyButton;

    public TMP_Text[] NumberTexts;
    public TMP_Text[] PlayerNameTexts;

    public GameObject[] PlayerPanels;

    public TMP_Text TimerText;

    public GameObject WinImage;
    public GameObject LoseImage;
    
    private bool _isReadyFinished = false;
    private bool _isGoFinished = false;

    public Button readyButton;
    private RainGaugePlayer player;

    private void Start()
    {
        ReadyImg.gameObject.SetActive(false);
        StartImg.gameObject.SetActive(false);
        ReadyButtonPressed.SetActive(false);
        WinImage.gameObject.SetActive(false);
        LoseImage.gameObject.SetActive(false);
        InitializePlayerUI();

        readyButton.onClick.AddListener(OnReadyButtonClick);
        player = FindObjectOfType<RainGaugePlayer>();
    }

    private void OnReadyButtonClick()
    {
        if (player != null)
        {
            bool newReadyState = !player.isReady; 
            player.SetReadyState(newReadyState);
        }
    }

    void Update()
    {
        if (RainGaugeManager.Instance != null)
        {
            if (RainGaugeManager.Instance.CurrentGameState == GameState.Loading)
            {
                if (!_isReadyFinished)
                {
                    _isReadyFinished = true;
                    _isGoFinished = false;
                }
                ReadyButtonPressed.gameObject.SetActive(false);
                ReadyButton.gameObject.SetActive(false);
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
            UpdateTimerUI();

            if (RainGaugeManager.Instance.CurrentGameState == GameState.Over)
            {
                if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties != null)
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
        }
         
    }

    private void UpdateTimerUI()
    {
        if (RainGaugeManager.Instance != null)
        {
            TimerText.text = $"{(int)RainGaugeManager.Instance.TimeRemaining}";
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
            SetReadyImageState(isReadyValue);
        }
    }
    private void InitializePlayerUI()
    {
        foreach (var text in NumberTexts)
        {
            text.text = "0";
        }

        foreach (var text in PlayerNameTexts)
        {
            text.text = "Waiting...";
        }
        foreach (var panel in PlayerPanels)
        {
            panel.SetActive(false); 
        }
    }

    private void UpdatePlayerUI()
    {
        int playerIndex = 0;
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("PlayerNumber", out object playerNumberObj))
            {
                int playerNumber = (int)playerNumberObj;
                string playerName = player.NickName;
                int score = GetPlayerScoreFromRoom(playerNumber); // 플레이어의 점수를 가져옴

                if (playerIndex < PlayerNameTexts.Length && playerIndex < NumberTexts.Length)
                {
                    PlayerNameTexts[playerIndex].text = playerName;
                    NumberTexts[playerIndex].text = score.ToString();
                    PlayerPanels[playerIndex].SetActive(true);
                    playerIndex++;
                }
            }
        }

        // 나머지 빈 UI를 초기화
        for (int i = playerIndex; i < PlayerNameTexts.Length; i++)
        {
            PlayerNameTexts[i].text = "Waiting...";
            NumberTexts[i].text = "0";
            PlayerPanels[i].SetActive(false);
        }
    }



    private int GetPlayerScoreFromRoom(int playerNumber)
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue($"Player{playerNumber}score", out object score))
        {
            return (int)score;
        }
        return 0;
    }

    IEnumerator ShowImage_Coroutine(GameObject img)
    {
        yield return new WaitForSeconds(1f);
        img.SetActive(true);
    }

    // 플레이어가 방에 입장할 때 호출되는 메서드
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        UpdatePlayerUI();
    }

    // 플레이어가 방에서 나갈 때 호출되는 메서드
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        UpdatePlayerUI();
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
        UpdatePlayerUI();
    }

    public void SetReadyImageState(bool state)
    {
        if (ReadyImg != null)
        {
            ReadyImg.SetActive(state);
        }
    }
}
