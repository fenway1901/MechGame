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
        public MountIcon icon; // Use for maybe a visual element of where the weapon is
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

        if (m.icon != null && m.slot == weapon.GetWeaponStats().Slot)
        {
            weapon.WeaponStuttered += m.icon.WeaponStuttered;

            m.icon.SetWeaponImage(weapon.GetIcon());
        }

        return true;
    }

    public void DisableAllweapons(string reason)
    {
        foreach (BaseWeapons w in EquippedWeapons)
        {
            if(w.GetIsAttacking())
                w.StopAttack(reason);
        }

        // Turn off icons here
        foreach (Mount m in mounts)
            if (m.equipped != null)
                m.icon?.WeaponDestroyed();
    }

    public void StutterWeapon(float seconds)
    {
        foreach (BaseWeapons w in EquippedWeapons)
            w.StutterCharge(seconds);
    }
}
