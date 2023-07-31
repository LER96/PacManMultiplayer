using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public static SpawnManager Instance { get; private set; }

    [SerializeField] GameObject[] _characters;

    //RPC
    private const string GAME_STARTED_RPC = nameof(GameStarted);
    private const string START_GAME_TIMER = nameof(Timer);
    const string ASK_SPAWN_POINT_RPC = nameof(AskSpawnPoint);
    const string SPAWN_PLAYER_CLIENT_RPC = nameof(SpawnPlayer);

    public bool hasGameStarted = false;

    [Header("SpawnPoints")]
    [SerializeField] private SpawnPoint[] _spawnPoints;
    string _characterName;
    string _characterTeam;

    [Header("UI")]
    [SerializeField] Button _startGame;
    [SerializeField] GameObject _canvasStartGame;

    [Header("Players Controllers")]
    [SerializeField] List<Movement> playerControllers = new List<Movement>();
    Movement _localPlayerController;

    bool _isCountingForStartGame;
    float _timeLeftForStartGame = 0;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (_isCountingForStartGame)
        {
            if (_timeLeftForStartGame > 0)
            {
                _timeLeftForStartGame -= Time.deltaTime;
            }
            else
            {
                photonView.RPC(GAME_STARTED_RPC, RpcTarget.AllViaServer);
                _isCountingForStartGame = false;
            }
        }
    }

    private void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _startGame.interactable = true;
                _canvasStartGame.SetActive(true);
            }
            else
            {
                _canvasStartGame.SetActive(false);
            }

            photonView.RPC(ASK_SPAWN_POINT_RPC, RpcTarget.MasterClient);
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(START_GAME_TIMER, RpcTarget.MasterClient);
            _canvasStartGame.SetActive(false);
        }

    }

    [PunRPC]
    void GameStarted(PhotonMessageInfo info)
    {
        hasGameStarted = true;
        _localPlayerController.canMove = true;
        _isCountingForStartGame = false;
    }

    [PunRPC]
    void Timer(PhotonMessageInfo info)
    {
        _isCountingForStartGame = true;
        _timeLeftForStartGame = 3;
    }

    [PunRPC]
    void AskSpawnPoint(PhotonMessageInfo messageInfo)
    {
        List<SpawnPoint> availableSpawnPoints = new List<SpawnPoint>();

        foreach (SpawnPoint spawnPoint in _spawnPoints)
        {
            if (!spawnPoint.taken)
                availableSpawnPoints.Add(spawnPoint);
        }

        SpawnPoint chosenSpawnPoint =
            availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];
        chosenSpawnPoint.taken = true;

        bool[] takenSpawnPoints = new bool[_spawnPoints.Length];
        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            takenSpawnPoints[i] = _spawnPoints[i].taken;
        }
        photonView.RPC(SPAWN_PLAYER_CLIENT_RPC,
            messageInfo.Sender, chosenSpawnPoint.ID,
            takenSpawnPoints);
    }

    public void SetPlayerController(Movement newLocalController)
    {
        _localPlayerController = newLocalController;
    }

    public void AddPlayerController(Movement playerController)
    {
        playerControllers.Add(playerController);
    }

    [PunRPC]
    //Spawn Player by his properties
    void SpawnPlayer(int spawnPointID, bool[] takenSpawnPoints)
    {
        SpawnPoint spawnPoint = GetSpawnPointByID(spawnPointID);

        string characterName = (string)PhotonNetwork.LocalPlayer.CustomProperties["Character"];
        _characterTeam = (string)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
        GameObject playerToSpawn = null;

        foreach (GameObject characterPrefab in _characters)
        {
            if (characterPrefab.name == characterName)
            {
                playerToSpawn = characterPrefab;
            }
        }

        SetPlayerControllerByType(playerToSpawn, spawnPoint);

        for (int i = 0; i < takenSpawnPoints.Length; i++)
        {
            _spawnPoints[i].taken = takenSpawnPoints[i];
        }
    }

    //Check if character is Pac or Ghost Type and set it's own controller
    void SetPlayerControllerByType(GameObject playerToSpawn, SpawnPoint spawnPoint)
    {
        if (playerToSpawn.name == "Pacman" || playerToSpawn.name == "Miss Pacman")
        {
            _localPlayerController = PhotonNetwork.Instantiate(playerToSpawn.name,
                        spawnPoint.transform.position,
                        spawnPoint.transform.rotation)
                    .GetComponent<PacmanMovement>();
        }
        else
        {
            _localPlayerController = PhotonNetwork.Instantiate(playerToSpawn.name,
                        spawnPoint.transform.position,
                        spawnPoint.transform.rotation)
                    .GetComponent<Ghost>();
        }

        _localPlayerController.SetTeamName(_characterTeam);
        _localPlayerController.StartingPoint(spawnPoint.transform.position);
        AddPlayerController(_localPlayerController);
    }

    //Need to check if the property of the player is match to the spawn state
    private SpawnPoint GetSpawnPointByID(int targetID)
    {
        foreach (SpawnPoint spawnPoint in _spawnPoints)
        {
            if (spawnPoint.ID == targetID)
                return spawnPoint;
        }

        return null;
    }

    public void RespawnAllPlayers()
    {
        foreach(Movement controller in playerControllers)
        {
            controller.CallRespawnRPC(controller.gameObject);
        }
    }

}
