using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;
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
    [HideInInspector] public List<BaseLimb> limbs;
    [HideInInspector] public BaseWeapons activeWeapon;
    public GameObject layoutPrefab;
    [HideInInspector] public GameObject spawnedLayout;

    public bool isDead = false;
    protected bool isMoving;

    public StatsComponent stats;
    public BuffController buffController;

    protected BaseHealthComponent healthComp;

    protected MechIconComponent iconComp;


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

        PopulateWeapons();

        if (layoutPrefab == null)
            Debug.LogError(gameObject.name + " layout is null, nothing will show!");

        iconComp = GetComponentInChildren<MechIconComponent>();
        healthComp.Died += iconComp.Died;
    }

    private void Update()
    {

    }


    #region Get Functions

    public BaseHealthComponent GetHealthComponent() { return healthComp; }
    public MechHealthComponent GetMechHealthComponent() { return healthComp as MechHealthComponent; }
    public List<BaseWeapons> GetWeapons() { return weapons; }

    public bool GetisMoving() { return isMoving; }

    #endregion


    #region Set Functions

    public void SetisMoving(bool state) { isMoving = state; }

    #endregion


    #region Stat Change



    #endregion


    #region Movement


    #endregion


    #region Weapon Management

    public virtual void EquipWeapons()
    {
        List<BaseWeapons> remainingWeapons = new List<BaseWeapons>(weapons);

        foreach(BaseLimb limb in limbs)
        {
            LimbWeaponMounts mount = limb.GetMount();
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

    protected virtual void PopulateWeapons()
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
            weapons[i].SetAttachedMech(gameObject);

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
        if (layoutPrefab.tag == "Player")
            spawnedLayout = Instantiate(layoutPrefab, CombatManager.instance.playerPanel.transform);
        else
            spawnedLayout = Instantiate(layoutPrefab, CombatManager.instance.enemyPanel.transform);
        
        spawnedLayout.transform.localPosition = new Vector3(spawnedLayout.transform.localPosition.x, -0.28f, spawnedLayout.transform.localPosition.z);

        healthComp = spawnedLayout.GetComponent<MechHealthComponent>();
        (healthComp as MechHealthComponent)._AttachedMech = gameObject;
        healthComp.Died += Died;
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


    #region Death Managment

    protected virtual void Died(BaseHealthComponent healthComp)
    {
        isDead = true;

        for (int i = 0; i < limbs.Count; ++i)
        {
            if (!limbs[i].isDestroyed)
            {
                limbs[i].DestroyLimb();
            }
        }
    }

    #endregion
}
