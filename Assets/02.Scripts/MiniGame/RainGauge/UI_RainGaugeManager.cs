using System;
using System.Collections;
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

    private bool _isFinished = true;

    private void Start()
    {
        ReadyImg.gameObject.SetActive(false);
        StartImg.gameObject.SetActive(false);
        ReadyButtonPressed.SetActive(false);
    }

    void Update()
    {
        if (RainGaugeManager.Instance.CurrentGameState == GameState.Loading)
        {
            if (!_isFinished)
            {
                StartCoroutine(Show_Coroutine(ReadyImg));
                _isFinished = true;
            }
        }
        else if (RainGaugeManager.Instance.CurrentGameState == GameState.Go)
        {
            if (!_isFinished)
            {
                StartCoroutine(Show_Coroutine(StartImg));
                _isFinished = true;
            }
            UpdateScore();
        }

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

}
