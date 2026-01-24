using UnityEngine;

public interface IHitEffect
{
    void Apply(BaseWeapons weapon, GameObject primaryTarget, Vector3 impactPoint);
}
