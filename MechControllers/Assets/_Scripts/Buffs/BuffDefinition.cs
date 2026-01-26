using System.Collections.Generic;
using UnityEngine;

public enum BuffTarget
{
    Mech,
    AllMechs,
    AllWeapons,
    WeaponsWithTag,
    AllLimbs,
    LimbsWithTag,

    SpecificWeaponSlot,
    SpecificLimbSlot,

    SpecificObject
}

// If I new weapon Slot or names here
public enum WeaponSlot 
{ 
    LeftArm, 
    RightArm, 
    ShoulderL, 
    ShoulderR, 
    Core 
}

// if new limbs exist add them here
public enum LimbSlot 
{ 
    Head, 
    Torso, 
    LeftArm, 
    RightArm, 
    LeftLeg, 
    RightLeg 
}

[CreateAssetMenu(menuName = "Combat/Buffs/BuffDefinition")]
public class BuffDefinition : ScriptableObject
{
    [SerializeField]
    public class StatModSpec
    {
        public BuffTarget target;
        public string targetTag;

        public StatType stat;
        public ModifierMode mod;
        public float value;
        public int priority;

        public WeaponSlot weaponSlot;
        public LimbSlot limbSlot;
    }

    public string buffName;
    public float durationSeconds = 0f; // IF 0 is INFINITE (or until removed manually)
    public bool refreshDurationOnReapply = true;

    public List<StatModSpec> statMods = new();
}
