using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    List<Player> _teamPm = new List<Player>();
    List<Player> _teamMsPm = new List<Player>();

    [PunRPC]
    public void JoinTeamPM(Player _player)
    {
        _teamPm.Add(_player);
    }

    [PunRPC]
    public void JoinTeamMsPM(Player _player)
    {
        _teamMsPm.Add(_player);
    }
}
