using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/MoveAndShoot")]
public class MoveAndShootAction : AIAction
{
    public float preferredRange = 8f;
    public float weightIfInRange = 10f;
    public float weightIfFar = 5f;

    public override float Score(in AIContext ctx)
    {
        if (ctx.target == null) return 0f;

        float dist = Vector2.Distance(ctx.self.transform.position, ctx.target.position);

        // Prefer this action if we can shoot or get into range
        if (dist <= preferredRange)
            return weightIfInRange;
        else if (dist <= preferredRange * 1.5f)
            return weightIfFar;
        else
            return 0f;
    }

    public override void Execute(AIContext ctx)
    {
        // Example hooks to your systems:
        ctx.self.MoveTowards(ctx.target.position, stopAtRange: preferredRange);
        ctx.self.TryFireWeaponsAt(ctx.target);
    }
}

