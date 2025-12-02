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
    }


    protected void SetUpLimbs()
    {
        Debug.Log("setting up enemy layout");

        spawnedLayout = Instantiate(layoutPrefab, CombatManager.instance.enemyPanel.transform);

        limbs = new List<BaseLimb>();

        for (int i = 0; i < layoutPrefab.transform.childCount; ++i)
        {
            // Just incase i will add things that arn't limbs to this prefab
            if (layoutPrefab.transform.GetChild(i).GetComponent<BaseLimb>())
            {
                limbs.Add(layoutPrefab.transform.GetChild(i).GetComponent<BaseLimb>());
                limbs[i].attachedMech = this;
            }
        }
    }

    // Basic attack sequence

    public void SetObjective(Objective obj)
    {
        CurrentObjective = obj;
    }
}
