using UnityEngine;
using UnityEngine.UI;

public class MechIconComponent : MonoBehaviour
{
    private SpriteRenderer sr;

    [SerializeField] private Sprite baseIcon;
    [SerializeField] private Sprite diedIcon;


    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Died(BaseHealthComponent healthComp)
    {
        sr.sprite = diedIcon;
    }
}
