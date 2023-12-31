using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PacmanMovement : Movement
{
    Vector3 _otherPosition;
    Quaternion _otherRotation;

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

        if (photonView.IsMine)
        {
            float angle = Mathf.Atan2(this._direction.y, this._direction.x);
            this.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, _otherPosition, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, _otherRotation, 0.1f);
        }
            
        base.Update();
    }

    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            _otherPosition = (Vector3)stream.ReceiveNext();
            _otherRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        base.OnPhotonInstantiate(info);
    }

    public override void StartingPoint(Vector3 pos)
    {
        base.StartingPoint(pos);
    }
}
