using System.Collections.Generic;
using UnityEngine;

// PUT ON MECH ROOT
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

    #region Set and Get functions

    public void SetLimbs(List<BaseLimbStats> limbs) { this.limbs = limbs; }
    public void SetLimbs(List<BaseLimb> limbs)
    {
        this.limbs.Clear();
        if (limbs == null) return;

        foreach (BaseLimb w in limbs)
        {
            if (w != null)
                this.limbs.Add(w.GetLimbStats());
        }
    }
    public void SetWeapons(List<BaseWeaponStats> weapons) { this.weapons = weapons; }
    public void SetWeapons(List<BaseWeapons> weapons)
    {
        this.weapons.Clear();
        if (weapons == null) return;

        foreach (BaseWeapons w in weapons)
        {
            if (w != null)
                this.weapons.Add(w.GetWeaponStats());
        }
    }


    #endregion


    public int Apply(BuffDefinition def, Object source)
    {
        // Optional: “reapply refreshes duration” behavior, search existing by def+source.
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
            foreach (StatsComponent targetStats in ResolveTargets(spec, null))
            {
                StatModifier mod = new StatModifier
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

    public int Apply(BuffDefinition def, Object source, Object target = null)
    {
        int id = _nextInstanceId++;
        BuffInstance inst = new BuffInstance
        {
            instanceId = id,
            definition = def,
            source = source,
            remaining = def.durationSeconds <= 0f ? -1f : def.durationSeconds
        };

        _active[id] = inst;

        foreach (BuffDefinition.StatModSpec spec in def.statMods)
        {
            foreach (StatsComponent targetStats in ResolveTargets(spec, target))
            {
                StatModifier mod = new StatModifier
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
        foreach (BaseWeaponStats w in weapons) w.Stats.RemoveModifiersByInstanceId(instanceId);
        foreach (BaseLimbStats l in limbs) l.Stats.RemoveModifiersByInstanceId(instanceId);

        _active.Remove(instanceId);
    }

    public void RemoveAllFromSource(Object source)
    {
        // Fast approach: remove modifiers directly, then clear active list entries that match.
        mechStats.RemoveModifiersBySource(source);
        foreach (BaseWeaponStats w in weapons) w.Stats.RemoveModifiersBySource(source);
        foreach (BaseLimbStats l in limbs) l.Stats.RemoveModifiersBySource(source);

        var toRemove = ListPool<int>.Get();
        foreach (var kvp in _active)
            if (kvp.Value.source == source) toRemove.Add(kvp.Key);

        foreach (int id in toRemove) _active.Remove(id);
        ListPool<int>.Release(toRemove);
    }

    private IEnumerable<StatsComponent> ResolveTargets(BuffDefinition.StatModSpec spec, Object specificTarget)
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

            case BuffTarget.SpecificObject:
                if (specificTarget == null) yield break;

                // Allow passing a StatsComponent directly
                if (specificTarget is StatsComponent sc)
                {
                    yield return sc;
                    yield break;
                }

                // Or passing a weapon/limb stats wrapper
                if (specificTarget is BaseWeaponStats ws)
                {
                    yield return ws.Stats;
                    yield break;
                }

                if (specificTarget is BaseLimbStats ls)
                {
                    yield return ls.Stats;
                    yield break;
                }

                // Or passing a GameObject / Component
                if (specificTarget is Component c)
                {
                    StatsComponent found = c.GetComponent<StatsComponent>();
                    if (found != null) yield return found;
                    yield break;
                }

                if (specificTarget is GameObject go)
                {
                    StatsComponent found = go.GetComponent<StatsComponent>();
                    if (found != null) yield return found;
                    yield break;
                }

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