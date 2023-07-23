using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public abstract class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] public Ghost[] ghosts;
    [SerializeField] public PacmanMovement pacman;
    [SerializeField] public Transform pellets;
    [SerializeField] public static GameManager instance;

    public int _team;
    [SerializeField] public int pacEatenScore;
    [SerializeField] public int ghostEatenScore;

   // public bool pacmanInPowerMode = false;
   // public bool mspacmanInPowerMode = false;

    public int teamScore { get;  set; }
    public int rounds { get;  set; }

    private void Awake()
    {
        instance = this;
    }

    public void NewGame()
    {
        SetTeamScore(0);
        SetRounds(0);
    }

    public void GameOver()
    {
        //announce which team won
    }

    public void QuitGame()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel(0);
    }

    public void NextRound()
    {
        foreach (Transform pellet in this.pellets)
        {
            pellet.gameObject.SetActive(true);
        }

        //later reset everyone's position
    }

    public void SetTeamScore(int score)
    {
        this.teamScore = score;
    }

    public void SetRounds(int rounds)
    {
        this.rounds = rounds;
    }

    public void PacEaten()
    {
        //reset pac's position
        SetTeamScore(teamScore + pacEatenScore);
        Debug.Log("Pacman eaten");
    }

    public void GhostEaten()
    {
        //reset ghost's position
        SetTeamScore(teamScore + ghostEatenScore);
        Debug.Log("Ghost eaten");
    }

    public void EatenPellets(EatingPellets pellets)
    {
        pellets.gameObject.SetActive(false);
        SetTeamScore(this.teamScore + pellets.score);

        if (!RemainingPellets())
        {
            //freeze all movements
            Invoke(nameof(NextRound), 2);
        }
    }

    public void EatenPowerPellets(PowerPellet powerPellet, Player player)
    {
        EatenPellets(powerPellet);

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

        //change ghost states to be eaten (based on team)
    }

    public void EndRound()
    {
        foreach (Transform pellet in pellets)
        {
            if (pellet.childCount <= 0)
            {
                Invoke(nameof(NextRound), 2f);
                //implement max round
            }
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
        Debug.Log($"{player.NickName} is in powermode");
        yield return new WaitForSeconds(timer);

        // When the timer ends, set the PowerMode Custom Property to false
        ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable();
        customProps["PowerMode"] = false;
        player.SetCustomProperties(customProps);
        Debug.Log($"{player.NickName} is in powermode {customProps["PowerMode"]}");
    }
}
