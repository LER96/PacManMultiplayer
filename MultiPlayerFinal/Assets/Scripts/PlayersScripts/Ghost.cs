using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Movement
{
    public override void Update()
    {
        if (canMove)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                this.SetDirection(Vector2.up);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                this.SetDirection(Vector2.down);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                this.SetDirection(Vector2.right);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                this.SetDirection(Vector2.left);
            }
        }
        base.Update();
    }

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        base.OnPhotonInstantiate(info);
    }

    //if pacman is in powerup mode you get eaten instead. 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        string teamName = (string)PhotonNetwork.LocalPlayer.CustomProperties["Team"];

        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman") && teamName == "Pacman")
        {
            if (GameManager.instance.pacmanInPowerMode)
            {
                GameManager.instance.GhostEaten();
            }
            else
                GameManager.instance.PacEaten();
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Miss Pacman") && teamName == "Pacman")
        {
            if (GameManager.instance.mspacmanInPowerMode)
            {
                GameManager.instance.GhostEaten();
            }
            else
                GameManager.instance.PacEaten();
        }
    }

}
