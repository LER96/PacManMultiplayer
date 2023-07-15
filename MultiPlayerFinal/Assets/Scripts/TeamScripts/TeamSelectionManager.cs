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

    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();

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
        RefreshTeamsUI();
    }

    public void JoinTeamMsPM()
    {
        photonView.RPC(_teamManager.JOIN_TEAM_MSPM, RpcTarget.All, PhotonNetwork.LocalPlayer);
        RefreshTeamsUI();
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
       // photonView.RPC(_teamManager.JOIN_TEAM_PM, RpcTarget.All, playerList[0]);

        Debug.Log($"{playerList[1]} is Miss Pacman");
       // photonView.RPC(_teamManager.JOIN_TEAM_MSPM, RpcTarget.All, playerList[1]);

        RefreshTeamsUI();

        playerProperties.Add("Pacman", playerList[0].NickName);
        PhotonNetwork.PlayerList[0].SetCustomProperties(playerProperties);

        playerProperties.Add("Miss Pacman", playerList[1].NickName);
        PhotonNetwork.PlayerList[1].SetCustomProperties(playerProperties);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        _teamPmMembersText.text = (string)targetPlayer.CustomProperties["Pacman"];
        _teamMsPmMembersText.text = (string)targetPlayer.CustomProperties["Miss Pacman"];
    }

    void RefreshTeamsUI()
    {
        _teamPmMembersText.text = string.Empty;

        foreach (Player player in _teamManager._teamPm)
        {
            _teamPmMembersText.text += $"{player.NickName}" + Environment.NewLine;
            Debug.Log($"{player.NickName} is in team Pacman");
        }

        _teamMsPmMembersText.text = string.Empty;

        foreach (Player player in _teamManager._teamMsPm)
        {
            _teamMsPmMembersText.text += $"{player.NickName}" + Environment.NewLine;
            Debug.Log($"{player.NickName} is in team Miss Pacman");
        }
    }
}
