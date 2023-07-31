using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public abstract class GameManager : MonoBehaviourPunCallbacks
{
    public ExitGames.Client.Photon.Hashtable roomProperties;

    [SerializeField] public static GameManager instance;
    [SerializeField] public Ghost[] ghosts;
    [SerializeField] public PacmanMovement pacman;
    [SerializeField] public Transform pellets;
    [SerializeField] public int pacEatenScore = 30;
    [SerializeField] public int ghostEatenScore = 20;
   
    public bool roundEnded { get; private set; }
    public bool gameIsFinished { get; private set; } = false;
    public int _team;
    public int teamPmScore { get; private set; }
    public int teamMsPmScore { get; private set; }
    public int teamMsPmRoundScore { get; private set; }
    public int teamPmRoundScore { get; private set; }
    public int rounds { get; set; }
    public int currentRound { get; set; }

    private void Awake()
    {
        gameIsFinished = false;
        currentRound = 0;
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
    }

    public void NewGame()
    {
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "PacmanScore", 0 } });
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "MissPacmanScore", 0 } });
        this.teamPmScore = 0;
        this.teamMsPmScore = 0;
    }

    public void QuitGame()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel(0);
    }

    [PunRPC]
    public void NextRound()
    {
        NewGame();
        SpawnManager.Instance.RespawnAllPlayers();

        foreach (Transform pellet in this.pellets)
        {
            pellet.gameObject.SetActive(true);
        }

        roundEnded = false;
    }

    public void SetTeamScore(int score, string team)
    {
        if (team == "Pacman")
        {
            this.teamPmScore += score;
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "PacmanScore", this.teamPmScore } });
        }

        if (team == "Miss Pacman")
        {
            this.teamMsPmScore += score;
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "MissPacmanScore", this.teamMsPmScore } });
        }
    }

    public void SetRoundScore()
    {
        int teamPmScore = (int)PhotonNetwork.CurrentRoom.CustomProperties["PacmanScore"];
        int teamMsPmScore = (int)PhotonNetwork.CurrentRoom.CustomProperties["MissPacmanScore"];

        if (teamPmScore > teamMsPmScore)
        {
            teamPmRoundScore++;
        }
        else if (teamMsPmScore > teamPmScore)
        {
            teamMsPmRoundScore++;
        }
    }

    public void SetRounds(int rounds)
    {
        this.rounds = rounds;
    }

    public void PacEaten(string team, GameObject obj)
    {
        SetTeamScore(pacEatenScore, team);
    }

    public void GhostEaten(string team, GameObject obj)
    {
        SetTeamScore(ghostEatenScore, team);
    }

    public virtual void EatenPellets(EatingPellets pellets, string team)
    {
    }

    public void EatenPowerPellets(PowerPellet powerPellet, Player player, string team)
    {
        EatenPellets(powerPellet, team);

        string character = (string)player.CustomProperties["Character"];

        if (character == "Pacman")
        {
            ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable();
            customProps["PowerMode"] = true;
            player.SetCustomProperties(customProps);
        }

        if (character == "Miss Pacman")
        {
            ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable();
            customProps["PowerMode"] = true;
            player.SetCustomProperties(customProps);
        }

        StartCoroutine(PowerModeCD(powerPellet.PowerupDuration, player));
    }

    [PunRPC]
    public void EndRound()
    {
        SetRoundScore();
        currentRound++;
        int roomRounds = (int)roomProperties["Rounds"];
        if (currentRound < roomRounds)
        {
            roundEnded = true;
            gameIsFinished = false;
        }
        else
        {
            gameIsFinished = true;
        }
    }

    public bool RemainingPellets()
    {
        foreach (Transform pellets in this.pellets)
        {
            if (pellets.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator PowerModeCD(float timer, Player player)
    {
        // Wait for the power mode duration
        yield return new WaitForSeconds(timer);

        // When the timer ends, set the PowerMode Custom Property to false
        ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable();
        customProps["PowerMode"] = false;
        player.SetCustomProperties(customProps);
    }

    public IEnumerator Respawn(GameObject obj)
    {
        PhotonView view = obj.GetComponent<PhotonView>();
        Movement mineView = view.GetComponent<Movement>();
        mineView.canMove = false;
        mineView.transform.position = mineView.startingPosition;
        yield return new WaitForSeconds(2f);

        mineView.canMove = true;
    }

    [PunRPC]
    private void UpdatePlayerDataRPC(Player player)
    {
        string team = (string)player.CustomProperties["Team"];
        int teamScore = team == "Pacman" ? (int)PhotonNetwork.CurrentRoom.CustomProperties["PacmanScore"] : (int)PhotonNetwork.CurrentRoom.CustomProperties["MissPacmanScore"];
        PlayerData playerData = new PlayerData();

        playerData.nickname = player.NickName;
        playerData.teamName = team;
        playerData.score = teamScore;
    }
}
