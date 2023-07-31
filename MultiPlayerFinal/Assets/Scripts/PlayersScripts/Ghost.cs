using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Movement
{
    string _otherTeam;

    private void Start()
    {
        if (myTeamName == "Pacman")
        {
            _otherTeam = "Miss Pacman";
        }
        else
        {
            _otherTeam = "Pacman";
        }
    }

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
        if (collision.gameObject.name != "Walls")
        {
            PhotonView photonView = collision.transform.GetComponent<PhotonView>();
            bool powerMode = (bool)photonView.Owner.CustomProperties["PowerMode"];

            if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman") && myTeamName=="Miss Pacman")
            {
                CheckPowerMode(powerMode, collision.gameObject);
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Miss Pacman") && myTeamName == "Pacman")
            {
                CheckPowerMode(powerMode, collision.gameObject);
            }
        }
    }

    void CheckPowerMode(bool powerMode, GameObject obj)
    {
        if (powerMode == true)
        {
            GameManager.instance.GhostEaten(_otherTeam, this.gameObject);
            CallRespawnRPC(this.gameObject);
        }
        else if (powerMode == false)
        {
            GameManager.instance.PacEaten(myTeamName, obj);
            CallRespawnRPC(obj);
        }
    }
}
