using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PacmanMovement : Movement
{
    private PhotonTransformView transformView;
    private void Start()
    {
        transformView = GetComponent<PhotonTransformView>();
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
        float angle = Mathf.Atan2(this._direction.y, this._direction.x);
        transformView.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward); 
            
        base.Update();
    }

    //public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        stream.SendNext(transform.position);
    //        stream.SendNext(transform.rotation);
    //    }
    //    else
    //    {
    //        photonView.transform.position = (Vector3)stream.ReceiveNext();
    //        photonView.transform.rotation = (Quaternion)stream.ReceiveNext();
    //    }
    //}

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        base.OnPhotonInstantiate(info);
    }
}
