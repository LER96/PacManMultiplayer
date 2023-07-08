using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Movement : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rb { get; set; }
    [SerializeField] LayerMask _obstaclesLayer;
    [SerializeField] float _speed = 8;
    [SerializeField] float _speedMultiplayer = 1;
    [SerializeField] float _score;
    [SerializeField] Vector2 initialDirection;
    Vector2 _direction { get; set; } 
    Vector2 _nextDirection { get; set; }
    Vector3 _startingPosition { get; set; }

    private void Awake()
    {
        this._rb = GetComponent<Rigidbody2D>();
        this._startingPosition = this.transform.position;
    }

    private void Start()
    {
        ResetAllStats();
    }

    private void Update()
    {
        if (this._nextDirection != Vector2.zero)
        {
            SetDirection(this._nextDirection);
        }
    }

    //moving player based on its current direction
    private void FixedUpdate()
    {
        Vector2 position = this._rb.position;
        Vector2 translatePosition = this._direction * _speed * _speedMultiplayer * Time.fixedDeltaTime;
        this._rb.MovePosition(position + translatePosition);
    }

    //resetting all players' stats
    public void ResetAllStats()
    {
        _speedMultiplayer = 1;
        _direction = initialDirection;
        _nextDirection = Vector2.zero;
        this.transform.position = this._startingPosition;
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
        Debug.Log("obstacle");

        //box cast so we wont go to a direction too early and collide with the sides
        RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, Vector2.one * 0.75f, 0, direction, 1.5f, _obstaclesLayer);
        return hit.collider != null;
    }
}
