using Unity.VisualScripting;
using UnityEngine;

public class MechHealthComponent : BaseHealthComponent
{
    public GameObject _AttachedMech;

    protected virtual void Start()
    {
        //SetMaxHealth(_AttachedMech.GetComponent<StatsComponent>().Get(StatType.Mech_MaxHealth));
    }

    protected override void Death(BaseHealthComponent sender)
    {
        base.Death(sender);
    }

    public override void TakeDamage(float amount, float armorPen = 0.0f)
    {
        base.TakeDamage(amount);

        //Debug.Log(CurrentHealth + " " + name + " current heatlh");
    }

    public void Kill()
    {
        Death(this);
    }
}
