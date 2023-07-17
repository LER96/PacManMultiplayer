using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviourPunCallbacks
{
    public string playerID;
    public int score;
    Player player;
    
    public void SetPlayerInfo(Player _player)
    {
        _player = player;
        playerID = _player.UserId;
    }
}
