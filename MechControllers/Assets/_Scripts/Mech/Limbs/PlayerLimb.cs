using UnityEngine;

public class PlayerLimb : BaseLimb
{
    [Header("Status Stat Variables")]
    [SerializeField] private GameObject statusWindow;
    [SerializeField] private GameObject statusIcon;

    protected override void ApplyDebuffs()
    {
        for (int i = 0; i < deathDebuffs.Count; ++i)
        {
            _AttachedMech.buffController.Apply(deathDebuffs[i], this, _AttachedMech.stats);
            StatusIcon status = Instantiate(statusIcon, statusWindow.transform).GetComponent<StatusIcon>();
            status.Init(deathDebuffs[i]);
        }
    }
}
