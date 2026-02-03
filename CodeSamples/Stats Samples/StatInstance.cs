using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class StatInstance
{
    public float BaseValue { get; private set; }

    private readonly List<StatModifier> _mods = new();
    private bool _dirty = true;
    private float _cached;


    public StatInstance(float baseValue)
    {
        BaseValue = baseValue; ;
        _dirty = true;
    }

    public void SetBase(float baseValue)
    {
        BaseValue = baseValue;
        _dirty = true;
    }

    public void AddModifier(StatModifier mod)
    {
        _mods.Add(mod);
        _dirty = true;
    }

    public void RemoveByInstanceId(int instanceId)
    {
        if (_mods.RemoveAll(m => m.instanceId == instanceId) > 0)
            _dirty = true;
    }

    public void RemoveBySource(UnityEngine.Object source)
    {
        if (_mods.RemoveAll(m => m.source == source) > 0)
            _dirty = true;
    }

    public float GetValue()
    {
        if (!_dirty) return _cached;

        // Overrides: highest priority wins
        var overrideMod = _mods
            .Where(m => m.mode == ModifierMode.Override)
            .OrderBy(m => m.priority)
            .LastOrDefault();

        if (overrideMod != null)
        {
            _cached = overrideMod.value;
            _dirty = false;
            return _cached;
        }

        float addSum = 0f;
        float mulProduct = 1f;

        foreach (var m in _mods)
        {
            switch (m.mode)
            {
                case ModifierMode.Add:
                    addSum += m.value;
                    break;
                case ModifierMode.Subtract:
                    addSum -= m.value;
                    break;
                case ModifierMode.Multiply:
                    mulProduct *= m.value;
                    break;
                case ModifierMode.Divide:
                    if(m.value != 0f) mulProduct /= m.value;
                    break;
            }
        }

        _cached = (BaseValue + addSum) * mulProduct;
        _dirty = false;
        return _cached;
    }
}
