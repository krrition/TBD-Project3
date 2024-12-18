using System;
using UnityEngine;

public class StoredController : MonoBehaviour
{
    public bool TurnP2;

    public GameObject GM;

    public bool roundSwitch;

    public int Rnd1Nest;
    public int Rnd2Nest;
    public int Rnd1Crown;
    public int Rnd2Crown;
    public int Rnd1Tnt;
    public int Rnd2Tnt;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);

    }

    private void Update()
    {
        if (roundSwitch && !TurnP2)
        {
            TurnP2 = true;
        }
    }

    /*GM = GameObject.Find("GameManager");
}

// Update is called once per frame
void Update()
{
    if (GM == null) GM = GameObject.Find("GameManager");
}*/
}
