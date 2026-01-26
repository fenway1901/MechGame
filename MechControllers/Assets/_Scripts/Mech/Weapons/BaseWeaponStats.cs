using UnityEngine;

[RequireComponent(typeof(BaseWeapons))]
public class BaseWeaponStats : MonoBehaviour
{
    [SerializeField] protected string[] tags;
    [SerializeField] protected WeaponSlot slot;

    private BaseWeapons weapon;
    private StatsComponent stats;

    private void Awake()
    {
        weapon = GetComponent<BaseWeapons>();
        stats = GetComponent<StatsComponent>();
        if (stats == null) stats = gameObject.AddComponent<StatsComponent>();

        // Initialize base values into StatsComponent
        // These are "base" and should NOT be modified by buffs.
        stats.SetBase(StatType.Weapon_Damage, weapon.BaseDamage);
        stats.SetBase(StatType.Weapon_Range, weapon.BaseRange);
        stats.SetBase(StatType.Weapon_AttackSpeed, weapon.BaseAttackSpeed);
        stats.SetBase(StatType.Weapon_Cooldown, weapon.BaseCooldown);
        //stats.SetBase(StatType.Weapon_Radius, weapon.);

        if (weapon.usesAmmo)
        {
            stats.SetBase(StatType.Weapon_MaxAmmo, weapon.BaseMaxAmmo);
            stats.SetBase(StatType.Weapon_AmmoUsedPerShot, weapon.BaseAmmoUsedPerShot);
            stats.SetBase(StatType.Weapon_ReloadTime, weapon.BaseReloadTime);
            stats.SetBase(StatType.Weapon_ReloadAmount, weapon.BaseReloadAmount);
        }
        else
        {
            stats.SetBase(StatType.Weapon_MaxAmmo, 0);
            stats.SetBase(StatType.Weapon_AmmoUsedPerShot, 0);
            stats.SetBase(StatType.Weapon_ReloadTime, 0);
            stats.SetBase(StatType.Weapon_ReloadAmount, 0);
        }

    }

    public float Damage => stats.Get(StatType.Weapon_Damage);
    public float Range => stats.Get(StatType.Weapon_Range);
    public float AttackSpeed => stats.Get(StatType.Weapon_AttackSpeed);
    public float Cooldown => stats.Get(StatType.Weapon_Cooldown);
    public float ReloadTime => stats.Get(StatType.Weapon_ReloadTime);

    
    public int MaxAmmo => stats.GetInt(StatType.Weapon_MaxAmmo);
    public int AmmoUsedPerShot => stats.GetInt(StatType.Weapon_AmmoUsedPerShot);
    public int ReloadAmount => stats.GetInt(StatType.Weapon_ReloadAmount);

    public StatsComponent Stats => stats;
    public WeaponSlot Slot => slot;

    public bool HasTag(string t)
    {
        if (string.IsNullOrEmpty(t) || tags == null) return false;
        for (int i = 0; i < tags.Length; i++)
            if (tags[i] == t) return true;
        return false;
    }
}
