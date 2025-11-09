using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class BaseMech : MonoBehaviour
{
    public List<BaseWeapons> weapons;
    public List<BaseLimb> parts;
    public BaseWeapons activeWeapon;

    [Header("Inputs")]
    [SerializeField] private InputAction selectWeapon1;
    [SerializeField] private InputAction selectWeapon2;
    [SerializeField] private InputAction selectWeapon3;


    private void Awake()
    {
        selectWeapon1.Enable();
        selectWeapon2.Enable();
        selectWeapon3.Enable();
    }

    private void Update()
    {
        // Weapon 1
        if (selectWeapon1.WasPressedThisFrame())
            SelectWeapon(weapons[0]);

        // Weapon 2
        if (selectWeapon2.WasPressedThisFrame())
            SelectWeapon(weapons[1]);

        // Weapon 3
        if (selectWeapon3.WasPressedThisFrame())
            SelectWeapon(weapons[2]);

        if(Mouse.current.leftButton.wasReleasedThisFrame && activeWeapon != null && LimbHighlighter.instance.currentLimb != null)
        {
            if (LimbHighlighter.instance.currentLimb.gameObject.CompareTag("Enemy"))
                AttackManager.instance.AttackEnemy(LimbHighlighter.instance.currentLimb.gameObject, activeWeapon);
        }
    }


    #region Get Info



    #endregion


    #region Movement


    #endregion



    #region Weapon Management

    private void SelectWeapon(BaseWeapons weapon)
    {
        Debug.Log(weapon.name + " is Selected!!!");
        activeWeapon = weapon;
    }

    private void TargetSelected(GameObject target)
    {
        AttackManager.instance.AttackEnemy(target, activeWeapon);
    }

    #endregion
}
