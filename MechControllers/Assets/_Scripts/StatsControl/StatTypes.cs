public enum StatType
{
    // Mech Stats
    Mech_MaxHealth,
    Mech_Speed,
    Mech_Shield,
    Mech_StutterTime,
    Mech_MaxAmmo,
    Mech_MaxEnergy,
    Mech_MaxMissiles,

    // Limb Stats
    Limb_MaxHealth = 100,
    Limb_Speed = 101,
    Limb_Armor = 102,
    Limb_Weight = 103,

    // Weapon Stats
    Weapon_Damage = 200,
    Weapon_AttackSpeed = 201,
    Weapon_Cooldown = 202,
    Weapon_Range = 203,
    Weapon_Radius = 204,
    Weapon_MaxAmmo = 205,
    Weapon_AmmoUsedPerShot = 206,
    Weapon_ReloadTime = 207,
    Weapon_ReloadAmount = 208,
    Weapon_ArmorPenPercent = 209,
    Weapon_Accuracy = 210
}

public enum ModifierMode
{
    Add, // +X
    Multiply, // *X
    Subtract, // -X
    Divide, // /X
    Override // set to X (Higher priority one will take over)
}
