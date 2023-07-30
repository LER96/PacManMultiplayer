using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TeamManagement : GameManager
{
    public const string Next_Round_RPC = nameof(NextRound);

    public void RestartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //add respawn all players
            //Invoke(nameof(NextRound), 2f);
            photonView.RPC(Next_Round_RPC, RpcTarget.AllViaServer);
        }
    }
}
