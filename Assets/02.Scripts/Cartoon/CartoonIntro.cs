using System.Collections;
using System.Collections.Generic;
using CLDRPlurals;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CartoonIntro : MonoBehaviour
{
    public List<GameObject> CartoonImg;
    public GameObject SpaceInfo;
    private int _currentIndex = 0;

    private void Start()
    {
        foreach (var img in CartoonImg)
        {
            img.SetActive(false);
        }
        CartoonImg[0].SetActive(true);
        SoundManager.instance.StopBgm();
        SoundManager.instance.PlayBgm(SoundManager.Bgm.IntroFireball);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 현재 이미지가 리스트의 끝이 아닌 경우
            if (_currentIndex < CartoonImg.Count - 1)
            {
                // 현재 이미지를 비활성화
                CartoonImg[_currentIndex].SetActive(false);

                // 다음 이미지 활성화
                _currentIndex++;
                CartoonImg[_currentIndex].SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            LoadLoadingScene();
        }

        if (CartoonImg[CartoonImg.Count - 1].gameObject.activeSelf)
        {
            LoadLoadingScene();
            SpaceInfo.SetActive(false);
        }
    }
    private void LoadLoadingScene()
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 20,
            IsVisible = true,
            IsOpen = true,
            EmptyRoomTtl = 1000 * 20,
        };
        PhotonNetwork.JoinOrCreateRoom("Main", roomOptions, TypedLobby.Default);
        SceneManager.LoadScene("LoadingScene");
    }
}
