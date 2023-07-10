using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField nicknameInputField;

    [SerializeField] private Button joinRoomButton;

    List<string> _namesInGame = new List<string>();


    private void Update()
    {
        //If something is 
        if(nicknameInputField.text != string.Empty)
        {
            joinRoomButton.interactable = true;
        }
    }

    //On log in press, add to list of names, log in to photon
    void TryConnect()
    {
        if(SameName(nicknameInputField.text)==false)
        {
            _namesInGame.Add(nicknameInputField.text);
        }
        else
        {
            nicknameInputField.text = "";
            joinRoomButton.interactable = false;
        }
    }
    //Check if there is otherPlayers with the same name
    bool SameName(string currentName)
    {

        foreach(Player playerName in PhotonNetwork.PlayerList)
        {
            _namesInGame.Clear();
            _namesInGame.Add(playerName.NickName);
        }

        if (_namesInGame.Count > 0)
        {
            foreach (string name in _namesInGame)
            {
                if (currentName.Equals(name))
                {
                    return true;
                }
            }
        }
        else
            return true;

        return false;
    }
}
