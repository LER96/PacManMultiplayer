using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class UIManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TextMeshProUGUI teamPacmanScore;
    [SerializeField] TextMeshProUGUI teamMissPacmanScore;
   // private const string UPDATE_SCORE_UI = nameof(UpdateScoreUI);

    void Update()
    {
       // if (PhotonNetwork.IsMasterClient)
       // {
       //     photonView.RPC(UPDATE_SCORE_UI, RpcTarget.AllViaServer);
       // }

        UpdateTeamScores();
    }

   // [PunRPC]
   // public void UpdateScoreUI()
   // {
   //     int pacmanScore = GameManager.instance.teamPmScore;
   //     teamPacmanScore.text = $"Team Pacman Score: {pacmanScore}";
   //
   //     int missPacmanScore = GameManager.instance.teamMsPmScore;
   //     teamMissPacmanScore.text = $"Team Miss Pacman Score: {missPacmanScore}";
   // }

    public void UpdateTeamScores()
    {
        ExitGames.Client.Photon.Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

        if (roomProperties.ContainsKey("PacmanScore"))
        {
            int pacmanScore = (int)roomProperties["PacmanScore"];
            teamPacmanScore.text = $"Team Pacman Score: {pacmanScore}";
        }

        if (roomProperties.ContainsKey("MissPacmanScore"))
        {
            int missPacmanScore = (int)roomProperties["MissPacmanScore"];
            teamMissPacmanScore.text = $"Team Miss Pacman Score: {missPacmanScore}";
        }
    }
}
