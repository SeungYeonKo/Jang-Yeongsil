using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class StoneHitScore : MonoBehaviour
{
    private StoneScoreManager _stoneScoreManager;
    private string _playerName;
    private StoneFalldownCheck _stoneFalldownCheck;

    // 비석이 플레이어에게 종속될 때 호출
    public void OnPickedUpByPlayer(Transform playerTransform)
    {
        // 플레이어의 PhotonView를 찾습니다.
        PhotonView playerPhotonView = playerTransform.GetComponentInParent<PhotonView>();

        if (playerPhotonView != null && playerPhotonView.IsMine)
        {
            // 플레이어의 닉네임을 저장합니다.
            _playerName = playerPhotonView.Owner.NickName;
            _stoneScoreManager = FindObjectOfType<StoneScoreManager>();
            Debug.Log("비석이 " + _playerName + " 플레이어에게 종속되었습니다.");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 비석이 충돌했을 때 점수 처리
        if (collision.gameObject.CompareTag("Pillar"))
        {
            if (_stoneScoreManager != null && !string.IsNullOrEmpty(_playerName))
            {
                _stoneScoreManager.AddScoreForPlayer(_playerName, 30);
                Debug.Log(_playerName + "의 점수가 추가되었습니다.");
            }
        }
    }
}