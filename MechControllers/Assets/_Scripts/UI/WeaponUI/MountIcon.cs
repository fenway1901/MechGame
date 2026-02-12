using System.Collections;
using UnityEngine;

public class MountIcon : MonoBehaviour
{
    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private SpriteRenderer iconStatus;
    [SerializeField] private Color destroyColor;
    [SerializeField] private Color stutterColor;
    [SerializeField] private Sprite destroyedIcon;
    [SerializeField] private Sprite stutterIcon;

    private bool isDestroyed = false;

    public void SetWeaponImage(Sprite icon)
    {
        this.icon.sprite = icon;
    }

    public void WeaponDestroyed()
    {
        isDestroyed = true;
        iconStatus.gameObject.SetActive(true);
        iconStatus.sprite = destroyedIcon;
        iconStatus.color = destroyColor;
    }

    public void WeaponRepaired()
    {
        iconStatus.gameObject.SetActive(false);
    }

    public void WeaponStuttered(float seconds)
    {
        if (isDestroyed) return;

        Debug.Log(name + " is stuttered");

        iconStatus.gameObject.SetActive(true);
        iconStatus.sprite = stutterIcon;
        iconStatus.color = stutterColor;
        
        StartCoroutine(StutterSequence(seconds));
    }

    private IEnumerator StutterSequence(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        WeaponRepaired();
    }
}
