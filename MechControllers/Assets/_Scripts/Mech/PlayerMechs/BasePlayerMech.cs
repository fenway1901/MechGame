using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BasePlayerMech : BaseMech
{
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

        if (activeWeapon != null)
            Debug.Log(activeWeapon.GetIsAttacking());

        if (Mouse.current.leftButton.wasReleasedThisFrame && activeWeapon != null && LimbHighlighter.instance.currentLimb != null && !activeWeapon.GetIsAttacking())
        {
            if (activeWeapon.GetIsAttacking())
            {
                Debug.Log("Weapon " + activeWeapon.displayName + " currently attacking");
                return;
            }

            if (LimbHighlighter.instance.currentLimb.gameObject.CompareTag("Enemy"))
                AttackManager.instance.AttackEnemy(LimbHighlighter.instance.currentLimb.gameObject, activeWeapon, activeDisplay);
        }

    }


    #region Limb Management

    private void SetUpLimbs()
    {
        Instantiate(layout, transform);

        limbs = new List<BaseLimb>();

        for (int i = 0; i < layout.transform.childCount; ++i)
        {
            // Just incase i will add things that arn't limbs to this prefab
            if (layout.transform.GetChild(i).GetComponent<BaseLimb>())
            {
                limbs.Add(layout.transform.GetChild(i).GetComponent<BaseLimb>());
            }
        }
    }

    #endregion


    #region Weapon Management

    protected virtual void SelectWeapon(BaseWeapons weapon, WeaponDisplay display)
    {
        Debug.Log(weapon.name + " is Selected!!!");
        activeWeapon = weapon;
        activeDisplay = display;
    }

    #endregion
}
