using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class UI_Login : MonoBehaviour
{
    public LobbyScene lobbyScene;
    public TMP_InputField ID_input;
    public TMP_InputField PW_input;
    public Toggle RememberToggle;

    public void Start()
    {
        // 0 : ID, PW 입력 창 팝업후 
        LoadLoginInfo();
        AutoLogin();
    }
    
    // 로그인 정보를 PlayerPrefs에 저장해두고 있다면 자동으로 채워서 불러오기
    private void LoadLoginInfo()
    {
        string loggedInUser = PlayerPrefs.GetString("LoggedInId", string.Empty);
        string loggedInPassword = PlayerPrefs.GetString("LoggedInPassword", string.Empty);
        ID_input.text = loggedInUser;
        PW_input.text = loggedInPassword;
    }
    
    private void AutoLogin()
    {
        string loggedInUser = PlayerPrefs.GetString("LoggedInId", string.Empty);
        string loggedInPassword = PlayerPrefs.GetString("LoggedInPassword", string.Empty);
        // 로그인 정보가 있다면
        if (!string.IsNullOrEmpty(loggedInUser) && !string.IsNullOrEmpty(loggedInPassword))
        {
            // 퍼스널매니저에 있던 로그인 함수를 불러옴
            var user = PersonalManager.Instance.Login(loggedInUser, loggedInPassword);
            if (user != null)
            {
                // 포톤네트워크의 닉네임은 아이디가 되고
                PhotonNetwork.NickName = loggedInUser;
            }
        }
    }

    
    
    public void OnClickLogin()
    {
        string nickname = ID_input.text;
        string password = PW_input.text;
        // 아이디나 비밀번호 입력란이 Null이라면
        if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(password))
        {
            Debug.Log("아이디, 비밀번호 둘 다 입력해주세요");
            return;
        }

        if (RememberToggle.isOn)
        {
            // 토글이 선택되었다면 해당 함수 불러오기
            RememberUserInfo(nickname, password);
        }
        else // 토글이 선택되지 않았다면 
        {
            Debug.Log("Login failed.");
        }
        lobbyScene.LoginPopup.SetActive(false);
    }
    private void RememberUserInfo(string nickname, string password)
    {
        // 몽고디비에서 아이디랑 비밀번호가 없다면
        if (!PersonalManager.Instance.CheckUser(nickname, password))
        {
            // 몽고디비에 새로 저장
            PersonalManager.Instance.JoinList(nickname, password);
            Debug.Log("New user registered.");
        }
        // 혹시 몰라서 써놓는 해시테이블로 아이디, 비밀번호 저장하는 방법
        Hashtable loginInfo = new Hashtable
        {
            { "LoggedInId", nickname },
            { "LoggedInPassword", password }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(loginInfo);

        Debug.Log("Login successful, user remembered.");
    }
}
