using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AmmoPoolUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private AmmoStash stash;
    [SerializeField] private AmmoType ammoType;

    [Header("UI")]
    [SerializeField] private Image fill;
    [SerializeField] private TextMeshProUGUI amountText;

    public void Init(AmmoStash stash)
    {
        this.stash = stash;

        stash.Changed += OnChanged;

        // initial push
        OnChanged(ammoType, stash.GetCurrent(ammoType), stash.GetMax(ammoType));
    }

    private void OnDisable()
    {
        if (!stash) return;
        stash.Changed -= OnChanged;
    }

    private void OnChanged(AmmoType type, int current, int max)
    {
        if (type != ammoType) return;

        if (fill) fill.fillAmount = (max <= 0) ? 0f : (float)current / max;
        if (amountText) amountText.text = $"{current}/{max}";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        amountText.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        amountText.gameObject.SetActive(false);
    }
}
