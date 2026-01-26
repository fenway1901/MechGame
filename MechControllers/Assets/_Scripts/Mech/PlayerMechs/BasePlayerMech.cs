using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class BasePlayerMech : BaseMech
{
    private GameObject panel;

    [Header("Inputs")]
    [SerializeField] private InputAction selectWeapon1;
    [SerializeField] private InputAction selectWeapon2;
    [SerializeField] private InputAction selectWeapon3;


    [Header("PROTOTYPE VARIBLES")]
    public WeaponDisplay activeDisplay;
    public WeaponDisplay weapon1;
    public WeaponDisplay weapon2;
    public WeaponDisplay weapon3;

    private void Awake()
    {
        selectWeapon1.Enable();
        selectWeapon2.Enable();
        selectWeapon3.Enable();
    }

    public override void Init()
    {
        base.Init();

        panel = GameObject.Find("Player Panel");

        SetUpLimbs();
    }


    private void Update()
    {
        // Weapon 1
        if (selectWeapon1.WasPressedThisFrame())
            SelectWeapon(weapons[0], weapon1);

        // Weapon 2
        if (selectWeapon2.WasPressedThisFrame())
            SelectWeapon(weapons[1], weapon2);

        // Weapon 3
        if (selectWeapon3.WasPressedThisFrame())
            SelectWeapon(weapons[2], weapon3);

        //if (activeWeapon != null)
        //    Debug.Log(activeWeapon.GetIsAttacking());

        if (Mouse.current.leftButton.wasReleasedThisFrame && activeWeapon != null && LimbHighlighter.instance.currentLimb != null && !activeWeapon.GetIsAttacking())
        {
            if (activeWeapon.GetIsAttacking())
            {
                Debug.Log("Weapon " + activeWeapon.displayName + " currently attacking");
                return;
            }
            else if (activeWeapon.GetRange() < GameUtils.GetDistance(transform.position, LimbHighlighter.instance.currentLimb.attachedMech.transform.position))
            {
                Debug.Log(GameUtils.GetDistance(transform.position, LimbHighlighter.instance.currentLimb.attachedMech.transform.position));
                Debug.Log("Weapon of " + gameObject.name + " not in range");
                return;
            }

            if (LimbHighlighter.instance.currentLimb.gameObject.CompareTag("Enemy"))
                activeWeapon.Attack(LimbHighlighter.instance.currentLimb.gameObject); //AttackManager.instance.AttackEnemy(LimbHighlighter.instance.currentLimb.gameObject, activeWeapon, activeDisplay);
        }

    }


    #region Limb Management

    #endregion


    #region Weapon Management

    protected virtual void SelectWeapon(BaseWeapons weapon, WeaponDisplay display)
    {
        Debug.Log("Player Selected: " + weapon.name);
        activeWeapon = weapon;
        activeDisplay = display;
    }

    #endregion
}
