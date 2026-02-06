using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices.WindowsRuntime;

public class ActiveWeaponScreen : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Color chargeColor;
    [SerializeField] private Color cooldownColor;
    [SerializeField] private Color reloadColor;
    [SerializeField] private Color standbyColor;
    [SerializeField] private string chargeText;
    [SerializeField] private string cooldownText;
    [SerializeField] private string reloadText;

    [SerializeField] private Image fill;
    [SerializeField] private Image icon;
    [SerializeField] private Image boardercolor;
    [SerializeField] private GameObject ammoTxtCounter;
    [SerializeField] private GameObject infinitySymbol;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI statusTxt;
    [SerializeField] private TextMeshProUGUI currentAmmo;
    [SerializeField] private TextMeshProUGUI maxAmmo;

    private BaseWeapons assignedWeapon = new BaseWeapons();

    private float endTime;
    private float duration;
    private bool counting;

    public void Init(BaseMech mech)
    {
        (mech as BasePlayerMech).WeaponSelected += SetNewWeapon;
    }

    // Update is called once per frame
    void Update()
    {
        if (counting)
        {
            float elapsed = duration - (endTime - Time.time);
            slider.value = Mathf.Clamp(elapsed, 0f, duration);

            if (Time.time >= endTime)
            {
                slider.value = duration;
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

        if (assignedWeapon.usesAmmo)
        {
            ammoTxtCounter.SetActive(true);
            infinitySymbol.SetActive(false);
            maxAmmo.text = assignedWeapon.BaseMaxAmmo.ToString();
            currentAmmo.text = assignedWeapon.GetCurrentAmmo().ToString();
        }
        else
        {
            ammoTxtCounter.SetActive(false);
            infinitySymbol.SetActive(true);
        }

        assignedWeapon.Reloaded += UpdateAmmo;
        assignedWeapon.Reloading += ReloadingGun;
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

    private void ReloadingGun(BaseWeapons weapon, float time)
    {
        if (weapon != assignedWeapon) return;

        SetUpBarAndText(reloadColor, reloadText, time);
    }

    private void ChargingWeapon(BaseWeapons weapon, float time)
    {
        if (weapon != assignedWeapon) return;

        SetUpBarAndText(chargeColor, chargeText, time);
    }

    private void CoolingWeapon(BaseWeapons weapon, float time)
    {
        if (weapon != assignedWeapon) return;

        SetUpBarAndText(cooldownColor, cooldownText, time);
    }

    private void SetUpBarAndText(Color color, string text, float time)
    {
        fill.color = color;
        boardercolor.color = color;
        statusTxt.text = text;

        duration = time;
        endTime = time + Time.time;

        slider.maxValue = time;
        slider.value = 0f;

        counting = true;
    }

    private void AttackHasStopped(BaseWeapons weapon)
    {
        if (weapon != assignedWeapon) return;

        // show cool effect for cancleing attack
        // Make bar red, show cancel text and each weapon should have a cooldown time until they can attack again (canceling should be a big deal)
        slider.value = 0;
        statusTxt.text = "Stand By";
        boardercolor.color = standbyColor;
        counting = false;
        fill.fillAmount = 0;
    }

    // Should also do a reload timer here
}
