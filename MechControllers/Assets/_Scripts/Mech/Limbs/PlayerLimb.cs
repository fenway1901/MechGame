using Unity.VisualScripting;
using UnityEngine;

public class PlayerLimb : BaseLimb
{
    [Header("Status Stat Variables")]
    [SerializeField] private GameObject statusWindow;
    [SerializeField] private GameObject statusIcon;

    protected override void ApplyDebuffs(float percent)
    {
        for (int i = 0; i < deathDebuffs.Count; ++i)
        {
            if (deathDebuffs[i].applied) continue;
            if (deathDebuffs[i].percentHealth < percent) continue;

            deathDebuffs[i].SetApplied(true);
            _AttachedMech.buffController.Apply(deathDebuffs[i].debuff, this, _AttachedMech.stats);
            StatusIcon status = Instantiate(statusIcon, statusWindow.transform).GetComponent<StatusIcon>();
            status.Init(deathDebuffs[i].debuff);
        }
    }
}
