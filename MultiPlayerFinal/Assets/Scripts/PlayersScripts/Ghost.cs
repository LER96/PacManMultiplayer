using Photon.Pun;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //check if in the same team if yes do nothing.
        //if pacman is in powerup mode you get eaten instead.
        if (collision.CompareTag("Pacman") || collision.CompareTag("MsPacman"))
        {
            GameManager.instance.PacEaten();
        }
    }
}
