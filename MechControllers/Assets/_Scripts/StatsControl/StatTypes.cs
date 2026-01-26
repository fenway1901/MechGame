public enum StatType
{
    // Mech Stats
    Mech_MaxHealth,
    Mech_Speed,
    Mech_Shield,


    // Limb Stats
    Limb_MaxHealth,
    Limb_Speed,
    Limb_Armor,
    Limb_Weight,

    // Weapon Stats
    Weapon_Damage,
    Weapon_AttackSpeed,
    Weapon_Cooldown,
    Weapon_Range,
    Weapon_Radius,
    Weapon_MaxAmmo,
    Weapon_AmmoUsedPerShot,
    Weapon_ReloadTime,
    Weapon_ReloadAmount
}

public enum ModifierMode
{
    Add, // +X
    Multiply, // *X
    Subtract, // -X
    Divide, // /X
    Override // set to X (Higher priority one will take over)
}
