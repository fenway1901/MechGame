using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject _PlayerStatPanel;
    public GameObject _PlayerSilloPanel;
    public GameObject _LocalMapPanel;
    public GameObject _EnemySilloPanel;

    private GameObject playerStat_LimbSection;
    private GameObject playerStat_HullSection;

    [Header("Prefabs")]
    [Header("Health UI")]
    public GameObject limbHealthBarPrefab;

    private void Awake()
    {
        instance = this;

        if (!_PlayerStatPanel)
        {
            _PlayerStatPanel = GameObject.Find("Player Stat Panel").transform.Find("Screen").GetChild(0).gameObject;
            playerStat_LimbSection = _PlayerStatPanel.transform.Find("Limb Section").gameObject;
            playerStat_HullSection = _PlayerStatPanel.transform.Find("Hull Section").gameObject;
        }

        if(!_PlayerSilloPanel)
            _PlayerSilloPanel = GameObject.Find("Player Panel").transform.Find("Screen").GetChild(0).gameObject;

        if(!_LocalMapPanel)
            _LocalMapPanel = GameObject.Find("MinimapPanel").transform.Find("Screen").GetChild(0).gameObject;

        if(!_EnemySilloPanel)
            _EnemySilloPanel = GameObject.Find("TargetMechPanel").transform.Find("Screen").GetChild(0).gameObject;
    }


    #region Combat UI Set Up

    public void CreatePlayerHealthBars(BaseMech mech)
    {
        List<BaseLimb> limbs = mech.limbs;

        // Set up Hull Healthbar
        BaseHealthBar hullHB = playerStat_HullSection.GetComponent<BaseHealthBar>();
        hullHB.SetMaxHealth(mech.stats.Get(StatType.Mech_MaxHealth));
        mech.GetHealthComponent().Damaged += hullHB.DamageTaken;

        // Create and set up limb healthbars
        for (int i = 0; i < limbs.Count; ++i)
        {
            BaseHealthBar hb = Instantiate(limbHealthBarPrefab, playerStat_LimbSection.transform).GetComponent<BaseHealthBar>();
            hb.SetMaxHealth(limbs[i].GetLimbStats().Stats.Get(StatType.Limb_MaxHealth));
            limbs[i].GetHealthComponent().Damaged += hb.DamageTaken;
            hb.SetName(limbs[i].limbName);
        }
    }

    #endregion
}
