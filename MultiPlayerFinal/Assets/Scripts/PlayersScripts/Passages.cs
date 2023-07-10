using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passages : MonoBehaviour
{
    [SerializeField] Transform _connection;

    //depending on each collision we hit, we give it, we transform to the opposite game object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector3 position = collision.transform.position;

        position.x = this._connection.transform.position.x;
        position.y = this._connection.transform.position.y;
        collision.transform.position = position;
    }
}
