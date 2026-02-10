using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public static AttackManager instance;

    // CURRENTLY NOT USED ANYMORE

    void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region AttackSequence

    public void AttackEnemy(GameObject target, BaseWeapons weapon, WeaponDisplay display = null)
    {
        // Check if it has the needed ammo
        /*if (weapon.usesAmmo)
        {
            if(weapon.GetCurrentAmmo() == 0 || weapon.GetCurrentAmmo() < weapon.GetAmmoUsedPerShot())
            {
                Debug.LogWarning("You are trying to fire a weapon that dosn't have enough ammo");
                return;
            }
        }*/

        if (display != null)
        {
            Debug.Log("turing on attack timer!");
            display.SetTimer(weapon.GetAttackSpeed());
            display.timerTxt.color = Color.white;
        }

        StartCoroutine(Attack(target, weapon, display));
        
        Debug.Log("attacking " + target.name + " with " + weapon.displayName);
    }

    private IEnumerator Attack(GameObject target, BaseWeapons weapon, WeaponDisplay display = null)
    {
        weapon.SetIsAttacking(true);

        yield return new WaitForSeconds(weapon.GetAttackSpeed());

        if(display != null)
        {
            Debug.Log("turning on cooldown timer");
            display.SetTimer(weapon.GetCoolDown());
            display.timerTxt.color = Color.darkRed;
        }

        target.GetComponent<BaseHealthComponent>().TakeDamage(weapon.GetDamage());

        yield return new WaitForSeconds(weapon.GetCoolDown());

        weapon.SetIsAttacking(false);
    }

    #endregion
}
