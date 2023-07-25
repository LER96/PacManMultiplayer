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

    public void SetPlayerInfo(Player _player)
    {
        _playerNameText.text = _player.NickName;
        player = _player;
    }

    public void SetPlayerInfoUI(Player player)
    {
        _playerRoleText.text = $"Role: {(string)player.CustomProperties["Character"]}";
        _playerTeamText.text = $"Team: {(string)player.CustomProperties["Team"]}";
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (player == targetPlayer)
        {
            SetPlayerInfoUI(targetPlayer);
        }        
    }

    // public void DisableRoleSwitch()
    // {
    //     foreach (Player player in PhotonNetwork.PlayerList)
    //     {
    //         string role = (string)player.CustomProperties["Character"];
    //
    //         if (role == "Pacman" || role == "Miss Pacman")
    //         {
    //             _joinTeamPmButton.SetActive(false);
    //             _joinTeamMsPmButton.SetActive(false);
    //         }
    //     }
    // }
}
