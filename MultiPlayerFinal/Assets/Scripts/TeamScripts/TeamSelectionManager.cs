using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamSelectionManager : MonoBehaviour
{
    [Header("UI Refrences")]
    [SerializeField] TextMeshProUGUI _teamPmMembersText;
    [SerializeField] TextMeshProUGUI _teamMsPmMembersText;

    public void JoinTeamPM()
    {
        GameManager.instance._team = 0;
        _teamPmMembersText.text = "0";
    }

    public void JoinTeamMsPM()
    {
        GameManager.instance._team = 1;
        _teamMsPmMembersText.text = "1";
    }
}
