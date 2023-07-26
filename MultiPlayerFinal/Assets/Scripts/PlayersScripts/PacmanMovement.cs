using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PacmanMovement : Movement
{
    private Vector3 otherPosition;
    private Quaternion otherRotation;

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
            transform.position = Vector3.Lerp(transform.position, otherPosition, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, otherRotation, 0.1f);
        }
            
        base.Update();
    }

    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isSeen);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            otherPosition = (Vector3)stream.ReceiveNext();
            otherRotation = (Quaternion)stream.ReceiveNext();
            isSeen = (bool)stream.ReceiveNext();
        }
        this.gameObject.SetActive(isSeen);
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
