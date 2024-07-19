using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RainGaugePlayer : MonoBehaviourPunCallbacks
{
    public bool isReady = false;
    public int MyNum;
    private bool _isFinished = false;
    private GameObject _startpoint;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name != "RainGauge")
        {
            this.enabled = false;
            return;
        }
        if (!photonView.IsMine) return;
    }

    private void Start()
    {
        MyNum = GetUniqueRandomNumber();
        PhotonNetwork.LocalPlayer.SetCustomProperties
            (new Hashtable { { "PlayerNumber", MyNum }, { "PlayerJarNumber", MyNum } });
        _startpoint = GameObject.Find($"Start{MyNum}");
        MoveStartPosition();
    }
    public void MoveStartPosition()
    {
        transform.position = _startpoint.transform.position;
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            SetReadyStateOnInput();
        }
        if (RainGaugeManager.Instance.CurrentGameState == GameState.Loading)
        {
            MoveStartPosition();
        }
        if (RainGaugeManager.Instance.CurrentGameState == GameState.Go
            || RainGaugeManager.Instance.CurrentGameState == GameState.Over)
        {
            switch (MyNum)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }
        }
    }

    private int GetUniqueRandomNumber()
    {
        int num = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log($"playernum = {num}");
        return num;
    }

    private void SetReadyStateOnInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            isReady = !isReady;
            UpdateReadyState(isReady);
        }
    }

    private void UpdateReadyState(bool readyState)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "IsReady_RainGauge", readyState } });
    }
}
