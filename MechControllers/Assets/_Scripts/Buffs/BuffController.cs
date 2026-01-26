using System.Collections.Generic;
using UnityEngine;

public class BuffController : MonoBehaviour
{
    private int _nextInstanceId = 1;

    [SerializeField] private StatsComponent mechStats;
    [SerializeField] private List<BaseWeaponStats> weapons = new();
    [SerializeField] private List<BaseLimbStats> limbs = new();

    private readonly Dictionary<int, BuffInstance> _active = new();

    private void Reset()
    {
        mechStats = GetComponent<StatsComponent>();
    }

    private void Update()
    {
        // Tick durations
        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        var toRemove = ListPool<int>.Get();
        foreach (var kvp in _active)
        {
            BuffInstance inst = kvp.Value;
            if (inst.remaining < 0f) continue; // infinite

            inst.remaining -= dt;
            if (inst.remaining <= 0f)
                toRemove.Add(inst.instanceId);
        }

        foreach (var id in toRemove)
            Remove(id);

        ListPool<int>.Release(toRemove);
    }

    public int Apply(BuffDefinition def, Object source)
    {
        // Optional: if you want “reapply refreshes duration” behavior, search existing by def+source.
        int id = _nextInstanceId++;
        BuffInstance inst = new BuffInstance
        {
            instanceId = id,
            definition = def,
            source = source,
            remaining = def.durationSeconds <= 0f ? -1f : def.durationSeconds
        };

        _active[id] = inst;

        foreach (var spec in def.statMods)
        {
            foreach (var targetStats in ResolveTargets(spec))
            {
                var mod = new StatModifier
                {
                    stat = spec.stat,
                    mode = spec.mod,
                    value = spec.value,
                    priority = spec.priority,
                    instanceId = id,
                    source = source
                };
                targetStats.AddModifier(mod);
            }
        }

        return id;
    }

    public void Remove(int instanceId)
    {
        if (!_active.TryGetValue(instanceId, out var inst)) return;

        // Remove from all routed stats holders.
        mechStats.RemoveModifiersByInstanceId(instanceId);
        foreach (var w in weapons) w.Stats.RemoveModifiersByInstanceId(instanceId);
        foreach (var l in limbs) l.Stats.RemoveModifiersByInstanceId(instanceId);

        _active.Remove(instanceId);
    }

    public void RemoveAllFromSource(Object source)
    {
        // Fast approach: remove modifiers directly, then clear active list entries that match.
        mechStats.RemoveModifiersBySource(source);
        foreach (var w in weapons) w.Stats.RemoveModifiersBySource(source);
        foreach (var l in limbs) l.Stats.RemoveModifiersBySource(source);

        var toRemove = ListPool<int>.Get();
        foreach (var kvp in _active)
            if (kvp.Value.source == source) toRemove.Add(kvp.Key);

        foreach (var id in toRemove) _active.Remove(id);
        ListPool<int>.Release(toRemove);
    }

    private IEnumerable<StatsComponent> ResolveTargets(BuffDefinition.StatModSpec spec)
    {
        switch (spec.target)
        {
            case BuffTarget.Mech:
                if (mechStats != null) yield return mechStats;
                yield break;

            case BuffTarget.AllWeapons:
                foreach (BaseWeaponStats w in weapons)
                    if (w != null) yield return w.Stats;
                yield break;

            case BuffTarget.WeaponsWithTag:
                foreach (BaseWeaponStats w in weapons)
                    if (w != null && w.HasTag(spec.targetTag)) yield return w.Stats;
                yield break;

            case BuffTarget.SpecificWeaponSlot:
                foreach (BaseWeaponStats w in weapons)
                    if (w != null && w.Slot == spec.weaponSlot) yield return w.Stats;
                yield break;

            case BuffTarget.AllLimbs:
                foreach (BaseLimbStats l in limbs)
                    if (l != null) yield return l.Stats;
                yield break;

            case BuffTarget.LimbsWithTag:
                foreach (BaseLimbStats l in limbs)
                    if (l != null && l.HasTag(spec.targetTag)) yield return l.Stats;
                yield break;

            case BuffTarget.SpecificLimbSlot:
                foreach (BaseLimbStats l in limbs)
                    if (l != null && l.Slot == spec.limbSlot) yield return l.Stats;
                yield break;
        }
    }

    private class BuffInstance
    {
        public int instanceId;
        public BuffDefinition definition;
        public Object source;
        public float remaining; // -1 = infinite
    }
}