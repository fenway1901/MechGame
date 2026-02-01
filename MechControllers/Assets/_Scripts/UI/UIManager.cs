using System.Collections.Generic;
using System.Xml;
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

    private GameObject weaponPanel;
    private GameObject weaponIconPanel;
    private ActiveWeaponScreen activeWeaponPanel;

    [Header("Prefabs")]
    [Header("Health UI")]
    [SerializeField] private GameObject limbHealthBarPrefab;

    [Header("Weapon UI")]
    [SerializeField] private GameObject weaponIconPrefab;
    private List<WeaponIcon> icons;

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

        if (!_LocalMapPanel)
        {
            _LocalMapPanel = GameObject.Find("MinimapPanel").transform.Find("Screen").GetChild(0).gameObject;
            weaponPanel = _LocalMapPanel.transform.Find("Weapon Section").gameObject;
            weaponIconPanel = weaponPanel.transform.Find("Icon Section").gameObject;
            activeWeaponPanel = weaponPanel.transform.Find("ActiveWeapon Section").gameObject.GetComponent<ActiveWeaponScreen>();
        }


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

    public void CreateWeaponUI(BaseMech mech)
    {
        List<BaseWeapons> weps = mech.GetWeapons();

        icons = new List<WeaponIcon>();

        // Set weps[0] here
        activeWeaponPanel.Init(mech);

        // Double check if they only have one weapon
        // Later right now it will just highlight it
        //if (weps.Count == 1)
        //    return;

        // Starting at 1 bc first weapon will be in active slot
        for(int i = 0; i < weps.Count; ++i)
        {
            WeaponIcon icon = Instantiate(weaponIconPrefab, weaponIconPanel.transform).GetComponent<WeaponIcon>();
            icon.Init(weps[i], mech);
            icons.Add(icon);
        }

    }

    #endregion
}
