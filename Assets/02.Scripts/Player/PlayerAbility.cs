using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerAbility : MonoBehaviourPunCallbacks
{
    protected Player _owner { get; private set; }
    protected virtual void Awake()
    {
        _owner = GetComponentInParent<Player>();
    }
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        // 이 메서드는 PlayerCanvasAbility에서 오버라이드 됩니다.
    }
}
