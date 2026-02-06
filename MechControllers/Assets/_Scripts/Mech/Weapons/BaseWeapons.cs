using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEngine.Video;

public class BaseWeapons : MonoBehaviour
{
    [Header("General Variables")]
    [SerializeField] public string displayName;
    [SerializeField] protected Sprite icon;
    [SerializeField] MonoBehaviour hitEffectBehaviour;
    protected IHitEffect hitEffect;
    [SerializeField] protected float baseDamage;
    [SerializeField] protected float baseRange;
    [SerializeField] protected float baseAttackSpeed;
    [SerializeField] protected float baseCooldown;

    [HideInInspector] public GameObject _AttachedMech;
    protected BaseMech mechComp;

    [SerializeField] protected bool canMoveWhileCharging;
    [SerializeField] protected bool isAttacking;
    
    protected bool isCharging;
    protected bool isCoolingDown;
    protected float chargeEndTime;
    protected float cooldownEndTime;

    protected GameObject target;
    protected Transform targetTrans;

    [Header("Ranged Variables (usesAmmo REMEMBER usesAmmo)")]
    public bool usesAmmo;
    protected bool reloading;
    [SerializeField] protected int maxAmmo;
    protected int currentAmmo;
    [SerializeField] protected int ammoUsedPerShot;
    [SerializeField] protected float reloadTime;
    [SerializeField] protected int baseReloadAmount;
    protected float reloadEndTime;

    public float BaseDamage => baseDamage;
    public float BaseRange => baseRange;
    public float BaseAttackSpeed => baseAttackSpeed;
    public float BaseCooldown => baseCooldown;
    public float BaseReloadTime => reloadTime;
    public int BaseMaxAmmo => maxAmmo;
    public int BaseAmmoUsedPerShot => ammoUsedPerShot;
    public int BaseReloadAmount => baseReloadAmount;

    [SerializeField] protected BaseWeaponStats weaponStats;


    public event Action<BaseWeapons, float> WeaponCharging;
    public event Action<BaseWeapons, float> WeaponCooling;
    public event Action<BaseWeapons, float> AmmoFired;
    public event Action<BaseWeapons, float> Reloaded;
    public event Action<BaseWeapons, float> Reloading;
    public event Action<BaseWeapons> CancelAttack;

    //[Header("Melee Variables")]


    #region Unity Functions

    protected virtual void Awake()
    {
        hitEffect = hitEffectBehaviour as IHitEffect;

        if (hitEffect == null)
        {
            Debug.LogError(
                $"{name}: hitEffectBehaviour is assigned but does NOT implement IHitEffect. " +
                $"Assigned type: {hitEffectBehaviour?.GetType().Name}"
            );
        }

        weaponStats = GetComponent<BaseWeaponStats>();
        if(weaponStats == null)
        {
            Debug.LogError("The weapon " + name + " did not have a WeaponStat Component!");
            weaponStats = gameObject.AddComponent<BaseWeaponStats>();
        }
    }

    protected virtual void Start()
    {
        // Might cause problems in future keep an eye on this
        if(usesAmmo)
            currentAmmo = weaponStats.MaxAmmo;
    }

    protected virtual void Update()
    {
        // Reload and attack timers are in here incase of a lag spike wont delay the attack but keep it as close as possible

        // Reloading stuff here
        if (reloading)
        {
            if(Time.time >= reloadEndTime)
                FinishedReload();
        }

        // Attack Stuff here
        if (isAttacking)
        {
            if (target == null)
            {
                Debug.LogWarning("Attacking but target is null");
                return;
            }

            if (isCharging)
            {
                if (!canMoveWhileCharging && mechComp.GetisMoving())
                {
                    StopAttack("Mech moved during charging");
                }

                // Time to damage enemy
                if(Time.time >= chargeEndTime)
                {
                    if(GameUtils.GetDistance(_AttachedMech.transform.position, targetTrans.position) > weaponStats.Range)
                    {
                        StopAttack("Distance to far: " + GameUtils.GetDistance(_AttachedMech.transform.position, targetTrans.position));
                        return;
                    }

                    if (usesAmmo)
                    {
                        if (currentAmmo == 0 || currentAmmo < ammoUsedPerShot)
                        {
                            StopAttack("Not enough ammo to shoot " + currentAmmo);
                            return;
                        }

                        FiredWeapon();
                    }

                    Debug.Log(name + " finished charging and applying damage");
                    //target.GetComponent<BaseHealthComponent>().TakeDamage(baseDamage);
                    hitEffect.Apply(this, target, targetTrans.position);
                    cooldownEndTime = Time.time + weaponStats.Cooldown;
                    isCharging = false;
                    isCoolingDown = true;

                    WeaponCooling?.Invoke(this, weaponStats.Cooldown);

                    // PROTO: Display controled like this for a minute
                    // In future make it a unity event the UI is subcribed to
                    //if (transform.parent.tag == "Player")
                    //    AttackManager.instance.CooldownDisplay(weaponStats.Cooldown);
                }
            }
            else if (isCoolingDown)
            {
                // Cooling off weapon
                if(Time.time >= cooldownEndTime)
                {
                    //Debug.Log(name + " finshed attack");

                    // PROTO: Display controled like this for a minute
                    // In future make it a unity event the UI is subcribed to
                    //if (transform.parent.tag == "Player")
                    //    AttackManager.instance.TurnOffDisplay();

                    FinishedAttack();
                }
            }
        }
    }

    #endregion


    #region Stat Controls

    // If i want a permanent upgrade to a weapon
    public virtual void AddToDamage(float add)
    {
        baseDamage += add;
        weaponStats.Stats.SetBase(StatType.Weapon_Damage, baseDamage);
    }

    public virtual void ReduceFromDamage(float reduce)
    {
        baseDamage -= reduce;
        baseDamage = Mathf.Clamp(baseDamage, 0, Mathf.Infinity);
    }

    #endregion


    #region Set Functions

    public void SetIsAttacking(bool attack) { isAttacking = attack; }
    public void SetIsRelaoding(bool reload) { reloading = reload; }
    public void SetAttachedMech(GameObject mech)
    {
        _AttachedMech = mech;
        mechComp = mech.GetComponent<BaseMech>();
    }

    #endregion


    #region Get Functions

    // float gets
    public float GetDamage() { return weaponStats.Damage; }
    public float GetRange() { return weaponStats.Range; }
    public float GetAttackSpeed() { return weaponStats.AttackSpeed; }
    public float GetCoolDown() { return weaponStats.Cooldown; }

    // int gets
    //public int GetMaxAmmo() { return weaponStats.; }
    public int GetCurrentAmmo() { return currentAmmo; }
    public int GetAmmoUsedPerShot() { return ammoUsedPerShot; }
    public int GetMaxAmmo() { return maxAmmo; }

    // bool gets
    public bool GetIsAttacking() { return isAttacking; }
    public bool GetIsReloading() { return reloading; }
    public bool GetIsCharging() { return isCharging; }
    public bool GetIsCoolingDown() { return isCoolingDown; }


    public BaseWeaponStats GetWeaponStats() { return weaponStats; }
    public Sprite GetIcon() { return icon; }

    #endregion


    #region Attack Functions

    public virtual void Attack(GameObject target)
    {
        if (target == null)
        {
            Debug.LogError(transform.parent.name + " is trying to attack a null target with: " + name);
            return;
        }

        // Layer 6 is limb layer
        if(target.layer == 6)
        {
            Debug.Log("Target a limb");
            targetTrans = target.transform.parent.GetComponent<MechHealthComponent>()._AttachedMech.transform;
        }
        else
        {
            targetTrans = target.transform;
        }

        if (usesAmmo)
        {
            if (currentAmmo == 0 || currentAmmo < ammoUsedPerShot)
            {
                Debug.LogWarning(_AttachedMech.name + " is trying to fire " + name + " with not enough ammo");
                return;
            }
        }

        Debug.Log(_AttachedMech.name + " is starting attack against: " + target.name + " with: " + name);

        this.target = target;
        chargeEndTime = Time.time + weaponStats.AttackSpeed;
        
        isAttacking = true;
        isCharging = true;
        isCoolingDown = false;

        //Debug.Log(WeaponCharging);

        WeaponCharging?.Invoke(this, weaponStats.AttackSpeed);

        // PROTO: Display controled like this for a minute
        // In future make it a unity event the UI is subcribed to
        //if (transform.parent.tag == "Player")
        //    AttackManager.instance.ChargeDisplay(weaponStats.AttackSpeed);
    }

    public virtual void StopAttack(string reason = "No reason given")
    {
        Debug.Log("Stopped attack because: " + reason);

        target = null;
        isAttacking = false;
        isCharging = false;
        isCoolingDown = false;

        CancelAttack?.Invoke(this);
        //AttackManager.instance.TurnOffDisplay();
    }

    protected virtual void FinishedAttack()
    {
        target = null;
        isAttacking = false;
        isCharging = false;
        isCoolingDown = false;

        CancelAttack?.Invoke(this);
    }

    public virtual void StutterCharge(float seconds)
    {
        if(!isAttacking || !isCharging) return;
        // have visual subscribe to this and invode the action here
        chargeEndTime += seconds;
    }

    #endregion


    #region Ammo Funcitons

    public virtual void FiredWeapon()
    {
        if (usesAmmo)
        {
            currentAmmo -= weaponStats.AmmoUsedPerShot;
            AmmoFired?.Invoke(this, currentAmmo);
        }
    }

    public virtual void Reload()
    {
        // Check if buff to reduce reload time
        if (reloading)
        {
            Debug.LogWarning("Tried to call reload when already reloading");
            return;
        }

        reloading = true;
        reloadEndTime = Time.time + weaponStats.ReloadTime;

        Reloading?.Invoke(this, weaponStats.ReloadTime);
    }

    protected virtual void FinishedReload()
    {
        reloading = false;
        // FUTURE: when I want to set up a partial reload system right now I will just do max ammo
        //currentAmmo = Mathf.Min(weaponStats.MaxAmmo, currentAmmo + weaponStats.ReloadAmount);
        currentAmmo = weaponStats.MaxAmmo;
        Reloaded?.Invoke(this, currentAmmo);
    }

    #endregion


}
