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
    RightShoulder, 
    LeftShoulder,
    Chest,
    Head
}

// if new limbs exist add them here
public enum LimbSlot 
{ 
    Head, 
    Chest,
    Core,
    LeftArm, 
    RightArm, 
    LeftLeg, 
    RightLeg 
}

[CreateAssetMenu(menuName = "Combat/Buffs/BuffDefinition")]
public class BuffDefinition : ScriptableObject
{
    [System.Serializable]
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
    public Sprite icon;
    public string description;
    public float durationSeconds = 0f; // IF 0 is INFINITE (or until removed manually)
    public bool refreshDurationOnReapply = true;

    public List<StatModSpec> statMods = new();
}
