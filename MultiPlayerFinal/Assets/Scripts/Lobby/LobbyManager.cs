using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using System;

public class LobbyManager : GameManager
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
    [SerializeField] TMP_Dropdown dropDownNumberOfRounds;
    [SerializeField] Button _createTagRoomButton;
    [SerializeField] Button _createRoom;

    int _numberOfPlayers;
    int _currentNumOfPlayers;

    int _numberOfRounds;
    bool _numberOfPlayersCheck;
    bool _numberOfRoundsCheck;

    [Header("Join")]
    [SerializeField] GameObject joinRoomFather;
    [SerializeField] TMP_InputField joinRoomNameInputField;
    [SerializeField] TMP_Dropdown dropDownJoinList;
    [SerializeField] Button joinRoomButton;

    [Header("Info")]
    [SerializeField] Button startGameButton;
    [SerializeField] Button leaveRoomButton;
    [SerializeField] TextMeshProUGUI roomPlayersText;
    [SerializeField] TextMeshProUGUI playerListText;
    TextMeshProUGUI roomsListText;

    List<string> roomNames = new List<string>();
    List<string> _namesInGame = new List<string>();
    string startInput;
    List<RoomInfo> roomsInfo;

    //make start game button not interactable at the start of the game.
    private void Awake()
    {
        StartUI();

        startInput = nicknameInputField.text;
        startGameButton.interactable = false;
        leaveRoomButton.interactable = false;

        //Set Event on dropdown list
        dropDownJoinList.onValueChanged.AddListener(delegate { SetJoinInput(dropDownJoinList); });//Set the number of rooms that available
        dropDownPlayersNumberList.onValueChanged.AddListener(delegate { SetCreateInput(dropDownPlayersNumberList); });// set the number of players in a room
        dropDownNumberOfRounds.onValueChanged.AddListener(delegate { SetRounds(dropDownNumberOfRounds); });//set the number of rounds in game

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Update()
    {
        //If something is written in the input field
        if (nicknameInputField.text != "" || nicknameInputField.text != startInput)
        {
            enterRoomButton.interactable = true;
        }

        if (_numberOfRoundsCheck && _numberOfPlayersCheck)
        {
            _createRoom.interactable = true;
        }
        else
        {
            _createRoom.interactable = false;
        }
    }

    #region LogIn
    public void LoginToPhoton()
    {
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
        PhotonNetwork.NickName = nicknameInputField.text;
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
        _createTagRoomButton.interactable = false;
    }

    public void JoinTabMenu()
    {
        createRoomFather.SetActive(false);
        joinRoomFather.SetActive(true);
        joinRoomButton.interactable = false;
        _createTagRoomButton.interactable = true;
    }
    #endregion

    #region Create
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinRoomNameInputField.text);
    }

    public void CreateRoom()
    {
        //createRoomButton.interactable = false;
        bool sameName = false;
        foreach (string roomName in roomNames)
        {
            if (roomName == createRoomNameInputField.text)
            {
                sameName = true;
            }
        }

        //If there isn't a room with the same name, then cre
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

    //Set number Of players
    public void SetCreateInput(TMP_Dropdown dropdown)
    {
        int i = dropdown.value;
        if (dropdown.options[i].text != "None")
        {
            _numberOfPlayers = int.Parse(dropdown.options[i].text);
            _numberOfPlayersCheck = true;
        }
        else
        {
            _numberOfPlayersCheck = false;
        }
    }

    //Set number of Rounds
    public void SetRounds(TMP_Dropdown dropdown)
    {
        int i = dropdown.value;
        _numberOfRounds = int.Parse(dropdown.options[i].text);
        Debug.Log(_numberOfRounds);
        this.rounds = _numberOfRounds;
        _numberOfRoundsCheck = true;
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("We are in a room!");
        RefreshUI();
    }
    #endregion

    #region Join
    public void SetJoinInput(TMP_Dropdown dropdown)
    {
        int i = dropdown.value;
        joinRoomNameInputField.text = dropdown.options[i].text;
        //foreach (RoomInfo room in roomsInfo)
        //{
        //    if (room.Name == joinRoomNameInputField.text)
        //    {
        //        roomsListText.text = $"Players:{room.PlayerCount}/{room.MaxPlayers}";
        //        break;
        //    }
        //}
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
        leaveRoomButton.interactable = true;
    }
    #endregion

    #region Condition Rooms

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        leaveRoomButton.interactable = false;
        RefreshUI();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        _currentNumOfPlayers++;
        RefreshUI();

        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == _numberOfPlayers)
            {
                startGameButton.interactable = true;
            }
        }


    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        _numberOfPlayers--;
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
        _createTagRoomButton.interactable = true;
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
            }
        }
    }

    void ResetDrop()
    {
        dropDownJoinList.options.Clear();
        dropDownJoinList.options.Add(new TMP_Dropdown.OptionData() { text = "None" });
    }

    void RefreshUI()
    {
        if (PhotonNetwork.CountOfRooms > 0)
        {
            roomPlayersText.text = $"{_currentNumOfPlayers}/{_numberOfPlayers}";
        }
        else
        {
            roomPlayersText.text = $"{0}/{_numberOfPlayers}";
        }

        playerListText.text = "";
        foreach (Player photonPlayer in PhotonNetwork.PlayerList)
        {
            playerListText.text += $"{photonPlayer.NickName} In the Room" + Environment.NewLine;
        }

    }
}
