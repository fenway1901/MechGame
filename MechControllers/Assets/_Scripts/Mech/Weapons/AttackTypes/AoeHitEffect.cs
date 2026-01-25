using UnityEngine;
using UnityEngine.Analytics;

public class AoeHitEffect : MonoBehaviour, IHitEffect
{
    [SerializeField] private float baseRadius;
    private float totalRadius;
    [SerializeField] private LayerMask damageMask;
    [SerializeField] private bool useFalloff = true;
    [SerializeField] AnimationCurve falloff = AnimationCurve.Linear(0, 1, 1, 0);

    public void Apply(BaseWeapons weapon, GameObject primaryTarget, Vector3 impactPoint)
    {
        // PROTO: make sure there is a radius
        if(totalRadius == 0) { totalRadius = baseRadius; }

        // Check if limb was selected as target point
        if(primaryTarget.layer == 6)
        {
            impactPoint = primaryTarget.transform.parent.GetComponent<MechHealthComponent>()._AttachedMech.transform.position;
        }

        Collider[] hits = Physics.OverlapSphere(impactPoint, totalRadius, damageMask);


        foreach(Collider hit in hits)
        {
            BaseHealthComponent health = new BaseHealthComponent();

            if (hit.GetComponent<BaseMech>() != null)
            {
                // At the moment it will effec the main hull
                // Future will have to figure out the limb damage situation
                health = hit.GetComponent<BaseMech>().spawnedLayout.GetComponent<BaseHealthComponent>();
            }

            if (health == null) { continue; }

            float damage = weapon.GetDamage();
            if (useFalloff)
            {
                float t = Vector3.Distance(impactPoint, hit.transform.position) / totalRadius;
                damage *= falloff.Evaluate(t);
            }

            health.TakeDamage(damage);
        }
    }

    public void ChangeRadius(float amount)
    {
        totalRadius += amount;

        if(totalRadius <= 0)
        {
            totalRadius = 0.5f;
        }
    }
}
