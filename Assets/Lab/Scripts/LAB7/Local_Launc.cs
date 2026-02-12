using Photon.Pun;
using UnityEngine;
public class Local_Launch : PUN_ConnectionBase
{
    [Header("Level Transition Settings")]
    public string TargetSceneName = "GameScene"; // Name of the scene to load
    public override void SpawnGameplayObjects()
    {
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("We load the 'Room for 1' ");
            PhotonNetwork.LoadLevel(TargetSceneName);
        }
    }
}