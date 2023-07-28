using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Movement
{
    string teamName;

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

    public override void StartingPoint(Vector3 pos)
    {
        base.StartingPoint(pos);
    }

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        base.OnPhotonInstantiate(info);
    }

    //if pacman is in powerup mode you get eaten instead. 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name != "Walls" )
        {
            PhotonView photonView = collision.transform.GetComponent<PhotonView>();
            Movement movement = photonView.transform.GetComponent<Movement>();
            teamName = movement.myTeamName;
            //teamName = (string)photonView.Owner.CustomProperties["Team"];

            bool powerMode = (bool)photonView.Owner.CustomProperties["PowerMode"];

            if (CompareTeam(teamName)==false)
            {
                if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman"))
                {
                    CheckPowerMode(powerMode, collision.gameObject);
                }
                else if (collision.gameObject.layer == LayerMask.NameToLayer("Miss Pacman"))
                {
                    CheckPowerMode(powerMode, collision.gameObject);
                }
            }
        }
    }

    void CheckPowerMode(bool powerMode, GameObject obj)
    {
        if (powerMode == true)
        {
            GameManager.instance.GhostEaten(teamName ,this.gameObject);
            
        }
        else
        {
            GameManager.instance.PacEaten(this.myTeamName, obj);
        }
    }

    bool CompareTeam(string team)
    {
        if (this.myTeamName == team)
        {
            return true;
        }
        else
            return false;
    }
}
