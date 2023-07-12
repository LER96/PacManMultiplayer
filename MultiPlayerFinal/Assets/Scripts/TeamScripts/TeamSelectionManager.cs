using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamSelectionManager : MonoBehaviour
{
    [SerializeField] TeamManager _teamManager;

    [Header("UI Refrences")]
    [SerializeField] TextMeshProUGUI _teamPmMembersText;
    [SerializeField] TextMeshProUGUI _teamMsPmMembersText;

    public void JoinTeamPM()
    {
        _teamPmMembersText.text = "0";
    }
  
    public void JoinTeamMsPM()
    {
        _teamMsPmMembersText.text = "1";
    }
}
