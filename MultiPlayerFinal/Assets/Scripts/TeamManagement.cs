using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TeamManagement : GameManager
{
    public const string Next_Round_RPC = nameof(NextRound);
    public const string Round_End_RPC = nameof(EndRound);

    public void RestartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //add respawn all players
            //Invoke(nameof(NextRound), 2f);
            photonView.RPC(Next_Round_RPC, RpcTarget.AllViaServer);
        }
    }

    public override void EatenPellets(EatingPellets pellets, string team)
    {
        pellets.gameObject.SetActive(false);

        SetTeamScore(pellets.score, team);

        if (!RemainingPellets() && PhotonNetwork.IsMasterClient)
        {
            //freeze all movements
            photonView.RPC(Round_End_RPC, RpcTarget.MasterClient);
        }
    }
}
