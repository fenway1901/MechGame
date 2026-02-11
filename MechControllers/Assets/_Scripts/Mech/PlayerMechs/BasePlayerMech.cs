using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class BasePlayerMech : BaseMech
{
    private GameObject panel;

    [Header("Inputs")]
    [SerializeField] private InputAction selectWeapon1;
    [SerializeField] private InputAction selectWeapon2;
    [SerializeField] private InputAction selectWeapon3;
    [SerializeField] private InputAction reload;

    public event Action<BaseWeapons> WeaponSelected;

    private void Awake()
    {
        selectWeapon1.Enable();
        selectWeapon2.Enable();
        selectWeapon3.Enable();
        reload.Enable();
    }

    public override void Init()
    {
        base.Init();

        panel = GameObject.Find("Player Panel");

        healthComp.Damaged += DamageTaken;
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

        // Reload Sequence
        if (reload.WasPressedThisFrame() && activeWeapon != null && !activeWeapon.GetIsAttacking())
        {
            if (activeWeapon.usesAmmo && activeWeapon.GetCurrentAmmo() != activeWeapon.GetMaxAmmo())
                activeWeapon.Reload();
        }

        // Target selection sequence
        if (Mouse.current.leftButton.wasReleasedThisFrame && activeWeapon != null && LimbHighlighter.instance.currentLimb != null && !activeWeapon.GetIsAttacking())
        {
            if (activeWeapon.GetIsAttacking())
            {
                Debug.Log("Weapon " + activeWeapon.displayName + " currently attacking");
                return;
            }
            else if (activeWeapon.GetRange() < GameUtils.GetDistance(transform.position, LimbHighlighter.instance.currentLimb._AttachedMech.transform.position))
            {
                Debug.Log(GameUtils.GetDistance(transform.position, LimbHighlighter.instance.currentLimb._AttachedMech.transform.position));
                Debug.Log("Weapon of " + gameObject.name + " not in range");
                return;
            }

            if (LimbHighlighter.instance.currentLimb.gameObject.CompareTag("Enemy"))
                activeWeapon.Attack(LimbHighlighter.instance.currentLimb.gameObject); //AttackManager.instance.AttackEnemy(LimbHighlighter.instance.currentLimb.gameObject, activeWeapon, activeDisplay);
        }

    }

    private Coroutine shakeRoutine;
    private Vector3 camOriginalLocalPos;

    protected virtual void DamageTaken(BaseHealthComponent comp, float damage, float currentHealth)
    {
        if (currentHealth <= 0) return;

        Transform camT = Camera.main.transform;

        if (shakeRoutine == null)
            camOriginalLocalPos = camT.localPosition;

        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
            camT.localPosition = camOriginalLocalPos;
            shakeRoutine = null;
        }

        shakeRoutine = StartCoroutine(GameUtils.ShakeTransform(camT, 0.08f, 0.08f));

        EffectsManager.instance.PlaySparks();
    }


    #region Limb Management

    #endregion


    #region Weapon Management

    protected virtual void SelectWeapon(BaseWeapons weapon)
    {
        //Debug.Log("Player Selected: " + weapon.name);
        activeWeapon = weapon;
        
        // So all weapons are ready
        WeaponSelected.Invoke(weapon);
    }

    #endregion
}
