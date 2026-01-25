using UnityEngine;

public class SingleTargetHitEffect : MonoBehaviour, IHitEffect
{
    public void Apply(BaseWeapons weapon, GameObject target, Vector3 impactPoint)
    {
        if (!target)
        {
            Debug.LogWarning(weapon.name + " has not primaryTarget");
            return;
        }

        Debug.Log("Weapon target is " + target.name);

        BaseHealthComponent hp = target.GetComponent<BaseHealthComponent>();

        if (hp) hp.TakeDamage(weapon.GetDamage());
        else Debug.Log(weapon.name + " target does not have a BaseHealthComponent");
    }
}
