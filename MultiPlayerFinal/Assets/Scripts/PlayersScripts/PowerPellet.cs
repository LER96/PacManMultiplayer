using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPellet : EatingPellets
{
    public float PowerupDuration = 12;

    //notify all players (probably with rpc) that this player eat the power pallet, after checking which player is it.
    //most likely use boolean to tell if in power mode.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string character = (string)PhotonNetwork.LocalPlayer.CustomProperties["Character"];

        if (collision.CompareTag("Pacman") || collision.CompareTag("MsPacman"))
        {
            Eat();
        }
    }

    public override void Eat()
    {
        base.Eat();
    }
}
