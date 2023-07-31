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
    [SerializeField] GameObject _logInFather;
    [SerializeField] Button _logInButton;

    [Header("Input Name")]
    [SerializeField] GameObject _inputNameFather;
    [SerializeField] TMP_InputField _nicknameInputField;
    [SerializeField] Button _enterRoomButton;

    [Header("Create/Join Options")]
    [SerializeField] GameObject _creatOrJoinFather;

    [Header("Create")]
    [SerializeField] GameObject _createRoomFather;
    [SerializeField] TMP_InputField _createRoomNameInputField;
    [SerializeField] TMP_Dropdown _dropDownPlayersNumberList;
    [SerializeField] TMP_Dropdown _dropDownNumberOfRounds;
    [SerializeField] Button _createTabRoomButton;
    [SerializeField] Button _createRoom;

    int _numberOfPlayers;
    int _numberOfRounds = 1;
    bool _numberOfPlayersCheck;

    [Header("Join")]
    [SerializeField] GameObject _joinRoomFather;
    [SerializeField] TMP_InputField _joinRoomNameInputField;
    [SerializeField] TMP_Dropdown _dropDownJoinList;
    [SerializeField] Button _joinTabRoomButton;

    [Header("Filter ")]
    [SerializeField] GameObject _filterFather;
    [SerializeField] GameObject _roomsActiveFather;
    [SerializeField] TMP_Dropdown _dropDownRandomJoinList;
    [SerializeField] TMP_Dropdown _dropDownMaxPlayers;
    [SerializeField] Button _filterJoinTabButton;
    int _randomMaxPlayers;

    [Header("Info")]
    [SerializeField] Button _startGameButton;
    [SerializeField] Button _leaveRoomButton;
    [SerializeField] TextMeshProUGUI _roomPlayersText;
    [SerializeField] TextMeshProUGUI _playerListText;

    List<RoomInfo> _roomsInfo = new List<RoomInfo>();
    List<string> _roomNames = new List<string>();
    List<string> _namesInGame = new List<string>();
    string _startInput;

    //make start game button not interactable at the start of the game.
    private void Awake()
    {
        StartUI();

        this.rounds = _numberOfRounds;
        _startInput = _nicknameInputField.text;
        _startGameButton.interactable = false;
        _leaveRoomButton.interactable = false;

        //Set Event on dropdown list
        _dropDownJoinList.onValueChanged.AddListener(delegate { SetJoinInput(_dropDownJoinList); });//Set the number of rooms that available
        _dropDownPlayersNumberList.onValueChanged.AddListener(delegate { SetCreateInput(_dropDownPlayersNumberList); });// set the number of players in a room
        _dropDownNumberOfRounds.onValueChanged.AddListener(delegate { SetRoundsDropDown(_dropDownNumberOfRounds); });//set the number of rounds in game

        //Random DropDowns
        _dropDownMaxPlayers.onValueChanged.AddListener(delegate { SetRandomMaxPlayers(_dropDownMaxPlayers); });
        _dropDownRandomJoinList.onValueChanged.AddListener(delegate { SetRandomInput(_dropDownRandomJoinList); });

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Update()
    {
        //If something is written in the input field
        if (_nicknameInputField.text != "" || _nicknameInputField.text != _startInput)
        {
            _enterRoomButton.interactable = true;
        }

        if (_numberOfPlayersCheck)
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
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();

        //Disable login and activate input
        _logInFather.SetActive(false);
        _inputNameFather.SetActive(true);
    }

    //On log in press, add to list of names, log in to photon
    public void TryConnect()
    {
        PhotonNetwork.NickName = _nicknameInputField.text;
        if (SameName(_nicknameInputField.text) == false)
        {
            _namesInGame.Add(_nicknameInputField.text);

            _inputNameFather.SetActive(false);
            _creatOrJoinFather.SetActive(true);
            CreateTabMenu();
        }
        else
        {
            _nicknameInputField.text = "";
            _enterRoomButton.interactable = false;
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
        _logInFather.SetActive(true);
        _inputNameFather.SetActive(false);
        _creatOrJoinFather.SetActive(false);
    }

    public void CreateTabMenu()
    {
        _createRoomFather.SetActive(true);
        _joinRoomFather.SetActive(false);
        _joinTabRoomButton.interactable = true;
        _createTabRoomButton.interactable = false;
        _filterJoinTabButton.interactable = true;
    }

    public void JoinTabMenu()
    {
        _createRoomFather.SetActive(false);
        _joinRoomFather.SetActive(true);
        _filterFather.SetActive(false);
        _roomsActiveFather.SetActive(true);

        _joinTabRoomButton.interactable = false;
        _createTabRoomButton.interactable = true;
        _filterJoinTabButton.interactable = true;
    }

    public void FilterTabMenu()
    {
        _createRoomFather.SetActive(false);
        _joinRoomFather.SetActive(true);
        _filterFather.SetActive(true);
        _roomsActiveFather.SetActive(false);

        _joinTabRoomButton.interactable = true;
        _createTabRoomButton.interactable = true;
        _filterJoinTabButton.interactable = false;
    }

    #endregion

    #region Create
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(_joinRoomNameInputField.text);
    }

    public void CreateRoom()
    {
        bool sameName = false;
        foreach (string roomName in _roomNames)
        {
            if (roomName == _createRoomNameInputField.text)
            {
                sameName = true;
            }
        }

        //If there isn't a room with the same name, then cre
        if (sameName == false)
        {
            PhotonNetwork.CreateRoom(_createRoomNameInputField.text, new RoomOptions() { MaxPlayers = _numberOfPlayers, EmptyRoomTtl = 2000 },
                null);
            _dropDownJoinList.options.Add(new TMP_Dropdown.OptionData() { text = _createRoomNameInputField.text });
        }
        else
        {
            _createRoomNameInputField.text = "";
        }

    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("We are in a room!");
        roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Rounds", _numberOfRounds } });
        RefreshUI();
    }

    #endregion

    #region DropDown
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
    public void SetRoundsDropDown(TMP_Dropdown dropdown)
    {
        int i = dropdown.value;
        _numberOfRounds = int.Parse(dropdown.options[i].text);
        Debug.Log(_numberOfRounds);
    }

    public void SetRandomInput(TMP_Dropdown dropdown)
    {
        RefreshUI();
        int i = dropdown.value;
        _joinRoomNameInputField.text = dropdown.options[i].text;

        RefreshUI();
    }

    public void SetRandomMaxPlayers(TMP_Dropdown dropdown)
    {
        int i = dropdown.value;
        _randomMaxPlayers = int.Parse(dropdown.options[i].text);
        RefreshUI();
    }
    #endregion

    #region Join

    public void SetJoinInput(TMP_Dropdown dropdown)
    {
        int i = dropdown.value;
        _joinRoomNameInputField.text = dropdown.options[i].text;
        RefreshUI();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        RefreshUI();
        _joinTabRoomButton.interactable = false;
        _leaveRoomButton.interactable = true;
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
        PhotonNetwork.JoinLobby();
        _joinTabRoomButton.interactable = true;
        _leaveRoomButton.interactable = false;
        RefreshUI();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        RefreshUI();

        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == _numberOfPlayers)
            {
                _startGameButton.interactable = true;
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
        _startGameButton.interactable = false;
        RefreshUI();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        _createTabRoomButton.interactable = true;
    }

    #endregion

    #region On Rooms Update
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        _roomsInfo.Clear();
        _roomsInfo = roomList;
        base.OnRoomListUpdate(roomList);
        ManageRooms(roomList);
    }

    //Clear the rooms list and orginize available rooms
    void ManageRooms(List<RoomInfo> roomList)
    {
        ResetDrop();
        UpdateAvailableRooms(roomList);
    }

    //Reset rooms list
    void ResetDrop()
    {
        _roomNames.Clear();
        _dropDownJoinList.options.Clear();
        _dropDownRandomJoinList.options.Clear();

        _dropDownRandomJoinList.options.Add(new TMP_Dropdown.OptionData() { text = "None" });
        _dropDownJoinList.options.Add(new TMP_Dropdown.OptionData() { text = "None" });
    }

    //Orginize the room list drop down, checking if the current room isn't full
    void UpdateAvailableRooms(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            if (!room.RemovedFromList)
            {
                if (room.PlayerCount < room.MaxPlayers)
                {
                    _roomNames.Add(room.Name);
                    _dropDownJoinList.options.Add(new TMP_Dropdown.OptionData() { text = room.Name });
                    
                    if(room.MaxPlayers== _randomMaxPlayers)
                    {
                        _dropDownRandomJoinList.options.Add(new TMP_Dropdown.OptionData() { text = room.Name });
                    }
                    
                }
            }
            else
            {
                if (_roomNames.Contains(room.Name))
                {
                    _roomNames.Remove(room.Name);
                }
            }
        }
    }
    #endregion

    void RefreshUI()
    {
        ManageRooms(_roomsInfo);
        _playerListText.text = "";
        if (PhotonNetwork.InRoom)
        {
            foreach (Player photonPlayer in PhotonNetwork.PlayerList)
            {
                _playerListText.text += $"{photonPlayer.NickName} In the Room" + Environment.NewLine;
            }
        }
    }

    //Replace masterClient
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
    }
}
