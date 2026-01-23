using UnityEngine;

public class MechHealthComponent : BaseHealthComponent
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
