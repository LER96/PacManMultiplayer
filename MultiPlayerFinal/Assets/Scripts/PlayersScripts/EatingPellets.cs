using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatingPellets : MonoBehaviour
{
    [SerializeField] Transform _pellets;
    [SerializeField] float _points = 10;
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pacman") || collision.CompareTag("MsPacman"))
        {
            Eat();
        }
    }

    public void Eat()
    {
        this.gameObject.SetActive(false);
        
        //implement this later
        //if (!RemainingPellets())
        //{
        //    //game over
        //}
    }

    public bool RemainingPellets()
    {
        foreach (Transform pellets in this._pellets)
        {
            if (pellets.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }
}
