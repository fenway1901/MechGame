using UnityEngine;

[RequireComponent(typeof(StatsComponent), typeof(BaseLimb))]
public class BaseLimbStats : MonoBehaviour
{
    [SerializeField] private string[] tags;
    [SerializeField] private LimbSlot slot;

    public LimbSlot Slot => slot;
    public StatsComponent Stats { get; private set; }

    private BaseLimb limb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Awake()
    {
        Stats = GetComponent<StatsComponent>();
        limb = GetComponent<BaseLimb>();

        //Stats.SetBase(StatType.Limb_Armor, limb.armor);
        //Stats.SetBase(StatType.Limb_Speed, limb.moveSpeed);
        //Stats.SetBase(StatType.Limb_Weight, limb.weight);

    }

    public bool HasTag(string t)
    {
        if (string.IsNullOrEmpty(t) || tags == null) return false;
        for (int i = 0; i < tags.Length; i++)
            if (tags[i] == t) return true;
        return false;
    }

    public LimbSlot GetLimbSlot() { return slot; }
}
