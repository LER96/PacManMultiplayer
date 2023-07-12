using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    List<PlayerData> _teamPm = new List<PlayerData>();
    List<PlayerData> _teamMsPm = new List<PlayerData>();

    public void JoinTeamPM(PlayerData player)
    {
        _teamPm.Add(player);
    }

    public void JoinTeamMsPM(PlayerData player)
    {
        _teamMsPm.Add(player);
    }
}
