using System;
using System.Net.Sockets;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class LimbWeaponMounts : MonoBehaviour
{
    [Serializable]
    public class Mount
    {
        public WeaponSlot slot;
        public Transform mountPoint; // Use for maybe a visual element of where the weapon is
        public BaseWeapons equipped;
    }

    [SerializeField] private List<Mount> mounts = new();

    public IEnumerable<BaseWeapons> EquippedWeapons => mounts.Where(m => m.equipped != null).Select(m => m.equipped);

    public bool TryEquip(BaseWeapons weapon)
    {
        WeaponSlot ws = weapon.GetWeaponStats().Slot;
        //Debug.Log(ws + " is the slot trying to connect to " + weapon.GetWeaponStats().Slot + " of weapon " + weapon.displayName);
        Mount m = mounts.FirstOrDefault(x => x.slot == ws && x.equipped == null);

        // if the limb doesnt have weapons to equip
        if(m == null)
            return false;

        m.equipped = weapon;
        
        //TO DO: place the visual to the transform of the limb or something here
        // need to set up how I want to visualize weapons first

        return true;
    }

    public void DisableAllweapons(string reason)
    {
        foreach (BaseWeapons w in EquippedWeapons)
        {
            if(w.GetIsAttacking())
                w.StopAttack(reason);
        }
    }

    public void StutterWeapon(float seconds)
    {
        foreach (BaseWeapons w in EquippedWeapons)
            w.StutterCharge(seconds);
    }
}
