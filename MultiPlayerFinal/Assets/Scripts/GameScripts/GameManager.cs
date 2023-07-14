using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameManager : MonoBehaviour
{
    [SerializeField] public Ghost[] ghosts;
    [SerializeField] public PacmanMovement pacman;
    [SerializeField] public Transform pellets;
    [SerializeField] public static GameManager instance;

    public int _team;
    [SerializeField] public int pacEatenScore;
    [SerializeField] public int ghostEatenScore;

    public int teamScore { get; private set; }
    public int rounds { get; private set; }


    private void Awake()
    {
        instance = this;
    }

    public void NewGame()
    {
        SetTeamScore(0);
        SetRounds(0);
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

    public void GameOver()
    {
        //announce which team won
    }

    public void PacEaten()
    {
        //reset pac's position
        SetTeamScore(teamScore + pacEatenScore);
    }

    public void GhostEaten()
    {
        //reset ghost's position
        SetTeamScore(teamScore + ghostEatenScore);
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

    public void EatenPowerPellets( PowerPellet powerPellet)
    {
        EatenPellets(powerPellet);

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

}
