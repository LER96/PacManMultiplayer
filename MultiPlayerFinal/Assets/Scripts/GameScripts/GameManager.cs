using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int _team;

    private void Awake()
    {
        instance = this;
    }
}
