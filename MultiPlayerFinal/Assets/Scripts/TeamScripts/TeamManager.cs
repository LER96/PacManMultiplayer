using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeamManager : MonoBehaviourPunCallbacks
{
    List<Player> _teamPm = new List<Player>();
    List<Player> _teamMsPm = new List<Player>();

    public string JOIN_TEAM_PM { get; private set; } = nameof(JoinTeamPM);
    public string JOIN_TEAM_MSPM { get; private set; } = nameof(JoinTeamMsPM);

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
