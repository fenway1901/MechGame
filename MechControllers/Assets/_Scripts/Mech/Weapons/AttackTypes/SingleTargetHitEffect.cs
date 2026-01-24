using UnityEngine;

public class SingleTargetHitEffect : MonoBehaviour, IHitEffect
{
    public void Apply(BaseWeapons weapon, GameObject primaryTarget, Vector3 impactPoint)
    {
        if (!primaryTarget)
        {
            Debug.LogWarning(weapon.name + " has not primaryTarget");
            return;
        }

        BaseHealthComponent hp = primaryTarget.GetComponent<BaseHealthComponent>();

        if (hp) hp.TakeDamage(weapon.GetDamage());
        else Debug.Log(weapon.name + " target does not have a BaseHealthComponent");
    }
}
