public enum StatType
{
    // Mech Stats
    Mech_MaxHealth,
    Mech_Speed,
    Mech_Shield,
    Mech_StutterTime,

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
    Weapon_ReloadAmount,
    Weapon_CancelTime,
    Weapon_ArmorPenPercent,
    Weapon_Accuracy
}

public enum ModifierMode
{
    Add, // +X
    Multiply, // *X
    Subtract, // -X
    Divide, // /X
    Override // set to X (Higher priority one will take over)
}
