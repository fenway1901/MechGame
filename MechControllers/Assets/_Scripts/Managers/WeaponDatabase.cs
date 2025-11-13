using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class WeaponPrefabEntry
{
    public string id; // e.g. "LaserCannon"
    public BaseWeapons prefab;
}


// Holds all prefabs of weapons so any mech can grab the data and instantiat their own version
[CreateAssetMenu(menuName = "Game/WeaponDatabase")]
public class WeaponDatabase : ScriptableObject
{
    public List<WeaponPrefabEntry> weapons = new();
    private Dictionary<string, BaseWeapons> _byId;

    void OnEnable()
    {
        _byId = new(weapons.Count);
        foreach (var e in weapons)
        {
            if (string.IsNullOrEmpty(e.id) || e.prefab == null)
            {
                Debug.LogWarning($"Invalid weapon entry: {e.id}");
                continue;
            }

            if (_byId.ContainsKey(e.id))
                Debug.LogError($"Duplicate weapon id: {e.id}");
            else
                _byId[e.id] = e.prefab;
        }
    }

    public bool TryGet(string id, out BaseWeapons prefab)
        => _byId.TryGetValue(id, out prefab);
}
