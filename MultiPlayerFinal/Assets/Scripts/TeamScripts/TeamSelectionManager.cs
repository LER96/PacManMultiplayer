using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class TeamSelectionManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TeamManager _teamManager;

    [Header("UI Refrences")]
    [SerializeField] TextMeshProUGUI _teamPmMembersText;
    [SerializeField] TextMeshProUGUI _teamMsPmMembersText;

    private void Start()
    {
        AssignPacMan();
    }

    public void JoinTeamPM()
    {
        _teamPmMembersText.text = PhotonNetwork.LocalPlayer.NickName;
        photonView.RPC(_teamManager.JOIN_TEAM_PM, RpcTarget.All ,PhotonNetwork.LocalPlayer);
    }
  
    public void JoinTeamMsPM()
    {
        _teamMsPmMembersText.text = PhotonNetwork.LocalPlayer.NickName;
        photonView.RPC(_teamManager.JOIN_TEAM_MSPM, RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    public void AssignPacMan()
    {
        List<Player> playerList = PhotonNetwork.CurrentRoom.Players.Values.ToList();

        if (playerList.Count >= 2)
        {
            for (int i = 0; i < playerList.Count; i++)
            {
                int randomIndex = Random.Range(i, playerList.Count);
                Player temp = playerList[randomIndex];
                playerList[randomIndex] = playerList[i];
                playerList[i] = temp;
            }

            Debug.Log($"{playerList[0]} is Pacman");
            Debug.Log($"{playerList[1]} is Miss Pacman");
        }
        else
            Debug.Log("Not enough players in room");
    }
}
