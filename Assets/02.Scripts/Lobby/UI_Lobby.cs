using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UI_Lobby : MonoBehaviour
{
    public LobbyScene lobbyScene;
    public TMP_InputField ID_input;
    public TMP_InputField PW_input;

    

    public void OnClickLogin()
    {
        lobbyScene.LoginPopup.SetActive(false);
    }
}
