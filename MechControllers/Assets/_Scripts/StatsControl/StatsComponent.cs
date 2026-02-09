using System;
using UnityEngine;
using System.Collections.Generic;

public class StatsComponent : MonoBehaviour
{
    [Serializable]
    public struct BaseStat
    {
        public StatType stat;
        public float baseValue;
    }

    [SerializeField] private List<BaseStat> baseStats = new();

    private readonly Dictionary<StatType, StatInstance> _map = new();

    public event Action OnStatChanged;

    public void Awake()
    {
        foreach (BaseStat bs in baseStats)
        {
            if (!_map.ContainsKey(bs.stat))
                _map[bs.stat] = new StatInstance(bs.baseValue);
        }
    }

    public bool Has(StatType stat) => _map.ContainsKey(stat);

    public float Get(StatType stat)
    {
        if(_map.TryGetValue(stat, out StatInstance inst)) return inst.GetValue();
        Debug.Log("Failed to get stat " + stat);
        return 0f;
    }

    public int GetInt(StatType stat)
    {
        if (_map.TryGetValue(stat, out StatInstance inst)) return Mathf.RoundToInt(inst.GetValue());
        Debug.Log("Failed to get stat " + stat);
        return 0;
    }

    public void SetBase(StatType stat, float baseValue)
    {
        if(!_map.TryGetValue(stat, out StatInstance inst))
        {
            _map[stat] = new StatInstance(baseValue);
        }
        else
            inst.SetBase(baseValue);

        // Only if needed add ? otherwise i want to know if an error is called
        OnStatChanged?.Invoke();
    }

    public float GetBase(StatType stat)
    {
        if (_map.TryGetValue(stat, out StatInstance inst)) return inst.BaseValue;
        Debug.Log("Failed to get base stat " + stat);
        return 0f;
    }

    public bool TryGet(StatType stat, out float value)
    {
        if (_map.TryGetValue(stat, out StatInstance inst))
        {
            value = inst.GetValue();
            return true;
        }
        value = 0f;
        return false;
    }

    public void AddModifier(
        StatType stat,
        ModifierMode mode,
        float value,
        UnityEngine.Object source = null,
        int instanceId = 0,
        int priority = 0)
    {
        AddModifier(new StatModifier
        {
            stat = stat,
            mode = mode,
            value = value,
            source = source,
            instanceId = instanceId,
            priority = priority
        });
    }

    public void AddModifier(StatModifier modifier)
    {
        if(!_map.TryGetValue(modifier.stat, out StatInstance inst))
        {
            Debug.LogWarning("Stat: " + modifier.stat + " does not exist");
            return;
        }

        inst.AddModifier(modifier);

        // Same only add ? if desperatly need to
        OnStatChanged?.Invoke();
    }

    public void RemoveModifiersByInstanceId(int instanceId)
    {
        foreach (var kvp in _map)
            kvp.Value.RemoveByInstanceId(instanceId);

        // Same only add ? if desperatly need to
        OnStatChanged?.Invoke();
    }

    public void RemoveModifiersBySource(UnityEngine.Object source)
    {
        foreach (var kvp in _map)
            kvp.Value.RemoveBySource(source);

        // Same only add ? if desperatly need to
        OnStatChanged?.Invoke();
    }
}
