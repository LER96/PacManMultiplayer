using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PowerPellet : EatingPellets
{
    public float PowerupDuration = 12f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pacman"))
        {
            PhotonView photonView = collision.GetComponent<PhotonView>();
            string teamName = (string)photonView.Owner.CustomProperties["Team"];

            Eat(photonView.Owner, teamName);
        }

        if (collision.CompareTag("MsPacman"))
        {
            PhotonView photonView = collision.GetComponent<PhotonView>();
            string teamName = (string)photonView.Owner.CustomProperties["Team"];

            Eat(photonView.Owner, teamName);
        }
    }

    public void Eat(Player player, string team)
    {
        GameManager.instance.EatenPowerPellets(this, player, team);
    }
}
