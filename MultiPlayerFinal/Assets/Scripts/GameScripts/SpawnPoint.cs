using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPoint : MonoBehaviourPunCallbacks, IPunObservable
{
    public TeamState teamState;
    public CharacterState character;
    public int ID;
    public bool taken = false;

    [ContextMenu("Turn off taken")]
    void ChangeTaken()
    {
        taken = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(taken);
        }
        else if (stream.IsReading)
        {
            taken = (bool)stream.ReceiveNext();
        }
    }
}
