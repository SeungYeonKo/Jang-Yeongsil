using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class UI_Login : MonoBehaviour
{
    public LobbyScene lobbyScene;
    public InputField ID_input;
    public InputField PW_input;
    public Toggle RememberToggle;


    public void Start()
    {
        // 0 : ID, PW 입력 창 팝업후 
        LoadLoginInfo();
        AutoLogin();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ResetIDandPW();
        }
    }

    // 로그인 정보를 PlayerPrefs에 저장해두고 있다면 자동으로 채워서 불러오기
    // 정보를 저장하는 경우 : A (로그인 정보 저장)
    private void LoadLoginInfo()
    {
        string loggedInUser = PlayerPrefs.GetString("LoggedInId", string.Empty);
        string loggedInPassword = PlayerPrefs.GetString("LoggedInPassword", string.Empty);
        ID_input.text = loggedInUser;
        PW_input.text = loggedInPassword;
    }
    // 재접속시 정보가 있다면 : C (자동 로그인)
    public void AutoLogin()
    {
        string loggedInUser = PlayerPrefs.GetString("LoggedInId", string.Empty);
        string loggedInPassword = PlayerPrefs.GetString("LoggedInPassword", string.Empty);
        Debug.Log($"{loggedInUser}");
        Debug.Log($"{loggedInPassword}");
        
        
        // 로그인 정보가 있다면
        if (!string.IsNullOrEmpty(loggedInUser) && !string.IsNullOrEmpty(loggedInPassword))
        {
            // 퍼스널매니저에 있던 로그인 함수를 불러옴
            var user = PersonalManager.Instance.Login(loggedInUser, loggedInPassword);
            if (user != null)
            {
                // 포톤네트워크의 닉네임은 아이디가 되고
                PhotonNetwork.NickName = loggedInUser;
                StartCoroutine(FadeOutLogin());
            }
            else
            {
                Debug.Log("자동 로그인 실패");
                // 로그인 창을 다시 띄움
                lobbyScene.LoginPopup.SetActive(true);
            }
        }
        else
        {
            Debug.Log("로그인 정보가 없습니다.");
            // 로그인 창을 다시 띄움
            lobbyScene.LoginPopup.SetActive(true);
        }
    }

    
    // 로그인 버튼을 누른다면
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
            // - 정보를 저장하는 경우 : A (로그인 정보 저장)
            RememberUserInfo(nickname, password);
        }
        else // 토글이 선택되지 않았다면 - 정보를 저장하지 않는 경우 : B (저장X)
        {
            // 해시테이블에 아이디와 비밀번호 저장 (세션 동안 유지를 위해)
            Hashtable loginInfo = new Hashtable
            {
                { "LoggedInId", nickname },
                { "LoggedInPassword", password }
            };

            PhotonNetwork.LocalPlayer.SetCustomProperties(loginInfo);
        }
        // 로그인 창 종료
        lobbyScene.LoginPopup.SetActive(false);
        lobbyScene.CharacterPopup.SetActive(true);
    }
    private void RememberUserInfo(string nickname, string password)
    {
        // 몽고디비에 새로 저장
        PersonalManager.Instance.JoinList(nickname, password);
        Debug.Log("서버에 로그인 정보를 저장했습니다.");
        /*// 몽고디비에서 아이디랑 비밀번호가 없다면
        if (!PersonalManager.Instance.CheckUser(nickname, password))
        {

        }*/

        // 로그인 정보 저장
        PlayerPrefs.SetString("LoggedInId", nickname);
        PlayerPrefs.SetString("LoggedInPassword", password);
        PlayerPrefs.Save();

        Debug.Log("새로운 정보로 로그인 되었습니다.");
    }
    
    // 로컬 상태 초기화 메서드
    void ResetIDandPW()
    {
        PlayerPrefs.DeleteKey("LoggedInId");
        PlayerPrefs.DeleteKey("LoggedInPassword");
        PlayerPrefs.Save();

        Debug.Log("로컬 상태 초기화됨");
    }
    IEnumerator FadeOutLogin()
    {
        yield return new WaitForSeconds(0.5f);
        AutoLogin();
        lobbyScene.LoginPopup.SetActive(false);
        lobbyScene.CharacterPopup.SetActive(true);
    }
}
