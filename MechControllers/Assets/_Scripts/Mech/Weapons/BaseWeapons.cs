using UnityEngine;

public class BaseWeapons : MonoBehaviour
{
    [SerializeField] protected float damage;
    [SerializeField] protected int maxAmmo;
    protected int currentAmmo;
    [SerializeField] protected float range;
    [SerializeField] protected float attackSpeed;


    #region Attack

    public void AttackEnemy()
    {

    }

    #endregion

    #region Stat Controls

    public void AddToDamage(float add)
    {
        damage += add;
    }

    public void ReduceFromDamage(float reduce)
    {
        damage -= reduce;
        damage = Mathf.Clamp(damage, 0, Mathf.Infinity);
    }

    // Come back to how to handle buff system
    public void ApplyBuff(BaseBuff buff)
    {

    }

    #endregion


    #region Get Functions

    // float gets
    public float GetDamage() { return damage; }
    public float GetRange() { return range; }
    public float GetAttackSpeed() { return attackSpeed; }
    
    // int gets
    public int GetMaxAmmo() { return maxAmmo; }
    public int GetCurrentAmmo() { return currentAmmo; }

    #endregion
}
