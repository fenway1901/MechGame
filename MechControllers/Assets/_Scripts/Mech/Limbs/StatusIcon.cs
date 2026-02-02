using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatusIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject textPanel;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image icon;


    public void Init(BuffDefinition buff)
    {
        icon.sprite = buff.icon;
        text.text = buff.description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (textPanel) textPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (textPanel) textPanel.SetActive(false);
    }
}
