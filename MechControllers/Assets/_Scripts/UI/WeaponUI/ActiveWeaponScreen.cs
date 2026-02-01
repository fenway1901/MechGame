using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActiveWeaponScreen : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Color chargeColor;
    [SerializeField] private Color cooldownColor;
    [SerializeField] private Color standbyColor;
    [SerializeField] private string chargeText;
    [SerializeField] private string cooldownText;

    [SerializeField] private Image fill;
    [SerializeField] private Image icon;
    [SerializeField] private Image boardercolor;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI statusTxt;
    [SerializeField] private TextMeshProUGUI currentAmmo;
    [SerializeField] private TextMeshProUGUI maxAmmo;

    private BaseWeapons assignedWeapon = new BaseWeapons();

    private float chargeEndTime;
    private float chargeDuration;
    private bool charging;

    private float cooldownEndTime;
    private float cooldownDuration;
    private bool cooling;

    private bool cancelAttack;

    public void Init(BaseMech mech)
    {
        (mech as BasePlayerMech).WeaponSelected += SetNewWeapon;
    }

    // Update is called once per frame
    void Update()
    {
        if (cancelAttack)
        {
            // Do cancel stuff here
        }

        if (charging)
        {
            float elapsed = chargeDuration - (chargeEndTime - Time.time);
            slider.value = Mathf.Clamp(elapsed, 0f, chargeDuration);

            if (Time.time >= chargeEndTime)
            {
                charging = false;
                slider.value = chargeDuration;
            }
        }

        if (cooling)
        {
            float remaining = cooldownEndTime - Time.time;
            slider.value = Mathf.Clamp(remaining, 0f, cooldownDuration);

            if (Time.time >= cooldownEndTime)
            {
                cooling = false;
                slider.value = 0f;
            }
        }
    }

    public void SetNewWeapon(BaseWeapons weapon)
    {
        if (weapon == null)
        {
            Debug.LogWarning("Trying to assign a null weapon to active panel");
            return;
        }

        assignedWeapon = weapon;

        icon.sprite = assignedWeapon.GetIcon();
        title.text = assignedWeapon.displayName;
        maxAmmo.text = assignedWeapon.BaseMaxAmmo.ToString();
        currentAmmo.text = assignedWeapon.GetCurrentAmmo().ToString();

        assignedWeapon.Reloaded += UpdateAmmo;
        assignedWeapon.AmmoFired += UpdateAmmo;
        assignedWeapon.WeaponCharging += ChargingWeapon;
        assignedWeapon.WeaponCooling += CoolingWeapon;
        assignedWeapon.CancelAttack += AttackHasStopped;
    }

    private void UpdateAmmo(BaseWeapons weapon, float ammo)
    {
        if (weapon != assignedWeapon) return;

        currentAmmo.text = ammo.ToString();
    }

    private void ChargingWeapon(BaseWeapons weapon, float time)
    {
        if (weapon != assignedWeapon) return;

        fill.color = chargeColor;
        boardercolor.color = chargeColor;
        statusTxt.text = chargeText;
        
        chargeDuration = time;
        chargeEndTime = Time.time + time;

        slider.maxValue = time;
        slider.value = 0f;

        charging = true;
        cooling = false;
    }

    private void CoolingWeapon(BaseWeapons weapon, float time)
    {
        if (weapon != assignedWeapon) return;

        fill.color = cooldownColor;
        boardercolor.color = cooldownColor;
        statusTxt.text = cooldownText;

        cooldownDuration = time;
        cooldownEndTime = Time.time + time;

        slider.maxValue = time;
        slider.value = 0;
        
        cooling = true;
        charging = false;
    }

    private void AttackHasStopped(BaseWeapons weapon)
    {
        if (weapon != assignedWeapon) return;

        // show cool effect for cancleing attack
        // Make bar red, show cancel text and each weapon should have a cooldown time until they can attack again (canceling should be a big deal)
        slider.value = 0;
        statusTxt.text = "Stand By";
        boardercolor.color = standbyColor;
    }

    // Should also do a reload timer here
}
