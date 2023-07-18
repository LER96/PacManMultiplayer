using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using Photon.Pun.UtilityScripts;

public class PlayerData : MonoBehaviourPunCallbacks
{
    [SerializeField] TextMeshProUGUI _playerNameText;
    [SerializeField] TextMeshProUGUI _playerTeamText;
    [SerializeField] TextMeshProUGUI _playerRoleText;
    public int score;
    Player player;

    private void Start()
    {
      // if (PhotonNetwork.IsMasterClient)
      // {
      //     AssignRole();
      // }
    }

    public void SetPlayerInfo(Player _player)
    {
        _playerNameText.text = _player.NickName;
        player = _player;
    }

    public void SetPlayerInfoUI(Player player)
    {
        _playerRoleText.text = $"Role: {(string)player.CustomProperties["Character"]}";
        _playerTeamText.text = $"Team: {(string)player.CustomProperties["Team"]}";

        // if (player.CustomProperties.ContainsKey("Team Pacman"))
        //     _playerTeamText.text = $"Team: {(string)player.CustomProperties["Team Pacman"]}";
        // else if (player.CustomProperties.ContainsKey("Team MissPacman"))
        //     _playerTeamText.text = $"Team: {(string)player.CustomProperties["Team MissPacman"]}";
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (player == targetPlayer)
        {
            SetPlayerInfoUI(targetPlayer);
        }

       // string t = (string)player.CustomProperties["Team"];
       //
       // if (t == "Pacman")
       //     Debug.Log("team pacman");
       // else if (t == "Miss Pacman")
       //     Debug.Log("team miss pacman");
    }

    public void AssignRole()
    {
        List<Player> playerList = PhotonNetwork.CurrentRoom.Players.Values.ToList();

        for (int i = 0; i < playerList.Count; i++)
        {
            int randomIndex = Random.Range(i, playerList.Count);
            Player temp = playerList[randomIndex];
            playerList[randomIndex] = playerList[i];
            playerList[i] = temp;
        }

        Debug.Log($"{playerList[0]} is Pacman");

        Debug.Log($"{playerList[1]} is Miss Pacman");

        PhotonNetwork.PlayerList[0].SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Character", "Pacman" } });
        PhotonNetwork.PlayerList[0].SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Team", "Pacman" } });


        PhotonNetwork.PlayerList[1].SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Character", "Miss Pacman" } });
        PhotonNetwork.PlayerList[1].SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Team", "Miss Pacman" } });
    }
}
