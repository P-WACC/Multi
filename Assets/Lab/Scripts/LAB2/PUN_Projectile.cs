using Photon.Pun;
using UnityEngine;
/// <summary>
/// This is the NETWORK ADAPTER for the Projectile.
/// behavior in a PUN2 online environment.
/// </summary>
[RequireComponent(typeof(Projectile))]
[RequireComponent(typeof(PhotonView))]
public class PUN_Projectile : MonoBehaviourPun
{
    // A reference to the core projectile logic script.
    private Projectile _projectileLogic;

    private void OnEnable()
    {
        if (!photonView.IsMine)
            return;
        // Start listening for the OnMagicBoxHit event from the core projectile logic.
        _projectileLogic.OnMagicBoxHit += HandleMagicBoxHit;
        _projectileLogic.OnPlayerHit += HandlePlayerHit;
    }

    /// Unsubscribes from events when this component is disabled.
    private void OnDisable()
    {
        if (!photonView.IsMine)
            return;
        // Stop listening for the event to avoid errors and memory leaks.
        _projectileLogic.OnMagicBoxHit -= HandleMagicBoxHit;
        _projectileLogic.OnPlayerHit -= HandlePlayerHit;
    }

    /// This method is called when the projectile's OnMagicBoxHit event is fired.
    private void HandleMagicBoxHit(GameObject boxObject)
    {
        // Authority Check: When the event is received, only the owner of the projectile
        // should be allowed to initiate a network action.
        if (photonView.IsMine)
        {
            // If we are the owner, find the network action script on the box.
            PUN_RPCsNetworkAction magicBox = boxObject.GetComponent<PUN_RPCsNetworkAction>();
            if (magicBox != null)
            {
                // Tell the box to start its color change process across the network.
                magicBox.InitiateColorChange();
            }
        }
    }

    /// This method is now the "Smart Delivery Driver".
    /// It checks the effectValue to decide which RPC to call.
    private void HandlePlayerHit(GameObject playerObject, int effectValue)
    {
        if (!photonView.IsMine) return;
        PhotonView targetPhotonView = playerObject.GetComponentInParent<PhotonView>();
        if (targetPhotonView == null) return;
        if (effectValue < 0)
        {
            // Negative value means DAMAGE
            targetPhotonView.RPC(nameof(PUN_PlayerHealth.RpcTakeDamage),
            targetPhotonView.Owner, effectValue);
        }
        else if (effectValue > 0)
        {
            // Positive value means HEALING
            targetPhotonView.RPC(nameof(PUN_PlayerHealth.RpcReceiveHeal),
            targetPhotonView.Owner, effectValue);
        }
    }

    private void Awake()
    {
        _projectileLogic = GetComponent<Projectile>();
    }
    private void Start()
    {
        if (!photonView.IsMine)
        {
            // By disabling the Projectile script
            _projectileLogic.enabled = false;
        }
    }
}