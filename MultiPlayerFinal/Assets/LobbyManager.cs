using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using System;

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
    [SerializeField] TMP_InputField createRoomNameInputField;
    [SerializeField] TMP_Dropdown dropDownPlayersNumberList;
    [SerializeField] Button createRoomButton;
    int _numberOfPlayers;

    [Header("Join")]
    [SerializeField] GameObject joinRoomFather;
    [SerializeField] TMP_InputField joinRoomNameInputField;
    [SerializeField] TMP_Dropdown dropDownJoinList;
    [SerializeField] Button joinRoomButton;

    [Header("Info")]
    [SerializeField] Button startGameButton;
    [SerializeField] TMP_Text roomPlayersText;


    List<string> roomNames = new List<string>();
    List<string> _namesInGame = new List<string>();
    string startInput;
    List<RoomInfo> roomsInfo;
    TextMeshProUGUI roomsListText;

    private void Awake()
    {
        StartUI();

        startInput = nicknameInputField.text;

        dropDownJoinList.onValueChanged.AddListener(delegate { SetJoinInput(dropDownJoinList); });
        dropDownPlayersNumberList.onValueChanged.AddListener(delegate { SetCreateInput(dropDownPlayersNumberList); });

        PhotonNetwork.AutomaticallySyncScene = true;
    }


    private void Update()
    {
        //If something is written in the input field
        if (nicknameInputField.text != "" || nicknameInputField.text != startInput)
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
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("<color=#00ff00>We are connected!</color>");
        PhotonNetwork.JoinLobby();

        //Disable login and activate input
        logInFather.SetActive(false);
        inputNameFather.SetActive(true);
    }

    //On log in press, add to list of names, log in to photon
    public void TryConnect()
    {
        if (SameName(nicknameInputField.text) == false)
        {
            _namesInGame.Add(nicknameInputField.text);

            inputNameFather.SetActive(false);
            creatOrJoinFather.SetActive(true);
            CreateTabMenu();
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
        foreach (Player playerName in PhotonNetwork.PlayerList)
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

    #region Create/Join UI
    void StartUI()
    {
        logInFather.SetActive(true);
        inputNameFather.SetActive(false);
        creatOrJoinFather.SetActive(false);
    }

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

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        roomsInfo = roomList;
        Debug.Log("Got room list");
        base.OnRoomListUpdate(roomList);
        //roomsListText.text = string.Empty;
        roomNames.Clear();
        ResetDrop();
        foreach (RoomInfo roomInfo in roomList)
        {
            if (!roomInfo.RemovedFromList)
            {
                roomNames.Add(roomInfo.Name);
                dropDownJoinList.options.Add(new TMP_Dropdown.OptionData() { text = roomInfo.Name });
                //roomsListText.text += $"{roomInfo.Name}: {roomInfo.PlayerCount}/{roomInfo.MaxPlayers}" + Environment.NewLine;
            }
            else
            {
                if (roomNames.Contains(roomInfo.Name))
                {
                    roomNames.Remove(roomInfo.Name);
                }
                //Debug.Log("Room " + roomInfo.Name + " No longer exist");
            }
        }
    }

    void ResetDrop()
    {
        dropDownJoinList.options.Clear();
        dropDownJoinList.options.Add(new TMP_Dropdown.OptionData() { text = "None" });
    }

    #region Creat
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinRoomNameInputField.text);
    }

    public void CreateRoom()
    {
        createRoomButton.interactable = false;
        bool sameName = false;
        foreach (string roomName in roomNames)
        {
            if (roomName == createRoomNameInputField.text)
            {
                sameName = true;
            }
        }
        if (sameName == false)
        {
            PhotonNetwork.CreateRoom(createRoomNameInputField.text, new RoomOptions() { MaxPlayers = _numberOfPlayers, EmptyRoomTtl = 0 },
                null);
            dropDownJoinList.options.Add(new TMP_Dropdown.OptionData() { text = createRoomNameInputField.text });
        }
        else
        {
            createRoomNameInputField.text = "";
        }
    }

    void SetCreateInput(TMP_Dropdown dropdown)
    {
        int i = dropdown.value;
        _numberOfPlayers = int.Parse(dropdown.options[i].text);
        RefreshUI();
        //roomsListText.text = $"In Room:{room.PlayerCount}/{room.MaxPlayers}";
    }
    #endregion

    #region Join
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("We are in a room!");
        RefreshUI();
    }

    void SetJoinInput(TMP_Dropdown dropdown)
    {
        int i = dropdown.value;
        joinRoomNameInputField.text = dropdown.options[i].text;
        foreach (RoomInfo room in roomsInfo)
        {
            if (room.Name == joinRoomNameInputField.text)
            {
                roomsListText.text = $"Players:{room.PlayerCount}/{room.MaxPlayers}";
                break;
            }
        }
        RefreshUI();
        //roomsListText.text = $"In Room:{room.PlayerCount}/{room.MaxPlayers}";
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined Room!");
        RefreshUI();
        //RefreshCurrentRoomInfoUI();
        //isConnectedToRoomDebugTextUI.text = YES_STRING;
        //leaveRoomButton.interactable = true;
    }
    #endregion

    #region Condition Rooms
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        RefreshUI();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == _numberOfPlayers)
            {
                startGameButton.interactable = true;
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount < _numberOfPlayers)
            {
                startGameButton.interactable = false;
            }
        }
        RefreshUI();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.LogError("Create failed..." + Environment.NewLine + message);
        createRoomButton.interactable = true;
    }

    #endregion

    void RefreshUI()
    {
        roomPlayersText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{_numberOfPlayers}";
    }
}
