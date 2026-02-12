using System.Collections.Generic;
using System;
using UnityEngine;

public enum AmmoType
{
    Bullets,
    Energy,
    Missiles
}

public class AmmoStash : MonoBehaviour
{
    [Serializable]
    public class PoolDef
    {
        public AmmoType type;

        [Header("Capacity")]
        public StatType maxStat;                 // e.g. Mech_MaxAmmo_A / _B / _C (or Mech_MaxEnergy for energy)
        public bool startFull = true;
        public int startAmount = 0;

        [Header("Regen (optional)")]
        public bool regenerates = false;         // turn on for energy pool
        public StatType regenPerSecondStat;      // StatType.Mech_EnergyPerSecond
    }

    [SerializeField] private List<PoolDef> pools = new();

    [Header("Stat polling (if buffs can change max / regen at runtime)")]
    [SerializeField] private float statPollInterval = 0.25f;

    private StatsComponent stats;
    private float nextPoll;

    private class RuntimePool
    {
        public PoolDef def;

        public int max;

        // Store as float so regen can be smooth; ammo ops still use ints.
        public float currentF;

        // Track last int value so we don’t spam Changed every frame during regen.
        public int lastReportedInt;
    }

    private readonly Dictionary<AmmoType, RuntimePool> runtime = new();

    // (type, current, max)
    public event Action<AmmoType, int, int> Changed;

    private void Awake()
    {
        stats = GetComponent<StatsComponent>();
        if (!stats)
        {
            Debug.LogError($"{name}: MechAmmoStash requires StatsComponent.");
            enabled = false;
            return;
        }

        runtime.Clear();

        foreach (PoolDef def in pools)
        {
            if (runtime.ContainsKey(def.type))
            {
                Debug.LogWarning($"{name}: Duplicate ammo pool for {def.type}. Ignoring duplicate.");
                continue;
            }

            var rp = new RuntimePool { def = def };
            rp.max = GetMaxFromStats(def.maxStat);

            int start = def.startFull ? rp.max : Mathf.Clamp(def.startAmount, 0, rp.max);
            rp.currentF = start;
            rp.lastReportedInt = start;

            runtime.Add(def.type, rp);
            Changed?.Invoke(def.type, rp.lastReportedInt, rp.max);
        }
    }

    private void Update()
    {
        // Regen every frame
        foreach (var kv in runtime)
        {
            var rp = kv.Value;
            if (!rp.def.regenerates) continue;
            if (rp.currentF >= rp.max) continue;

            float perSec = Mathf.Max(0f, stats.Get(rp.def.regenPerSecondStat));
            if (perSec <= 0f) continue;

            rp.currentF = Mathf.Min(rp.max, rp.currentF + perSec * Time.deltaTime);

            int currentInt = Mathf.FloorToInt(rp.currentF);
            if (currentInt != rp.lastReportedInt)
            {
                rp.lastReportedInt = currentInt;
                Changed?.Invoke(rp.def.type, rp.lastReportedInt, rp.max);
            }
        }

        // If I want to add buffing max ammo count on any
        /*if (Time.time < nextPoll) return;
        nextPoll = Time.time + statPollInterval;

        foreach (var kv in runtime)
        {
            var rp = kv.Value;
            int newMax = GetMaxFromStats(rp.def.maxStat);
            if (newMax == rp.max) continue;

            rp.max = newMax;
            rp.currentF = Mathf.Min(rp.currentF, rp.max);

            int currentInt = Mathf.FloorToInt(rp.currentF);
            rp.lastReportedInt = currentInt;
            Changed?.Invoke(rp.def.type, currentInt, rp.max);
        }*/
    }

    private int GetMaxFromStats(StatType stat) => Mathf.Max(0, Mathf.RoundToInt(stats.Get(stat)));
    public int GetCurrent(AmmoType type) => runtime.TryGetValue(type, out var rp) ? Mathf.FloorToInt(rp.currentF) : 0;
    public int GetMax(AmmoType type) => runtime.TryGetValue(type, out var rp) ? rp.max : 0;
    public float GetCurrentFloat(AmmoType type) => runtime.TryGetValue(type, out var rp) ? rp.currentF : 0f;

    public int Take(AmmoType type, int amount)
    {
        if (amount <= 0) return 0;
        if (!runtime.TryGetValue(type, out var rp)) return 0;

        int currentInt = Mathf.FloorToInt(rp.currentF);
        int taken = Mathf.Min(currentInt, amount);
        if (taken <= 0) return 0;

        rp.currentF = Mathf.Max(0f, rp.currentF - taken);

        int newInt = Mathf.FloorToInt(rp.currentF);
        if (newInt != rp.lastReportedInt)
        {
            rp.lastReportedInt = newInt;
            Changed?.Invoke(type, newInt, rp.max);
        }

        return taken;
    }

    public int Add(AmmoType type, int amount)
    {
        if (amount <= 0) return 0;
        if (!runtime.TryGetValue(type, out var rp)) return 0;

        float before = rp.currentF;
        rp.currentF = Mathf.Min(rp.max, rp.currentF + amount);

        int beforeInt = Mathf.FloorToInt(before);
        int afterInt = Mathf.FloorToInt(rp.currentF);

        if (afterInt != rp.lastReportedInt)
        {
            rp.lastReportedInt = afterInt;
            Changed?.Invoke(type, afterInt, rp.max);
        }

        return Mathf.Max(0, afterInt - beforeInt);
    }
}
