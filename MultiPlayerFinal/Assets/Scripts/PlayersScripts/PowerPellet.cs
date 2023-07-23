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
            Eat(PhotonNetwork.LocalPlayer);
        }

        if (collision.CompareTag("MsPacman"))
        {
            Eat(PhotonNetwork.LocalPlayer);
        }
    }

    public void Eat(Player player)
    {
        GameManager.instance.EatenPowerPellets(this, player);
    }
}
