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

    #region Limb Management

    protected override void SetUpLimbs()
    {
        spawnedLayout = Instantiate(layoutPrefab, CombatManager.instance.enemyPanel.transform);
        spawnedLayout.GetComponent<MechHealthComponent>()._AttachedMech = gameObject;
        spawnedLayout.GetComponent<MechHealthComponent>().SetMaxHealth(stats.Get(StatType.Mech_MaxHealth));
        limbs = new List<BaseLimb>();

        for (int i = 0; i < spawnedLayout.transform.childCount; ++i)
        {
            // Just incase i will add things that arn't limbs to this prefab
            if (spawnedLayout.transform.GetChild(i).GetComponent<BaseLimb>())
            {
                limbs.Add(spawnedLayout.transform.GetChild(i).GetComponent<BaseLimb>());
                limbs[i]._AttachedMech = this;
            }
        }

        if (buffController != null)
            buffController.SetLimbs(limbs);
    }

    #endregion
}
