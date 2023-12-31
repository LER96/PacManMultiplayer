using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class Movement : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] Rigidbody2D _rb { get; set; }
    [SerializeField] PhotonView _photonView;
    [SerializeField] LayerMask _obstaclesLayer;
    [SerializeField] float _speed = 8;
    [SerializeField] float _speedMultiplayer = 1;
    [SerializeField] float _score;
    [SerializeField] Vector2 _initialDirection;

    public bool canMove;
    public bool isSeen;
    public string myTeamName;
    public const string RESPAWN_RPC = nameof(RespawnRpc);

    public Vector2 _direction { get; set; }
    public Vector3 startingPosition { get; set; }
    Vector2 _nextDirection { get; set; }

    private void Awake()
    {
        isSeen = true;
        this._rb = GetComponent<Rigidbody2D>();
        this._photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        ResetAllStats();
        StartingPoint(this.transform.position);
    }

    public void CallRespawnRPC(GameObject obj)
    {
        PhotonView objPhotonView = obj.GetComponent<PhotonView>();
        int objViewId = objPhotonView.ViewID;
        photonView.RPC(RESPAWN_RPC, RpcTarget.AllViaServer, objViewId);
    }

    [PunRPC]
    public void RespawnRpc(int objViewId)
    {
        PhotonView objPhotonView = PhotonNetwork.GetPhotonView(objViewId);
        GameObject obj = objPhotonView.gameObject;
        StartCoroutine(Respawn(obj));
    }

    public IEnumerator Respawn(GameObject obj)
    {
        PhotonView view = obj.GetComponent<PhotonView>();
        Movement mineView = view.GetComponent<Movement>();
        Collider2D collider = view.GetComponent<Collider2D>();
        mineView.canMove = false;
        mineView.transform.position = mineView.startingPosition;
        collider.enabled = false;
        yield return new WaitForSeconds(2f);

        mineView.canMove = true;
        collider.enabled = true;
    }

    public virtual void SetTeamName(string name)
    {
        myTeamName = name;
    }

    public virtual void Update()
    {
        if (this._nextDirection != Vector2.zero && _photonView.IsMine)
        {
            SetDirection(this._nextDirection);
        }
    }

    //moving player based on its current direction
    private void FixedUpdate()
    {
        if (canMove)
        {
            Vector2 position = this._rb.position;
            Vector2 translatePosition = this._direction * _speed * _speedMultiplayer * Time.deltaTime;
            this._rb.MovePosition(position + translatePosition);
        }
    }

    //resetting all players' stats
    public void ResetAllStats()
    {
        _speedMultiplayer = 1;
        _direction = _initialDirection;
        _nextDirection = Vector2.zero;
        this.transform.position = this.startingPosition;
    }

    //checks if player pressed on moving towards another direction, and waits to reach where it can enable this
    public void SetDirection(Vector2 direction, bool forced = false)
    {
        if (!IsDirectionOccupied(direction) || forced)
        {
            this._direction = direction;
            this._nextDirection = Vector2.zero;
        }
        else
        {
            this._nextDirection = direction;
        }
    }

    //checking if player can go there based on obstacle layer
    public bool IsDirectionOccupied(Vector2 direction)
    {
        //box cast so we wont go to a direction too early and collide with the sides
        RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, Vector2.one * 0.75f, 0, direction, 1.5f, _obstaclesLayer);
        return hit.collider != null;
    }

    public virtual void StartingPoint(Vector3 pos)
    {
        startingPosition = pos;
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
        }
    }

    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (info.photonView.IsMine)
        {
            SpawnManager.Instance.SetPlayerController(this);
        }
        SpawnManager.Instance.AddPlayerController(this);
    }
}
