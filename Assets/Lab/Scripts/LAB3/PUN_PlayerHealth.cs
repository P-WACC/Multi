using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PlayerHealth))]

public class PUN_PlayerHealth : MonoBehaviourPun, IPunObservable
{
    private PlayerHealth _playerHealth;
    private void Awake()
    {
        // Get the components we need to control
        _playerHealth = GetComponent<PlayerHealth>();
    }

    /// This RPC is called by the projectile's owner and executed on
    [PunRPC]
    public void RpcTakeDamage(int amount)
    {
        if (photonView.IsMine)
        {
            Debug.Log("Take Damage : " + amount);
            _playerHealth.TakeDamage(amount);
        }
    }

    [PunRPC]
    public void RpcReceiveHeal(int amount)
    {
        if (photonView.IsMine)
        {
            Debug.Log("Receive Heal : " + amount);
            _playerHealth.RecceiveHeal(amount);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We are the owner. Read health from our core component and send it.
            stream.SendNext(_playerHealth.CurrentHealth);
        }
        else
        {
            // We are a remote client. Receive the health data.
            int receivedHealth = (int)stream.ReceiveNext();

            // Set the health on our core component.
            _playerHealth.SetHealth(receivedHealth);
        }
    }
}
