using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Move And Shoot")]
public class MoveAndShootAction : AIAction
{
    [Header("Scoring")]
    public float baseScore = 10f;
    public float inRangeBonus = 5f;
    public float outOfRangePenalty = 5f;
    public float tooFarMultiplier = 1.5f;   // don't consider targets beyond range * this
    public float noAmmoPenalty = 100f;

    public override float Score(in AIContext ctx)
    {
        if (ctx.target == null)
            return 0f;

        // Ask the mech for its preferred weapon vs this target
        BaseWeapons weapon = ctx.self.GetBestWeapon(ctx.target);
        if (weapon == null)
            return 0f;

        float weaponRange = weapon.GetRange();           // comes from BaseWeapons :contentReference[oaicite:22]{index=22}
        float distance = Vector3.Distance(ctx.self.transform.position,
                                          ctx.target.position);

        // If way too far, let some "move/approach" or "reposition" action handle it
        //if (distance > weaponRange * tooFarMultiplier)
        //    return 0f;

        float score = baseScore;

        // Prefer when in effective range
        if (distance <= weaponRange)
            score += inRangeBonus;
        else
            score -= outOfRangePenalty;

        // If out of ammo, heavily penalize so this action is effectively ignored
        //if (weapon.GetCurrentAmmo() <= 0)
        //    score -= noAmmoPenalty;

        return score;
    }

    public override void Execute(AIContext ctx)
    {
        if (ctx.target == null)
        {
            ctx.self.ClearTargetedLimb();
            return;
        }

        BaseWeapons weapon = ctx.self.GetBestWeapon(ctx.target);
        if (weapon == null)
        {
            ctx.self.ClearTargetedLimb();
            return;
        }

        // Get Target Limb and set it
        BasePlayerMech pMech = ctx.target.GetComponent<BasePlayerMech>();
        BaseLimb targetLimb = (pMech != null)
            ? ctx.self.GetTargetLimb(pMech.spawnedLayout.transform)
            : null;

        ctx.self.SetTargetedLimb(targetLimb);

        if (weapon.GetIsAttacking())
            return;

        if (weapon.usesAmmo)
        {
            if (weapon.GetCurrentAmmo() < weapon.GetAmmoUsedPerShot() || weapon.GetCurrentAmmo() == 0)
            {
                Debug.Log(ctx.self.name + " is reloading " + weapon.name);

                if(!weapon.GetIsReloading())
                    weapon.Reload();

                return;
            }
        }
        
        Transform aimTransform = targetLimb != null ? targetLimb.transform : ctx.target;
        float weaponRange = weapon.GetRange();

        // Charge at player
        ctx.self.MoveTowards(ctx.target, weaponRange * 0.9f);

        // once distance is inside then attack
        float distance = Vector3.Distance(ctx.self.transform.position, ctx.target.position);

        if (distance <= weaponRange)
        {
            Debug.Log("Attacking with dersired weapon");
            ctx.self.AttackWithDesiredWeapon(weapon, aimTransform, stopMovement: false);
        }
    }
}