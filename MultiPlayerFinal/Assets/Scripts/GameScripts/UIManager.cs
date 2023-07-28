using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class UIManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TextMeshProUGUI teamPacmanScore;
    [SerializeField] TextMeshProUGUI teamMissPacmanScore;

    void Update()
    {
        UpdateTeamScores();
    }

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
