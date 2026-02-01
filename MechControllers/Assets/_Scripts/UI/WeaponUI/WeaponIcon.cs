using UnityEngine;
using UnityEngine.UI;

public class WeaponIcon : MonoBehaviour
{
    [SerializeField] private Color activeColor;
    [SerializeField] private Color deactiveColor;
    [SerializeField] private Image icon;
    [SerializeField] private Image boarderColors;
    private BaseWeapons connectedWeapon;


    public void Init(BaseWeapons weapon, BaseMech mech)
    {
        connectedWeapon = null;

        (mech as BasePlayerMech).WeaponSelected += SetWeaponActive;

        connectedWeapon = weapon;
        SetIcon(weapon.GetIcon());
        boarderColors.color = deactiveColor;
    }

    private void SetWeaponActive(BaseWeapons weapon)
    {
        if (weapon == null)
            return;

        if (weapon == connectedWeapon)
            boarderColors.color = activeColor;
        else
            boarderColors.color = deactiveColor;
    }

    private void SetIcon(Sprite image) 
    { 
        icon.sprite = image;
    }
}
