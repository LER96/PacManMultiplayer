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

    //start game button only interactive when teams are full
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            AssignRole();
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
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "PowerMode", false } });
    }

    public void JoinTeamMsPM(string team)
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
            PhotonNetwork.LocalPlayer.CustomProperties.Remove("Team");

        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Character"))
            PhotonNetwork.LocalPlayer.CustomProperties.Remove("Character");

        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Team", team } });
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Character", "Ghost" } });
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "PowerMode", false } });
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

        Debug.Log($"{playerList[1]} is Miss Pacman");

        playerList[0].SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Character", "Pacman" } });
        playerList[0].SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Team", "Pacman" } });
        playerList[0].SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "PowerMode", false } });

        playerList[1].SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Character", "Miss Pacman" } });
        playerList[1].SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Team", "Miss Pacman" } });
        playerList[1].SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "PowerMode", false } });
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        UpdateTeamsUI();

        if (targetPlayer == PhotonNetwork.LocalPlayer)
        {
            if (changedProps.ContainsKey("Character"))
            {
                DisableRoleSwitch();
            }

            CheckTeamsSize();
        }
    }

    void CheckTeamsSize()
    {
        int teamPmSize = 0;
        int teamMsPmSize = 0;
        string localPlayerRole = (string)PhotonNetwork.LocalPlayer.CustomProperties["Character"];

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            string t = (string)player.CustomProperties["Team"];
            string role = (string)player.CustomProperties["Character"];

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

            if (player == PhotonNetwork.LocalPlayer && (role == "Pacman" || role == "Miss Pacman"))
            {
                _joinTeamPmButton.SetActive(false);
                _joinTeamMsPmButton.SetActive(false);
                return; 
            }
        }

        _teamPmFull = teamPmSize >= PhotonNetwork.CurrentRoom.MaxPlayers / 2;
        _teamMsPmFull = teamMsPmSize >= PhotonNetwork.CurrentRoom.MaxPlayers / 2;

        if (localPlayerRole != "Pacman" && localPlayerRole != "Miss Pacman")
        {
            _joinTeamPmButton.SetActive(!_teamPmFull);
            _joinTeamMsPmButton.SetActive(!_teamMsPmFull);
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
        string localPlayerRole = (string)PhotonNetwork.LocalPlayer.CustomProperties["Character"];

        if (localPlayerRole == "Pacman" || localPlayerRole == "Miss Pacman")
        {
            _joinTeamPmButton.SetActive(false);
            _joinTeamMsPmButton.SetActive(false);
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
            _playerDataList.Add(_newPlayerData);
        }
    }
}
