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

    //RPC
    private const string GAME_STARTED_RPC = nameof(GameStarted);
    private const string START_GAME_TIMER = nameof(Timer);
    const string ASK_SPAWN_POINT_RPC = nameof(AskSpawnPoint);
    const string SPAWN_PLAYER_CLIENT_RPC = nameof(SpawnPlayer);

    public bool hasGameStarted = false;
    [SerializeField] GameObject[] _characters;

    [Header("SpawnPoints")]
    [SerializeField] private SpawnPoint[] _spawnPoints;

    [Header("UI")]
    [SerializeField] Button _startGame;

    [Header("Players Controllers")]
    private List<Movement> playerControllers = new List<Movement>();
    private Movement localPlayerController;

    private bool isCountingForStartGame;
    private float timeLeftForStartGame = 0;

    private void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            photonView.RPC(ASK_SPAWN_POINT_RPC, RpcTarget.MasterClient);
            if (PhotonNetwork.IsMasterClient)
            {
                _startGame.interactable = true;
            }
        }
    }

    public void StartGame()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(START_GAME_TIMER, RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void GameStarted(PhotonMessageInfo info)
    {
        hasGameStarted = true;
        localPlayerController.canMove = true;
        isCountingForStartGame = false;
        Debug.Log("Game Started!!! WHOW");
    }

    [PunRPC]
    void Timer()
    {
        float time = 3;
        while(time>0)
        {
            time -= Time.deltaTime;
        }
        photonView.RPC(GAME_STARTED_RPC, RpcTarget.AllViaServer);
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
        localPlayerController = newLocalController;
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

        GameObject playerToSpawn = _characters[(int)PhotonNetwork.LocalPlayer.CustomProperties["Character"]];

        PhotonNetwork.Instantiate(playerToSpawn.name,
                    spawnPoint.transform.position,
                    spawnPoint.transform.rotation)
                .GetComponent<Movement>();

        for (int i = 0; i < takenSpawnPoints.Length; i++)
        {
            _spawnPoints[i].taken = takenSpawnPoints[i];
        }

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
}