using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class TeamSelectionManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TeamManager _teamManager;

    [Header("UI Refrences")]
    [SerializeField] TextMeshProUGUI _teamPmMembersText;
    [SerializeField] TextMeshProUGUI _teamMsPmMembersText;

    public void JoinTeamPM()
    {
        _teamPmMembersText.text = PhotonNetwork.LocalPlayer.NickName;
        photonView.RPC("JoinTeamPM", PhotonNetwork.LocalPlayer);
    }
  
    public void JoinTeamMsPM()
    {
        _teamMsPmMembersText.text = PhotonNetwork.LocalPlayer.NickName;
        photonView.RPC("JoinTeamMsPM", PhotonNetwork.LocalPlayer);
    }
}
