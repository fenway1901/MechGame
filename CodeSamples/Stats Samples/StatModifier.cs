using System;

[Serializable]
public class StatModifier
{
    public StatType stat;
    public ModifierMode mode;
    public float value;
    public int priority;

    // For removal
    public int instanceId;
    public UnityEngine.Object source;
}
