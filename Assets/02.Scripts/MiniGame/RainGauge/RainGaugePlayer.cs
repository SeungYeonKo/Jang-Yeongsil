using Photon.Pun;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RainGaugePlayer : MonoBehaviourPunCallbacks
{
    public bool isReady = false;
    public int MyNum;
    private JarScore _jarScore;
    private bool _isFinished = false;
    private Jar _jarController;
    private GameObject _startpoint;

    public Transform leftHandTransform; 
    public Transform rightHandTransform;

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
        AssignPlayerNumber();
        PhotonNetwork.LocalPlayer.SetCustomProperties
            (new Hashtable { { "PlayerNumber", MyNum }, { "PlayerJarNumber", MyNum } });
        
        _startpoint = GameObject.Find($"Start{MyNum}");
        if (_startpoint != null)
        {
            Debug.Log($"Start point found for player {MyNum}: {_startpoint.transform.position}");
        }
        else
        {
            Debug.LogError($"Start point not found for player {MyNum}");
        }

        MoveStartPosition();
        _jarController = FindObjectOfType<Jar>();
        if (_jarController == null)
        {
           // Debug.LogError("JarController is not found. Make sure there is a Jar object in the scene.");
        }
    }

    private void AssignPlayerNumber()
    {
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("PlayerNumber"))
        {
            int assignedNumber = PhotonNetwork.LocalPlayer.ActorNumber % 4 + 1;
            MyNum = assignedNumber;
            Debug.Log($"Assigning PlayerNumber: {MyNum} to player: {PhotonNetwork.LocalPlayer.NickName}");
            
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "PlayerNumber", MyNum } });
        }
        else
        {
            MyNum = (int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerNumber"];
        }
    }

    public void MoveStartPosition()
    {
        if (photonView.IsMine)
        {
            if (_startpoint != null)
            {
                transform.position = _startpoint.transform.position;
                Debug.Log($"Player {MyNum} moved to start position: {transform.position}");
            }
            else
            {
                Debug.LogError($"Start point not found for player {MyNum}, cannot move to start position.");
            }
        }
          
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            SetReadyStateOnInput();
            Vector3 handPosition = GetHandPosition(MyNum);
            photonView.RPC("RPC_SetJarPosition", RpcTarget.AllBuffered, MyNum, handPosition);

            if (RainGaugeManager.Instance.CurrentGameState == GameState.Loading)
            {
                MoveStartPosition();
            }
        }
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

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("FirstPlayerNames"))
        {
            string[] firstPlayerNames = (string[])PhotonNetwork.CurrentRoom.CustomProperties["FirstPlayerNames"];
            if (firstPlayerNames != null)
            {
                if (RainGaugeManager.Instance.CurrentGameState == GameState.Over)
                {
                    if (!_isFinished)
                    {
                        Animator animator = GetComponent<Animator>();
                        if (photonView.IsMine)
                        {
                            StartCoroutine(PlayWinOrSadAnimation(animator, firstPlayerNames));
                        }
                        _isFinished = true;
                    }
                }
            }
        }
    }

    private IEnumerator PlayWinOrSadAnimation(Animator animator, string[] firstPlayerNames)
    {
        yield return new WaitForSeconds(1f); 

        if (firstPlayerNames.Contains(photonView.Owner.NickName))
        {

            //UI_GameOver.Instance.CheckFirst();
            animator.SetBool("Win", true);
        }
        else
        {
            //UI_GameOver.Instance.CheckLast();
            animator.SetBool("Sad", true);
        }
    }

    public Vector3 GetHandPosition(int playerNumber)
    {
        return rightHandTransform.position;
    }

    [PunRPC]
    private void RPC_SetJarPosition(int playerNumber, Vector3 position)
    {
        if (_jarController != null)
        {
            _jarController.SetJarPosition(playerNumber, position);
        }
        else
        {
           // Debug.LogError("JarController is not assigned.");
        }
    }
}
