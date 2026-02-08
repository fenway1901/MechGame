using UnityEngine;
using UnityEngine.UI;

public class WeaponIcon : MonoBehaviour
{
    [SerializeField] private Color activeColor;
    [SerializeField] private Color deactiveColor;
    
    [SerializeField] private Image icon;
    [SerializeField] private Image boarderColors;

    [SerializeField] private Image progressFill;
    [SerializeField] private bool hideProgressWhenStandby = true;
    [SerializeField] private bool invertFill = false;

    [Header("Progress Colors")]
    [SerializeField] private Color chargingColor;
    [SerializeField] private Color cooldownColor;
    [SerializeField] private Color reloadColor;
    [SerializeField] private Color standbyColor;

    private BaseWeapons connectedWeapon;
    private BasePlayerMech playerMech;


    public void Init(BaseWeapons weapon, BaseMech mech)
    {
        connectedWeapon = weapon;

        playerMech = (mech as BasePlayerMech);
        playerMech.WeaponSelected += SetWeaponActive;

        SetIcon(weapon.GetIcon());
        boarderColors.color = deactiveColor;

        RefreshProgress();
    }


    #region Unity Functions

    private void OnDestroy()
    {
        if (playerMech != null)
            playerMech.WeaponSelected -= SetWeaponActive;
    }

    private void Update()
    {
        if (connectedWeapon == null || progressFill == null) return;
        RefreshProgress();
    }

    #endregion

    private void RefreshProgress()
    {
        BaseWeapons.WeaponUIPhase phase = connectedWeapon.GetUIPhase(out float duration, out float endTime);

        if (phase == BaseWeapons.WeaponUIPhase.Standby || duration <= 0f)
        {
            progressFill.fillAmount = 0f;
            progressFill.color = standbyColor;
            progressFill.enabled = !hideProgressWhenStandby;
            return;
        }

        float progress01 = connectedWeapon.GetUIPhaseProgress();
        if (invertFill) progress01 = 1f - progress01;

        progressFill.fillAmount = progress01;
        progressFill.color = GetPhaseColor(phase);
        progressFill.enabled = true;
    }

    private Color GetPhaseColor(BaseWeapons.WeaponUIPhase phase)
    {
        return phase switch
        {
            BaseWeapons.WeaponUIPhase.Charging => chargingColor,
            BaseWeapons.WeaponUIPhase.CoolingDown => cooldownColor,
            BaseWeapons.WeaponUIPhase.Reloading => reloadColor,
            _ => standbyColor
        };
    }

    private void SetWeaponActive(BaseWeapons weapon)
    {
        if (weapon == null)
            return;

        if (weapon == connectedWeapon)
            boarderColors.gameObject.SetActive(true);
        else
            boarderColors.gameObject.SetActive(false);
    }

    private void SetIcon(Sprite image) 
    { 
        icon.sprite = image;
    }
}
