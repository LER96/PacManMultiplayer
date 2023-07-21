using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PacmanMovement : Movement
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
        float angle = Mathf.Atan2(this._direction.y, this._direction.x);
        this.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward); 
            
        base.Update();
    }

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        base.OnPhotonInstantiate(info);
    }
}
