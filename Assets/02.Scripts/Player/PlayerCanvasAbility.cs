using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerCanvasAbility : PlayerAbility
{
    public Canvas PlayerCanvas;
    public TextMeshProUGUI NicknameTextUI;
    private Camera cachedMainCamera;

    private void Start()
    {
        // _owner와 photonView가 null인지 확인
        if (_owner == null || _owner.photonView == null)
        {
            Debug.LogError("_owner or _owner.photonView is null");
            return;
        }

        // Camera.main을 캐싱
        cachedMainCamera = Camera.main;

        // 커스텀 프로퍼티 없이 사용할 수 있는 코드
        SetNickname(_owner.photonView.Owner.NickName);
    }

    private void Update()
    {
        // 로컬 플레이어일 때만 실행
        if (_owner.photonView.IsMine)
        {
            // Camera.main 대신 캐시된 카메라 사용
            if (cachedMainCamera != null)
            {
                transform.forward = cachedMainCamera.transform.forward;
            }
            else
            {
                Debug.LogWarning("cachedMainCamera is null");
            }
        }
    }

    public void SetNickname(string nickname)
    {
        NicknameTextUI.text = nickname;
    }

    // 커스텀 프로퍼티는 사용하지 않으나 이렇게 사용해도 실행은 됨
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        Debug.Log("OnPlayerPropertiesUpdate called");

        if (changedProps == null)
        {
            Debug.LogError("changedProps is null");
            return;
        }

        if (targetPlayer == null)
        {
            Debug.LogError("targetPlayer is null");
            return;
        }

        if (_owner == null)
        {
            Debug.LogError("_owner is null");
            return;
        }

        if (_owner.photonView == null)
        {
            Debug.LogError("_owner.photonView is null");
            return;
        }

        if (changedProps.ContainsKey("Nickname"))
        {
            Debug.Log("changedProps contains 'Nickname'");
            if (targetPlayer == _owner.photonView.Owner)
            {
                Debug.Log("targetPlayer is _owner.photonView.Owner");
                SetNickname((string)changedProps["Nickname"]);
            }
            else
            {
                Debug.LogWarning("targetPlayer is not _owner.photonView.Owner");
            }
        }
    }
}
