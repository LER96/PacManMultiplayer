using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI teamPacmanScore;
    [SerializeField] TextMeshProUGUI teamMissPacmanScore;

    void Update()
    {
        int pacmanScore = GameManager.instance.teamScore;
        teamPacmanScore.text = $"Team Pacman Score: {pacmanScore}";
    }
}
