using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatusIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private BuffDefinition buff;

    [SerializeField] private GameObject textPanel;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image icon;
    [SerializeField] private Image timeDuration;
    [SerializeField] private TextMeshProUGUI timerTxt;

    private float remaining;

    private void Update()
    {
        if (remaining >= 0)
        {
            remaining -= Time.deltaTime;

            timeDuration.fillAmount = remaining / buff.durationSeconds;
            timerTxt.text = remaining.ToString("F0");

            if (remaining <= 0f)
            {
                // Remove it here
                RemoveStatus();
            }
        }
    }

    public void Init(BuffDefinition buff)
    {
        this.buff = buff;
        icon.sprite = buff.icon;
        text.text = buff.description;

        if (buff.durationSeconds <= 0)
        {
            // infinite
            remaining = -1f;
            timeDuration.gameObject.SetActive(false);
        }
        else
        {
            remaining = buff.durationSeconds;
            timeDuration.fillAmount = 1f;
        }

        textPanel.SetActive(false);
        timerTxt.gameObject.SetActive(false);
    }

    public void RemoveStatus()
    {
        Destroy(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (textPanel)
        {
            if (remaining >= 0)
                timerTxt.gameObject.SetActive(true);

            textPanel.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (textPanel)
        {
            if (remaining >= 0)
                timerTxt.gameObject.SetActive(false);
            
            textPanel.SetActive(false);
        }
    }
}
