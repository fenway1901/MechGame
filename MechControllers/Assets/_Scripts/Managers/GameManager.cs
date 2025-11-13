using System.Collections.Generic;
using UnityEngine;

// Will hold all global important information so other objects can easily access it
public class GameManager : MonoBehaviour
{
    public static GameManager singleton;

    public WeaponDatabase _weaponDatabase;

    private void Awake()
    {
        if(singleton == null)
            singleton = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        StartGame();
    }

    // PROTO: change this when ready to start true sequence
    public void StartGame()
    {
        CombatManager.instance.SetUpForCombat();
        CombatManager.instance.StartCombat();
    }


    #region Weapon Management

    public List<BaseWeapons> GetWeapons(List<string> weaponIds)
    {
        List<BaseWeapons> foundWeapons = new List<BaseWeapons>();

        for(int i = 0; i < weaponIds.Count; ++i)
        {
            if(_weaponDatabase.TryGet(weaponIds[i], out BaseWeapons weapon))
            {
                foundWeapons.Add(weapon);
            }
            else
            {
                Debug.LogError("WEAPON " + weaponIds[i] + " NOT FOUND IN DATABASE!"); 
            }
        }

        return foundWeapons;
    }

    #endregion
}
