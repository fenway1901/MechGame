using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class BaseWeapons : MonoBehaviour
{
    [Header("General Variables")]
    [SerializeField] public string displayName;
    [SerializeField] MonoBehaviour hitEffectBehaviour;
    protected IHitEffect hitEffect;
    [SerializeField] protected float baseDamage;
    [SerializeField] protected float baseRange;
    [SerializeField] protected float baseAttackSpeed;
    [SerializeField] protected float baseCooldown;
    protected float totalDamage;
    protected float totalAttackSpeed;
    protected float totalCooldown;
    protected float totalRange;

    public GameObject _AttachedMech;

    [SerializeField] protected bool isAttacking;
    protected bool isCharging;
    protected bool isCoolingDown;
    protected float chargeEndTime;
    protected float cooldownEndTime;
    protected GameObject target;

    [Header("Ranged Variables (usesAmmo REMEMBER usesAmmo)")]
    [SerializeField] protected bool usesAmmo;
    protected bool reloading;
    [SerializeField] protected int maxAmmo;
    protected int currentAmmo;
    [SerializeField] protected int ammoUsedPerShot;
    [SerializeField] protected float reloadTime;
    protected float reloadEndTime;

    //[Header("Melee Variables")]


    #region Unity Functions

    protected virtual void Awake()
    {
        hitEffect = hitEffectBehaviour as IHitEffect;

        // PROTO: until I get the buff stuff added
        totalDamage = baseDamage;
        totalAttackSpeed = baseAttackSpeed;
        totalCooldown = baseCooldown;
        totalRange = baseRange;
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
                // Time to damage enemy
                if(Time.time >= chargeEndTime)
                {
                    if(GameUtils.GetDistance(_AttachedMech.transform.position, target.transform.position) > totalRange)
                    {
                        StopAttack("Distance to far");
                        return;
                    }

                    if (usesAmmo)
                    {
                        if (currentAmmo == 0 || currentAmmo < ammoUsedPerShot)
                        {
                            StopAttack("Not enough ammo to shoot");
                            return;
                        }

                        FiredWeapon();
                    }

                    Debug.Log(name + " finished charging and applying damage");
                    //target.GetComponent<BaseHealthComponent>().TakeDamage(baseDamage);
                    hitEffect?.Apply(this, target, target.transform.position);
                    isCharging = false;
                    isCoolingDown = true;
                    AttackManager.instance.CooldownDisplay(totalCooldown);
                }
            }
            else if (isCoolingDown)
            {
                // Cooling off weapon
                if(Time.time >= cooldownEndTime)
                {
                    Debug.Log(name + " finshed attack");
                    FinishedAttack();
                }
            }
        }
    }

    #endregion


    #region Stat Controls

    public virtual void AddToDamage(float add)
    {
        baseDamage += add;
    }

    public virtual void ReduceFromDamage(float reduce)
    {
        baseDamage -= reduce;
        baseDamage = Mathf.Clamp(baseDamage, 0, Mathf.Infinity);
    }

    // Come back to how to handle buff system
    public virtual void ApplyBuff(BaseBuff buff)
    {
        // can effet total damage here
    }

    #endregion


    #region Set Functions

    public void SetIsAttacking(bool attack) { isAttacking = attack; }
    public void SetIsRelaoding(bool reload) { reloading = reload; }

    #endregion


    #region Get Functions

    // float gets
    public float GetDamage() { return totalDamage; }
    public float GetRange() { return totalRange; }
    public float GetAttackSpeed() { return totalAttackSpeed; }
    public float GetCoolDown() {  return totalCooldown; }
    
    // int gets
    public int GetMaxAmmo() { return maxAmmo; }
    public int GetCurrentAmmo() { return currentAmmo; }
    public int GetAmmoUsedPerShot() { return ammoUsedPerShot; }

    // bool gets
    public bool GetIsAttacking() { return isAttacking; }
    public bool GetIsReloading() { return reloading; }
    public bool GetIsCharging() { return isCharging; }
    public bool GetIsCoolingDown() { return isCoolingDown; }

    #endregion


    #region Attack Functions

    public virtual void Attack(GameObject target)
    {
        if (target == null)
        {
            Debug.LogError(transform.parent.name + " is trying to attack a null target with: " + name);
            return;
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
        chargeEndTime = Time.time + totalAttackSpeed;
        cooldownEndTime = Time.time + totalCooldown;
        AttackManager.instance.ChargeDisplay(totalAttackSpeed);
        
        isAttacking = true;
        isCharging = true;
        isCoolingDown = false;
    }

    public virtual void StopAttack(string reason = "No reason given")
    {
        Debug.Log("Manually stopped attack because: " + reason);

        target = null;
        isAttacking = false;
        isCharging = false;
        isCoolingDown = false;
    }

    protected virtual void FinishedAttack()
    {
        target = null;
        isAttacking = false;
        isCharging = false;
        isCoolingDown = false;
    }

    #endregion


    #region Ammo Funcitons

    public virtual void FiredWeapon()
    {
        if (usesAmmo)
            currentAmmo = currentAmmo - ammoUsedPerShot;
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
        reloadEndTime = Time.time + reloadTime;
    }

    protected virtual void FinishedReload()
    {
        reloading = false;
        currentAmmo = maxAmmo;
    }

    #endregion


}
