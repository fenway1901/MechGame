using TMPro;
using UnityEngine;

public class WeaponDisplay : MonoBehaviour
{
    public Sprite weaponIcon;
    public Sprite NumIcon;
    public TextMeshProUGUI title;
    public TextMeshProUGUI timerTxt;
    private float currentTime;
    private bool isRunning;


    private void Awake()
    {
        timerTxt.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;
        if (currentTime < 0) currentTime = 0;

        UpdateText();
    }

    private void UpdateText()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerTxt.text = currentTime.ToString("F2");
    }

    public void SetTimer(float time)
    {
        timerTxt.gameObject.SetActive(true);
        currentTime = time;
        isRunning = true;
        UpdateText();
    }

    public void StopTimer()
    {
        timerTxt.gameObject.SetActive(false);
        isRunning = false;
    }


}
