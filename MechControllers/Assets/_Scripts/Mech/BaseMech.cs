using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class BaseMech : MonoBehaviour
{
    public List<string> weaponIDs = new List<string>();
    protected List<BaseWeapons> weapons;
    public List<BaseLimb> limbs;
    public BaseWeapons activeWeapon;
    public GameObject layoutPrefab;
    public GameObject spawnedLayout;

    public bool isDead = false;

    public StatsComponent stats;
    public BuffController buffController;

    // AI variables
    public IReadOnlyList<BaseWeapons> Weapons => weapons;
    protected MechBrain brain;

    public virtual void Init()
    {
        if(TryGetComponent(out BuffController controller))
            buffController = controller;

        if (TryGetComponent(out StatsComponent stats))
            this.stats = stats;
        else
            Debug.LogWarning(name + " Does not have any stats!");

            GetWeapons();

        if (layoutPrefab == null)
            Debug.LogError(gameObject.name + " layout is null, nothing will show!");
    }

    private void Update()
    {

    }


    #region Get Info



    #endregion


    #region Movement


    #endregion


    #region Weapon Management

    public virtual void GetWeapons()
    {
        if (weaponIDs.Count == 0)
        {
            Debug.LogWarning(gameObject.name + " does not have any weapons in weaponsIDs, none will be loaded");
            return;
        }

        List<BaseWeapons> foundWeapons = GameManager.singleton.GetWeapons(weaponIDs);
        weapons = new List<BaseWeapons>();

        for(int i = 0; i < foundWeapons.Count; ++i)
        {
            weapons.Add(Instantiate(foundWeapons[i], transform));
            weapons[i]._AttachedMech = gameObject;
        }
    }

    protected virtual void TargetSelected(GameObject target)
    {
        activeWeapon.Attack(target);
        //AttackManager.instance.AttackEnemy(target, activeWeapon);
    }

    #endregion


    #region Limb Management

    protected virtual void SetUpLimbs()
    {
        //Debug.Log("setting up enemy layout");

        spawnedLayout = Instantiate(layoutPrefab, CombatManager.instance.enemyPanel.transform);
        spawnedLayout.GetComponent<MechHealthComponent>()._AttachedMech = gameObject;
        spawnedLayout.GetComponent<MechHealthComponent>().SetMaxHealth(stats.Get(StatType.Mech_MaxHealth));
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

        if (buffController != null)
            buffController.SetLimbs(limbs);
    }

    #endregion
}
