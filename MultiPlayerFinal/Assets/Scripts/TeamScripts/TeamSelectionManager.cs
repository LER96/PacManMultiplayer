using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System;

public class TeamSelectionManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TeamManager _teamManager;

    [Header("UI Refrences")]
    [SerializeField] TextMeshProUGUI _teamPmMembersText;
    [SerializeField] TextMeshProUGUI _teamMsPmMembersText;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            AssignPacMan();
        }

        RefreshTeamsUI();
    }

    public void JoinTeamPM()
    {
        photonView.RPC(_teamManager.JOIN_TEAM_PM, RpcTarget.All, PhotonNetwork.LocalPlayer);
        RefreshTeamPmUI();
    }

    public void JoinTeamMsPM()
    {
        photonView.RPC(_teamManager.JOIN_TEAM_MSPM, RpcTarget.All, PhotonNetwork.LocalPlayer);
        RefreshTeamMsPmUI();
    }

    public void AssignPacMan()
    {
        List<Player> playerList = PhotonNetwork.CurrentRoom.Players.Values.ToList();

        for (int i = 0; i < playerList.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, playerList.Count);
            Player temp = playerList[randomIndex];
            playerList[randomIndex] = playerList[i];
            playerList[i] = temp;
        }

        Debug.Log($"{playerList[0]} is Pacman");
        photonView.RPC(_teamManager.JOIN_TEAM_PM, RpcTarget.All, playerList[0]);

        Debug.Log($"{playerList[1]} is Miss Pacman");
        photonView.RPC(_teamManager.JOIN_TEAM_MSPM, RpcTarget.All, playerList[1]);

        RefreshTeamsUI();
    }

    void RefreshTeamsUI()
    {
        Debug.Log($"{_teamManager._teamPm[0]} is Pacman");
        RefreshTeamPmUI();

        Debug.Log($"{_teamManager._teamMsPm[0]} is Pacman");
        RefreshTeamMsPmUI();
    }

    void RefreshTeamPmUI()
    {
        _teamPmMembersText.text = string.Empty;

        foreach (Player player in _teamManager._teamPm)
        {
            _teamPmMembersText.text += $"{player.NickName}" + Environment.NewLine;
        }
    }

    void RefreshTeamMsPmUI()
    {
        _teamMsPmMembersText.text = string.Empty;

        foreach (Player player in _teamManager._teamMsPm)
        {
            _teamMsPmMembersText.text += $"{player.NickName}" + Environment.NewLine;
        }
    }
}
