using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class UIManager : MonoBehaviourPunCallbacks
{

    ExitGames.Client.Photon.Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
    [SerializeField] TextMeshProUGUI teamPacmanScore;
    [SerializeField] TextMeshProUGUI teamMissPacmanScore;
    [SerializeField] PlayerData _playerData;
    [SerializeField] Transform _PmTeamPlayerDataParent;
    [SerializeField] Transform _MsPmTeamPlayerDataParent;

    [Header("Round UI Refrences")]
    [SerializeField] GameObject _endRoundUI;
    [SerializeField] GameObject _nextRound;
    [SerializeField] GameObject _gameOver;
    [SerializeField] TextMeshProUGUI _missPmRoundScoreText;
    [SerializeField] TextMeshProUGUI _pmRoundScoreText;

    private void Awake()
    {
        UpdatePlayerDataUI();
    }

    void Update()
    {
        UpdateTeamScores();
        RoundEnded();
        //GameOver();
    }



    public void UpdateTeamScores()
    {
        if (roomProperties.ContainsKey("PacmanScore"))
        {
            int pacmanScore = (int)roomProperties["PacmanScore"];
            teamPacmanScore.text = $"Team Pacman Score: {pacmanScore}";
        }

        if (roomProperties.ContainsKey("MissPacmanScore"))
        {
            int missPacmanScore = (int)roomProperties["MissPacmanScore"];
            teamMissPacmanScore.text = $"Team Miss Pacman Score: {missPacmanScore}";
        }
    }

    void UpdatePlayerDataUI()
    {
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            string team = (string)player.Value.CustomProperties["Team"];

            Transform parent = team == "Pacman" ? _PmTeamPlayerDataParent : _MsPmTeamPlayerDataParent;

            PlayerData _newPlayerData = Instantiate(_playerData, parent);
            _newPlayerData.SetPlayerInfo(player.Value);
            _newPlayerData.SetPlayerInfoUI(player.Value);
        }
    }

    void UpdateRoundScoreUI()
    {
        _pmRoundScoreText.text = $"Team Pacman Round Score: {GameManager.instance.teamPmRoundScore}";
        _missPmRoundScoreText.text = $"Team Miss Pacman Round Score: {GameManager.instance.teamMsPmRoundScore}";
    }

    public void RoundEnded()
    {
        if (GameManager.instance.roundEnded)
        {
            _endRoundUI.SetActive(true);
            if(PhotonNetwork.IsMasterClient)
            {
                _nextRound.SetActive(true);
            }
            else
            {
                _nextRound.SetActive(false);
            }

            UpdateRoundScoreUI();
        }
        else
        {
            _endRoundUI.SetActive(false);
        }
    }

    //public void GameOver()
    //{
    //    if(GameManager.instance.gameIsFinished)
    //    {
    //        _endRoundUI.SetActive(true);
    //        _gameOver.SetActive(true);
    //    }
    //}
}
