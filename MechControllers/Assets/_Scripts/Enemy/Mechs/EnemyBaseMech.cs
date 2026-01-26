using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBaseMech : BaseMech
{
    public GameObject target; // keep GameObject incase I want to make them target freindlies

    public Objective CurrentObjective;


    private void Awake()
    {
        target = GameObject.Find("Player Mech");
    }

    public override void Init()
    {
        base.Init();

        SetUpLimbs();

        brain = GetComponent<MechBrain>();

        if (brain == null)
            Debug.LogError(name + " can not find or does not have MechBrain on same object");

        brain.mech = this;
    }

    // Basic attack sequence

    public void SetObjective(Objective obj)
    {
        CurrentObjective = obj;
        brain.currentObjective = obj;
    }
}
