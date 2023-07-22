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
    [SerializeField] List<PlayerData> _playerDataList = new List<PlayerData>();
    [SerializeField] PlayerData _playerData;
    [SerializeField] Transform _playerDataParent;
    bool _teamPmFull = false;
    bool _teamMsPmFull = false;

    [Header("UI Refrences")]
    [SerializeField] TextMeshProUGUI _teamPmMembersText;
    [SerializeField] TextMeshProUGUI _teamMsPmMembersText;
    [SerializeField] GameObject _joinTeamPmButton;
    [SerializeField] GameObject _joinTeamMsPmButton;
    [SerializeField] GameObject _startGameButton;

    private void Awake()
    {
        UpdatePlayerList();
    }

    //limit team to 3 players. 
    //start game only for master client
    //start game button only interactive when teams are full
    private void Start()
    {
        //use photon instantiate here to make player with playerdata script
        //then assign role through the player data
        // and check through the player data which role and instantiate that

        if (PhotonNetwork.IsMasterClient)
        {
            _startGameButton.SetActive(true);
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(2);
        }
    }
    public void JoinTeamPM(string team)
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
            PhotonNetwork.LocalPlayer.CustomProperties.Remove("Team");

        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Character"))
            PhotonNetwork.LocalPlayer.CustomProperties.Remove("Character");

        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Team", team } });
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Character", "Ghost" } });
    }

    public void JoinTeamMsPM(string team)
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
            PhotonNetwork.LocalPlayer.CustomProperties.Remove("Team");

        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Character"))
            PhotonNetwork.LocalPlayer.CustomProperties.Remove("Character");

        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Team", team } });
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Character", "Ghost" } });
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        UpdateTeamsUI();
        CheckTeamsSize();
    }

    void CheckTeamsSize()
    {
        int teamPmSize = 0;
        int teamMsPmSize = 0;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            string t = (string)player.CustomProperties["Team"];

            if (t == "Pacman")
            {
                teamPmSize++;
                Debug.Log($"Team pacman size:  {teamPmSize}");
            }
            else if (t == "Miss Pacman")
            {
                teamMsPmSize++;
                Debug.Log($"Team MissPacman size:  {teamMsPmSize}");
            }
        }

        if (teamPmSize >= PhotonNetwork.CurrentRoom.MaxPlayers / 2)
        {
            _joinTeamPmButton.SetActive(false);
            _teamPmFull = true;
        }

        if (teamMsPmSize >= PhotonNetwork.CurrentRoom.MaxPlayers / 2)
        {
            _joinTeamMsPmButton.SetActive(false);
            _teamMsPmFull = true;
        }
    }

    private void UpdateTeamsUI()
    {
        string teamPacmanText = "";
        string teamMissPacmanText = "";

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            string t = (string)player.CustomProperties["Team"];
            
            if (t == "Pacman")
            {
                teamPacmanText += player.NickName + "\n";
            }
            else if (t == "Miss Pacman")
            {
                teamMissPacmanText += player.NickName + "\n";
            }
        }

        _teamMsPmMembersText.text = teamMissPacmanText;
        _teamPmMembersText.text = teamPacmanText;
    }

    void DisableRoleSwitch()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            string role = (string)player.CustomProperties["Character"];

            if (role == "Pacman" || role == "Miss Pacman")
            {
                _joinTeamPmButton.SetActive(false);
                _joinTeamMsPmButton.SetActive(false);
            }
        }
    }

    void UpdatePlayerList()
    {
        if (PhotonNetwork.CurrentRoom == null)
            return;

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerData _newPlayerData = Instantiate(_playerData, _playerDataParent);
            _newPlayerData.SetPlayerInfo(player.Value);
            //_newPlayerData.DisableRoleSwitch();

            _playerDataList.Add(_newPlayerData);
        }
    }
}
