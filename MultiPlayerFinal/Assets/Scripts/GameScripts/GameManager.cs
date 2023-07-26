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
    [SerializeField] public int pacEatenScore = 30;
    [SerializeField] public int ghostEatenScore = 20;

    // public bool pacmanInPowerMode = false;
    // public bool mspacmanInPowerMode = false;

    public int teamPmScore { get; private set; }
    public int teamMsPmScore { get; private set; }
    public int rounds { get; set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public void NewGame()
    {
        SetTeamScore(0, "Pacman");
        SetTeamScore(0, "Miss Pacman");
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

    public void SetRounds(int rounds)
    {
        this.rounds = rounds;
        Debug.Log(rounds);
    }

    public void PacEaten(string team, GameObject obj)
    {
        //reset pac's position
        SetTeamScore(pacEatenScore, team);
        StartCoroutine(Respawn(obj));
        Debug.Log("Pacman eaten");
    }

    public void GhostEaten(string team, GameObject obj)
    {
        //reset ghost's position
        SetTeamScore(ghostEatenScore, team);
        StartCoroutine(Respawn(obj));
        Debug.Log("Ghost eaten");
    }

    public void EatenPellets(EatingPellets pellets, string team)
    {
        pellets.gameObject.SetActive(false);

        SetTeamScore(pellets.score, team);

        if (!RemainingPellets())
        {
            //freeze all movements
            Invoke(nameof(NextRound), 2);
        }
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

    public IEnumerator Respawn(GameObject obj)
    {
        obj.SetActive(false);
        yield return new WaitForSeconds(4f);

        obj.transform.position = new Vector3(0f, -3.5f, -5f);
        obj.SetActive(true);
    }
}
