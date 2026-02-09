using UnityEngine;
using System.Collections.Generic;

public class EnemyLimb : BaseLimb
{
    [Header("Targeting Visuals")]
    [SerializeField] private GameObject targetIndicator;

    List<BaseWeapons> weaponsTargetingLimb = new List<BaseWeapons>();

    protected override void Awake()
    {
        base.Awake();

        targetIndicator.SetActive(false);
    }

    public void TurnOnIndicator(BaseWeapons weapon)
    {
        if (weaponsTargetingLimb.Contains(weapon)) return;

        weaponsTargetingLimb.Add(weapon);

        if (!targetIndicator.activeInHierarchy)
            targetIndicator.SetActive(true);
    }

    public void TurnOffIndicator(BaseWeapons weapon)
    {
        weaponsTargetingLimb.Remove(weapon);

        if (weaponsTargetingLimb.Count > 0) return; // weapon still targeting limb

        if (targetIndicator.activeInHierarchy)
            targetIndicator.SetActive(false);
    }
}
