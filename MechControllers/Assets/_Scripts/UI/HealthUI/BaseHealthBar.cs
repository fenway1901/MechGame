using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BaseHealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;

    [SerializeField] protected Image fill;
    [SerializeField] protected Image lagFill;
    [SerializeField] protected Image icon;
    [SerializeField] protected TextMeshProUGUI title;

    [Header("Timing")]
    [SerializeField] private float chipDelay;
    [SerializeField] private float chipShrinkTime;

    private Coroutine chipRoutine;
    private float current01 = 1f;

    protected virtual void Awake()
    {
        slider = GetComponent<Slider>();

        current01 = slider.normalizedValue;
        ApplyInstant(current01);
    }


    public virtual void DamageTaken(BaseHealthComponent comp, float damage, float currentHealth)
    {
        //Debug.Log("parent: " + comp.gameObject.name + " took damage " + damage);
        float old01 = current01;

        slider.value = currentHealth;

        float new01 = slider.normalizedValue;
        current01 = new01;

        Color col = gradient.Evaluate(slider.normalizedValue);

        fill.color = col;

        if (icon)
            icon.color = col;

        SetFillAmount(fill, new01);

        if (new01 >= old01)
        {
            if (chipRoutine != null) { StopCoroutine(chipRoutine); chipRoutine = null; }
            SetFillAmount(lagFill, new01);
            return;
        }

        if (chipRoutine != null) StopCoroutine(chipRoutine);
        chipRoutine = StartCoroutine(AnimateChip(old01, new01));
    }

    public virtual void SetName(string name) { title.text = name; }
    public virtual void SetHealth(float health) 
    { 
        slider.value = health;
        current01 = slider.normalizedValue;
        ApplyInstant(current01);
    }

    public virtual void SetMaxHealth(float max)
    {
        slider.maxValue = max;
        slider.value = max;

        current01 = 1f;
        fill.color = gradient.Evaluate(1f);

        if (icon)
            icon.color = gradient.Evaluate(1f);

        ApplyInstant(1f);
    }


    #region Chip Functions

    private void ApplyInstant(float t01)
    {
        if (chipRoutine != null) { StopCoroutine(chipRoutine); chipRoutine = null; }
        SetFillAmount(fill, t01);
        SetFillAmount(lagFill, t01);
    }

    private IEnumerator AnimateChip(float from01, float to01)
    {
        SetFillAmount(lagFill, from01);

        yield return new WaitForSeconds(chipDelay);

        float elapsed = 0f;

        while (elapsed < chipShrinkTime)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / chipShrinkTime);
            SetFillAmount(lagFill, Mathf.Lerp(from01, to01, t));
            yield return null;
        }

        SetFillAmount(lagFill, to01);
        chipRoutine = null;
    }

    private static void SetFillAmount(Image img, float t01)
    {
        if (!img) return;

        img.fillAmount = t01;
    }

    #endregion
}
