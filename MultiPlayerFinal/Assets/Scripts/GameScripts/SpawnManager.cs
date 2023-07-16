using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public static SpawnManager Instance { get; private set; }
    private const string GAME_STARTED_RPC = nameof(GameStarted);
    public bool hasGameStarted = false;

    [Header("SpawnPoints")]
    [SerializeField] private SpawnPoint[] spawnPoints;


    [Header("Players Controllers")]
    private List<Movement> playerControllers = new List<Movement>();
    private Movement localPlayerController;

    private bool isCountingForStartGame;
    private float timeLeftForStartGame = 0;

    private void Start()
    {

        photonView.RPC(GAME_STARTED_RPC, RpcTarget.AllViaServer);
    }

    [PunRPC]
    void GameStarted(PhotonMessageInfo info)
    {
        hasGameStarted = true;
        localPlayerController.canMove = true;
        isCountingForStartGame = false;
        Debug.Log("Game Started!!! WHOW");
    }

    public void SetPlayerController(Movement newLocalController)
    {
        localPlayerController = newLocalController;
    }

    public void AddPlayerController(Movement playerController)
    {
        playerControllers.Add(playerController);
    }


    //Spawn Player by his properties
    void SpawnPlayer(int spawnPointID, bool[] takenSpawnPoints)
    {
        SpawnPoint spawnPoint = GetSpawnPointByID(spawnPointID);
        PhotonNetwork.Instantiate("",
                    spawnPoint.transform.position,
                    spawnPoint.transform.rotation)
                .GetComponent<Movement>();

        for (int i = 0; i < takenSpawnPoints.Length; i++)
        {
            spawnPoints[i].taken = takenSpawnPoints[i];
        }

    }

    //Need to check if the property of the player is match to the spawn state
    private SpawnPoint GetSpawnPointByID(int targetID)
    {
        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint.ID == targetID)
                return spawnPoint;
        }

        return null;
    }


}
