using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatingPellets : MonoBehaviour
{
    [SerializeField] public int score = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pacman") || collision.CompareTag("MsPacman"))
        {
            Eat();
        }
    }

    public virtual void Eat()
    {
        GameManager.instance.EatenPellets(this);
    }
}
