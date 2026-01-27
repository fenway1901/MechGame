using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Video;

[System.Serializable]
public struct WeaponSlotEntry
{
    public string id;
    public WeaponSlot slot;
}

public class BaseMech : MonoBehaviour
{
    public List<WeaponSlotEntry> assignedWeapons = new();
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

        SetUpLimbs();

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

    public virtual void EquipWeapons()
    {
        List<BaseWeapons> remainingWeapons = new List<BaseWeapons>(weapons);

        foreach(BaseLimb limb in limbs)
        {
            LimbWeaponMounts mount = limb.mount;
            if (mount == null) continue;

            limb.GetHealthComponent().Damaged += (sender, amount, newCurrent) =>
            {
                float stutterSeconds = stats.Get(StatType.Mech_StutterTime);
                mount.StutterWeapon(stutterSeconds);
            };

            for(int w = remainingWeapons.Count - 1; w >= 0; --w)
            {
                if(mount.TryEquip(remainingWeapons[w]))
                    remainingWeapons.RemoveAt(w);
            }
        }
    }

    public virtual void GetWeapons()
    {
        if (assignedWeapons.Count == 0)
        {
            Debug.LogWarning(gameObject.name + " does not have any weapons in weaponsIDs, none will be loaded");
            return;
        }

        if(limbs.Count == 0)
        {
            Debug.LogWarning("Calling get weapon before getting limbs, weapons wont be assigned correclty");
        }

        List<string> ids = new List<string>();

        foreach(WeaponSlotEntry w in assignedWeapons)
        {
            ids.Add(w.id);
        }

        List<BaseWeapons> foundWeapons = GameManager.singleton.GetWeapons(ids);
        weapons = new List<BaseWeapons>();

        for(int i = 0; i < foundWeapons.Count; ++i)
        {
            weapons.Add(Instantiate(foundWeapons[i], transform));
            weapons[i]._AttachedMech = gameObject;

            // They are made in the same list so will stay in sync
            weapons[i].GetWeaponStats().SetSlot(assignedWeapons[i].slot);
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
    }

    #endregion
}
