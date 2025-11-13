using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseMech : BaseMech
{
    public GameObject target; // keep GameObject incase I want to make them target freindlies

    private void Awake()
    {
        target = GameObject.Find("Player Mech");
    }

    // Basic attack sequence

}
