using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : MonoBehaviour
{    
    public GameObject LoginPopup;
    public GameObject CharacterPopup;
    public UI_Login UILogin;
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        StartCoroutine(Show_Coroutine());
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
}
