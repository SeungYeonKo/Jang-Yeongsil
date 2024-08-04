using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_OptionAbility : MonoBehaviourPunCallbacks
{
   public static UI_OptionAbility Instance { get; private set; }

   public GameObject OptionUI;
   
   private PlayerOptionAbility _playerOptionAbility;
   
   void Awake()
   {
      if (Instance == null)
      {
         Instance = this;
         DontDestroyOnLoad(gameObject);
      }
      else
      {
         Destroy(gameObject);
      }
   }

   private void Start()
   {
      OptionUI.gameObject.SetActive(false);

      if (photonView.IsMine)
      {
         _playerOptionAbility = FindObjectOfType<PlayerOptionAbility>();
      }
   }
   private void Update()
   {      
      if (Input.GetKeyDown(KeyCode.Escape))
      {
          ToggleUI();
      }
   }
   private void ToggleUI()
   {
      bool isActive = !OptionUI.gameObject.activeSelf;
      OptionUI.gameObject.SetActive(isActive);
      UnityEngine.Cursor.visible = isActive;
      UnityEngine.Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
      if (photonView.IsMine)
      {
         if (isActive)
         {
            _playerOptionAbility.Pause();
         }
         else
         {
            _playerOptionAbility.Continue();
         }
      }
   }
   public void OnClickXbutton()
   {
      OptionUI.gameObject.SetActive(false);
      UnityEngine.Cursor.visible = false;
      UnityEngine.Cursor.lockState = CursorLockMode.Locked;
      _playerOptionAbility.Continue();
   }
   public void OnClickReplay()
   {
      OptionUI.gameObject.SetActive(false);
      UnityEngine.Cursor.visible = false;
      UnityEngine.Cursor.lockState = CursorLockMode.Locked;
      _playerOptionAbility.Continue();
   }
   
   public void OnClickSquare()
   {
      OptionUI.gameObject.SetActive(false);
      UnityEngine.Cursor.visible = false;
      UnityEngine.Cursor.lockState = CursorLockMode.Locked;
      if (SceneManager.GetActiveScene().name == "MainScene")
      {
         OnClickReplay();
      }
      else
      {
         PhotonNetwork.LeaveRoom();
         PhotonNetwork.LoadLevel("MainScene");
      }
   }
   
   public void OnClickGameQuitButton()
   {
      OptionUI.gameObject.SetActive(false);
      UnityEngine.Cursor.visible = false;
      UnityEngine.Cursor.lockState = CursorLockMode.Locked;
      if (_playerOptionAbility!= null)
      {
         // Photon Network에서 방 나가기
         PhotonNetwork.LeaveRoom();
      }

      // 빌드 후 실행됐을 경우 종료하는 방법
      Application.Quit();

#if UNITY_EDITOR
      // 유니티 에디터에서 실행했을 경우 종료하는 방법
      UnityEditor.EditorApplication.isPlaying = false;
#endif
   }
   
}
