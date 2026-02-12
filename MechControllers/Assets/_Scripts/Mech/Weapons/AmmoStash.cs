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
        public StatType maxAmmo;
        public bool startFull = true;
        public int startAmount = 0;  // if I end up adding between missions ammo saving
    }

    [SerializeField]
    private List<PoolDef> pools = new()
    {
        new PoolDef { type = AmmoType.Bullets, maxAmmo = StatType.Mech_MaxAmmo, startFull = true },
        new PoolDef { type = AmmoType.Energy, maxAmmo = StatType.Mech_MaxEnergy, startFull = true },
        new PoolDef { type = AmmoType.Missiles, maxAmmo = StatType.Mech_MaxMissiles, startFull = true },
    };

    [Header("If buffs can change max ammo at runtime")]
    [SerializeField] private float maxPollInterval = 0.25f;

    private StatsComponent stats;
    //private float nextPoll;

    private class RuntimePool
    {
        public PoolDef def;
        public int current;
        public int max;
    }

    private readonly Dictionary<AmmoType, RuntimePool> runtime = new();

    public event Action<AmmoType, int, int> Changed; // (type, current, max)

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

        foreach (var def in pools)
        {
            if (runtime.ContainsKey(def.type))
            {
                Debug.LogWarning($"{name}: Duplicate ammo pool for {def.type}. Ignoring duplicate.");
                continue;
            }

            RuntimePool rp = new RuntimePool { def = def };
            rp.max = GetMaxFromStats(def.maxAmmo);
            rp.current = def.startFull ? rp.max : Mathf.Clamp(def.startAmount, 0, rp.max);

            runtime.Add(def.type, rp);
            Changed?.Invoke(def.type, rp.current, rp.max);
        }
    }

    private void Update()
    {
        // If I end up adding changing max supply mid mission
        /*
        if (Time.time < nextPoll) return;
        nextPoll = Time.time + maxPollInterval;

        foreach (var kv in runtime)
        {
            RuntimePool rp = kv.Value;
            int newMax = GetMaxFromStats(rp.def.maxAmmo);
            if (newMax == rp.max) continue;

            rp.max = newMax;
            if (rp.current > rp.max) rp.current = rp.max;

            Changed?.Invoke(rp.def.type, rp.current, rp.max);
        }*/
    }

    private int GetMaxFromStats(StatType stat) => Mathf.Max(0, Mathf.RoundToInt(stats.Get(stat)));

    public int GetCurrent(AmmoType type) => runtime.TryGetValue(type, out RuntimePool rp) ? rp.current : 0;
    public int GetMax(AmmoType type) => runtime.TryGetValue(type, out RuntimePool rp) ? rp.max : 0;

    public int Take(AmmoType type, int amount)
    {
        if (amount <= 0) return 0;
        if (!runtime.TryGetValue(type, out RuntimePool rp)) return 0;

        int taken = Mathf.Min(rp.current, amount);
        if (taken <= 0) return 0;

        rp.current -= taken;
        Changed?.Invoke(type, rp.current, rp.max);
        return taken;
    }

    public int Add(AmmoType type, int amount)
    {
        if (amount <= 0) return 0;
        if (!runtime.TryGetValue(type, out RuntimePool rp)) return 0;

        int before = rp.current;
        rp.current = Mathf.Min(rp.max, rp.current + amount);

        int added = rp.current - before;
        if (added != 0) Changed?.Invoke(type, rp.current, rp.max);
        return added;
    }

    public void Refill(AmmoType type)
    {
        if (!runtime.TryGetValue(type, out var rp)) return;
        rp.current = rp.max;
        Changed?.Invoke(type, rp.current, rp.max);
    }
}
