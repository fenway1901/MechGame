using UnityEditor.UIElements;
using UnityEngine;

public class LimbHealthComponent : BaseHealthComponent
{
    [SerializeField] private MechHealthComponent mechHealthComponent;

    [Tooltip("Damage to the main mech on limb death")]
    public float deathDamage;
    [SerializeField] private bool killLimbKillsMech = false;
    [Tooltip("Damage that will leak over to haul on hit")]
    [Range(0f, 1f)] public float leakToHullPercent;
    [Tooltip("If super strong hit will transfer rest to haul")]
    [Range(0f, 1f)] public float overkillToHullPercent;

    private BaseLimb limb;

    protected override void Awake()
    {
        base.Awake();

        limb = GetComponent<BaseLimb>();
        mechHealthComponent = transform.parent.gameObject.GetComponent<MechHealthComponent>();
    } 


    public override void TakeDamage(float amount)
    {
        if (amount <= 0f || CurrentHealth <= 0f) return;
        // TO DO: Currently armor is just a flat negations might want to change or remove
        amount = Mathf.Clamp(amount - limb.armor, 0f, amount);

        float leak = Mathf.Clamp01(leakToHullPercent);
        float hullDamage = amount * leak;
        float limbDamage = amount - hullDamage;


        // apply damage to limb
        float before = CurrentHealth;
        base.TakeDamage(limbDamage);

        Debug.Log("limb damaged " + CurrentHealth);

        // leak damage to hull
        if (hullDamage > 0f)
            mechHealthComponent.TakeDamage(hullDamage);

        // if overkill happens
        if (CurrentHealth <= 0f && overkillToHullPercent > 0f)
        {
            float remaingAtHit = before;
            float overkill = Mathf.Max(0f, limbDamage - remaingAtHit);
            float extraHull = overkill * overkillToHullPercent;

            if(extraHull > 0f)
                mechHealthComponent.TakeDamage(extraHull);
        }
    }

    public override void Heal(float amount)
    {
        base.Heal(amount);
    }


    protected override void Death(BaseHealthComponent sender)
    {
        base.Death(sender);

        if(deathDamage > 0f)
            mechHealthComponent.TakeDamage(deathDamage);

        if (killLimbKillsMech)
            mechHealthComponent.Kill();

        limb.DestroyedLimb(true);
    }
}
