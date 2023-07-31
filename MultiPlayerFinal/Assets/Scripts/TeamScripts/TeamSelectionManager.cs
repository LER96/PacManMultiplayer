using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
//using System;
using UnityEngine.TextCore.Text;

public class TeamSelectionManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TeamManager _teamManager;
    const string ASSIGN_GHOST_RPC = nameof(AssignGhost);

    [Header("Players List")]
    [SerializeField] List<string> ghostNames;
    [SerializeField] List<string> _copyGhostNames;
    string characterName;

    [Header("Player Data")]
    [SerializeField] List<PlayerData> _playerDataList = new List<PlayerData>();
    [SerializeField] PlayerData _playerData;

    bool _teamPmFull = false;
    bool _teamMsPmFull = false;

    [Header("UI Refrences")]
    [SerializeField] Transform _PmTeamPlayerDataParent;
    [SerializeField] GameObject _joinTeamPmButton;
    [SerializeField] GameObject _joinTeamMsPmButton;
    [SerializeField] GameObject _startGameButton;

    private void Awake()
    {
        foreach (string ghostName in ghostNames)
            _copyGhostNames.Add(ghostName);

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

    private void Update()
    {
        ShowStart();
    }

    void ShowStart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _startGameButton.SetActive(true);
        }
        else
        {
            _startGameButton.SetActive(false);
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
        if (characterName != "" && _copyGhostNames.Contains(characterName) == false)
        {
            _copyGhostNames.Add(characterName);
        }

        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
            PhotonNetwork.LocalPlayer.CustomProperties.Remove("Team");

        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Character"))
            PhotonNetwork.LocalPlayer.CustomProperties.Remove("Character");

        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Team", team } });
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "PowerMode", false } });

        photonView.RPC(ASSIGN_GHOST_RPC, RpcTarget.AllViaServer, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    void AssignGhost(Player player)
    {
        int rnd = Random.Range(0, _copyGhostNames.Count);
        characterName = _copyGhostNames[rnd];
        player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Character", characterName } });
        _copyGhostNames.Remove(characterName);
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

        playerList[0].SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Character", "Pacman" } });
        playerList[0].SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Team", "Pacman" } });
        playerList[0].SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "PowerMode", false } });

        playerList[1].SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Character", "Miss Pacman" } });
        playerList[1].SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Team", "Miss Pacman" } });
        playerList[1].SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "PowerMode", false } });
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
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
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerData _newPlayerData = Instantiate(_playerData, _PmTeamPlayerDataParent);
            _newPlayerData.SetPlayerInfo(player.Value);
            _playerDataList.Add(_newPlayerData);
        }
    }
}
