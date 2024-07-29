using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class FireballController : MonoBehaviourPunCallbacks
{
    public PlayableDirector TimelineMaker;
    private string RoomID = "Main";
    private bool isLeavingRoom = false;

    void Start()
    {
        //StartCoroutine(Particle_Coroutine());
        TimelineMaker.Play();
        TimelineMaker.stopped += OnPlayableDirectorStopped;
    }
    IEnumerator Particle_Coroutine()
    {
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene("LoadingScene");
    }
    private void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        if(isLeavingRoom)
        {
            return;
        }

        if (TimelineMaker == aDirector)
        {
            LoadLoadingScene(); // 타임라인이 끝나면 빌리지 씬으로 이동
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
