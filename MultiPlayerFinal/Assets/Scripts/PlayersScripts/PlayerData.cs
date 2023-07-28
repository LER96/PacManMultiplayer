using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class PlayerData : MonoBehaviourPunCallbacks
{
    [SerializeField] TextMeshProUGUI _playerNameText;
    [SerializeField] TextMeshProUGUI _playerTeamText;
    [SerializeField] TextMeshProUGUI _playerRoleText;
    [SerializeField] Image _playerSprite;
    [SerializeField] GameObject[] _sprites;

    Player player;

    public void SetPlayerInfo(Player _player)
    {
        _playerNameText.text = _player.NickName;
        player = _player;

        if (player.CustomProperties.ContainsKey("Character"))
            SetSprite(_player);
    }

    public void SetPlayerInfoUI(Player player)
    {
        _playerRoleText.text = $"Role: {(string)player.CustomProperties["Character"]}";
        _playerTeamText.text = $"Team: {(string)player.CustomProperties["Team"]}";
        SetSprite(player);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (player == targetPlayer)
        {
            SetPlayerInfoUI(targetPlayer);
        }
    }

    public void SetSprite(Player player)
    {
        string s = (string)player.CustomProperties["Character"];

        for (int i = 0; i < _sprites.Length; i++)
        {
            if (s == _sprites[i].name.ToString())
            {
                _playerSprite.sprite = _sprites[i].GetComponent<Image>().sprite;
                _playerSprite.color = _sprites[i].GetComponent<Image>().color;
            }
        }
    }
}
