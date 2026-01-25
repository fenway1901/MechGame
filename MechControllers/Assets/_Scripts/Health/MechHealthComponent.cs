using UnityEngine;

public class MechHealthComponent : BaseHealthComponent
{
    public GameObject _AttachedMech;

    protected override void Death(BaseHealthComponent sender)
    {
        base.Death(sender);
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);

        Debug.Log(CurrentHealth + " " + name + " current heatlh");
    }

    public void Kill()
    {
        Death(this);
    }
}
