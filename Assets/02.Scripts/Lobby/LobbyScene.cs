using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyScene : MonoBehaviourPunCallbacks
{    
    public GameObject LoginPopup;
    public GameObject CharacterPopup;
    public UI_Login UILogin;

    private string RoomID;
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        StartCoroutine(Show_Coroutine());

        SoundManager.instance.StopBgm();
        SoundManager.instance.PlayBgm(SoundManager.Bgm.LobbyScene);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            LoadLoadingScene("Main");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            LoadLoadingScene("NewRainGauge");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LoadLoadingScene("MiniGame2");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            LoadLoadingScene("MuseumScene");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            LoadLoadingScene("Test");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            LoadLoadingScene("AstronomicalChartScene");
        }
    }

    IEnumerator Show_Coroutine()
    {
        LoginPopup.SetActive(false);
        yield return new WaitForSeconds(5f);
        LoginPopup.SetActive(true);
        UILogin.AutoLogin();
    }
    public void ShowCharacterSelectPanel()
    {
        CharacterPopup.SetActive(true);
    }
    
    private void LoadLoadingScene(string RoomID)
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 20,
            IsVisible = true,
            IsOpen = true,
            EmptyRoomTtl = 1000 * 20,
        };
        PhotonNetwork.JoinOrCreateRoom(RoomID, roomOptions, TypedLobby.Default);
        SceneManager.LoadScene("LoadingScene");
    }
}
