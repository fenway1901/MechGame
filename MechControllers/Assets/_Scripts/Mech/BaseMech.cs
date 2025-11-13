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
    public GameObject layout;


    public virtual void Init()
    {
        GetWeapons();

        if (layout == null)
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
        }
    }

    protected virtual void TargetSelected(GameObject target)
    {
        AttackManager.instance.AttackEnemy(target, activeWeapon);
    }

    #endregion
}
