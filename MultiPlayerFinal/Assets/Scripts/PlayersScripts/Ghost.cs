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
        if (collision.gameObject.name != "Walls")
        {
            PhotonView photonView = collision.transform.GetComponent<PhotonView>();
            teamName = (string)photonView.Owner.CustomProperties["Team"];
            bool powerMode = (bool)photonView.Owner.CustomProperties["PowerMode"];

            if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman") && !CompareTeam(teamName))
            {
                CheckPowerMode(powerMode, collision.gameObject);
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Miss Pacman") && !CompareTeam(teamName))
            {
                CheckPowerMode(powerMode, collision.gameObject);
            }
        }
    }

    void CheckPowerMode(bool powerMode, GameObject obj)
    {
        if (powerMode == true)
        {
            GameManager.instance.GhostEaten(teamName);
            HandleEaten(this.gameObject);
        }
        else
        {
            GameManager.instance.PacEaten(_myTeamName);
            HandleEaten(obj);
        }
    }

    IEnumerator HandleEaten(GameObject obj)
    {
        Movement mineView = obj.GetComponent<Movement>();
    
        mineView.transform.position = mineView.startingPosition;
        mineView.canMove = false;
        mineView.isSeen = false;
        yield return new WaitForSeconds(4f);

        mineView.canMove = true;
        mineView.isSeen = true;
    }

    bool CompareTeam(string team)
    {
        if (_myTeamName == team)
        {
            return true;
        }
        else
            return false;
    }
}
