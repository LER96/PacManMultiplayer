using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatingPellets : MonoBehaviour
{
    [SerializeField] public int score = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pacman"))
        {
            PhotonView photonView = collision.GetComponent<PhotonView>();
            string teamName = (string)photonView.Owner.CustomProperties["Team"];
            Eat(teamName);
        }
        else if(collision.CompareTag("MsPacman"))
        {
            PhotonView photonView = collision.GetComponent<PhotonView>();
            string teamName = (string)photonView.Owner.CustomProperties["Team"];
            Eat(teamName);
        }
    }

    public virtual void Eat(string team)
    {
        TeamManagement.instance.EatenPellets(this, team);
    }
}
