using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    public PhotonView PhotonView { get; private set; }

    private void Awake()
    {
        PhotonView = GetComponent<PhotonView>();
        if (PhotonView.IsMine)
        {
            // 현재 씬이 SundialScene일 때 기능을 활성화
            CheckAndActivateFeatures();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }

    // 씬 이름을 확인하고 기능을 활성화하는 메서드
    private void CheckAndActivateFeatures()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "SundialScene")
        {
            // SundialScene에서만 동작해야 하는 기능을 활성화
            EnableSundialFeatures();
        }
        else
        {
            // SundialScene이 아닌 경우 기능을 비활성화하거나 유지하지 않음
            DisableSundialFeatures();
        }
    }

    // Sundial 관련 기능을 활성화하는 메서드
    private void EnableSundialFeatures()
    {
        // 여기에서 SundialScene에서만 활성화할 기능들을 설정합니다.
        Debug.Log("SundialScene: 기능 활성화됨");
        // 예: 특정 컴포넌트나 오브젝트를 활성화
    }

    // Sundial 관련 기능을 비활성화하는 메서드
    private void DisableSundialFeatures()
    {
        // 다른 씬에서는 SundialScene 기능을 비활성화하거나 실행하지 않도록 설정합니다.
        Debug.Log("SundialScene이 아님: 기능 비활성화됨");
        // 예: 특정 컴포넌트나 오브젝트를 비활성화
    }

    // 씬이 변경될 때 다시 체크
    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckAndActivateFeatures(); // 씬이 로드될 때마다 체크
    }
}
