using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System;
using UnityEngine.TextCore.Text;

public class TeamSelectionManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TeamManager _teamManager;

    [Header("UI Refrences")]
    [SerializeField] TextMeshProUGUI _teamPmMembersText;
    [SerializeField] TextMeshProUGUI _teamMsPmMembersText;
    [SerializeField] GameObject _joinTeamPmButton; 
    [SerializeField] GameObject _joinTeamMsPmButton; 
    [SerializeField] GameObject _startGameButton; 

    //ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();

    //limit team to 3 players. 
    //start game only for master client
    //start game button only interactive when teams are full
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            AssignRole();
            _startGameButton.SetActive(true);
        }

        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Pacman"))
        {
            _joinTeamPmButton.SetActive(false);
        }

        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Miss Pacman"))
        {
            _joinTeamMsPmButton.SetActive(false);
        }
    }

    public void JoinTeamPM(string team)
    {
        //photonView.RPC(_teamManager.JOIN_TEAM_PM, RpcTarget.All, PhotonNetwork.LocalPlayer);
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Team Pacman", team } });
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Character", "Ghost" } });
        // RefreshTeamsUI();
    }

    public void JoinTeamMsPM(string team)
    {
        //photonView.RPC(_teamManager.JOIN_TEAM_MSPM, RpcTarget.All, PhotonNetwork.LocalPlayer);
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Team MissPacman", team } });
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Character", "Ghost" } });
        // RefreshTeamsUI();
    }

    public void AssignRole()
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

        // RefreshTeamsUI();

        //playerProperties.Add("Pacman", playerList[0].NickName);
        //PhotonNetwork.PlayerList[0].SetCustomProperties(playerProperties);
        PhotonNetwork.PlayerList[0].SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Character", "Pacman" } });
        PhotonNetwork.PlayerList[0].SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Team Pacman", "Team Pacman" } });

        //playerProperties.Add("Miss Pacman", playerList[1].NickName);
        //PhotonNetwork.PlayerList[1].SetCustomProperties(playerProperties);

        PhotonNetwork.PlayerList[1].SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Character", "Miss Pacman" } });
        PhotonNetwork.PlayerList[1].SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Team MissPacman", "Team MissPacman" } });
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("Team Pacman"))
        {
            UpdatePacmanTeamUI();
        }

        if (changedProps.ContainsKey("Team MissPacman"))
        {
            UpdateMissPacmanTeamUI();
        }

        CheckTeamPmSize();
        CheckTeamMsPmSize();
    }

    void CheckTeamPmSize()
    {
        int teamSize = 0;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("Team Pacman"))
            {
                teamSize++;
            }
        }

        if (teamSize >= PhotonNetwork.CurrentRoom.MaxPlayers / 2)
        {
            _joinTeamPmButton.SetActive(false);
        }
    }

    void CheckTeamMsPmSize()
    {
        int teamSize = 0;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("Team MissPacman"))
            {
                teamSize++;
            }
        }

        if (teamSize >= PhotonNetwork.CurrentRoom.MaxPlayers / 2)
        {
            _joinTeamMsPmButton.SetActive(false);
        }
    }

    private void UpdatePacmanTeamUI()
    {
        string teamPacmanText = "";

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("Team Pacman"))
            {
                teamPacmanText += player.NickName + "\n";
            }
        }

        _teamPmMembersText.text = teamPacmanText;
    }

    private void UpdateMissPacmanTeamUI()
    {
        string teamMissPacmanText = "";

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("Team MissPacman"))
            {
                teamMissPacmanText += player.NickName + "\n";
            }
        }

        _teamMsPmMembersText.text = teamMissPacmanText;
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

    private void AssignCharacter(Player player, string character)
    {
        // Assign the character to the player using custom properties
        player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Character", character } });
    }

    private void AssignTeam(Player player, string team)
    {
        player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Character", team } });
    }
}
