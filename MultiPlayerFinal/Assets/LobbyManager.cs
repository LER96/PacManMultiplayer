using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Log In")]
    [SerializeField] GameObject logInFather;
    [SerializeField] private Button logInButton;

    [Header("Input Name")]
    [SerializeField] GameObject inputNameFather;
    [SerializeField] private TMP_InputField nicknameInputField;
    [SerializeField] private Button enterRoomButton;

    [Header("Create/Join Options")]
    [SerializeField] GameObject creatOrJoinFather;

    [Header("Create")]
    [SerializeField] GameObject createRoomFather;
    [SerializeField] private Button createRoomButton;

    [Header("Join")]
    [SerializeField] GameObject joinRoomFather;
    [SerializeField] private Button joinRoomButton;

    List<string> _namesInGame = new List<string>();
    string startInput;

    private void Awake()
    {
        inputNameFather.SetActive(false);
        startInput = nicknameInputField.text;
    }

    private void Update()
    {
        //If something is written in the input field
        if(nicknameInputField.text != "" || nicknameInputField.text== startInput)
        {
            enterRoomButton.interactable = true;
        }
    }

    #region LogIn
    public void LoginToPhoton()
    {
        //PhotonNetwork.NickName = nicknameInputField.text;
        //Debug.Log("Player nickname is " + PhotonNetwork.NickName);
        PhotonNetwork.ConnectUsingSettings();

        //Disable login and activate input
        logInFather.SetActive(false);
        inputNameFather.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("<color=#00ff00>We are connected!</color>");
        PhotonNetwork.JoinLobby();
    }

    //On log in press, add to list of names, log in to photon
    public void TryConnect()
    {
        if(SameName(nicknameInputField.text)==false)
        {
            _namesInGame.Add(nicknameInputField.text);

            inputNameFather.SetActive(false);
            creatOrJoinFather.SetActive(true);
        }
        else
        {
            nicknameInputField.text = "";
            enterRoomButton.interactable = false;
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
            return false;

        return false;
    }
    #endregion

    #region Create/Join
    public void CreateTabMenu()
    {
        createRoomFather.SetActive(true);
        joinRoomFather.SetActive(false);
        joinRoomButton.interactable = true;
        createRoomButton.interactable = false;
    }

    public void JoinTabMenu()
    {
        createRoomFather.SetActive(false);
        joinRoomFather.SetActive(true);
        joinRoomButton.interactable = false;
        createRoomButton.interactable = true;
    }
    #endregion
}
