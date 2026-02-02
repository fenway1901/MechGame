using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarderPulse : MonoBehaviour
{
    [SerializeField] private SpriteRenderer border;
    [SerializeField] private Color pulseColor;
    [SerializeField] private float pulseSpeed = 6f;
    [SerializeField, Range(0f, 1f)] private float minAlpha = 0.35f;
    [SerializeField, Range(0f, 1f)] private float maxAlpha = 1f;
    [SerializeField] private bool useUnscaledTime = true;

    private readonly HashSet<int> targeters = new HashSet<int>();
    private Coroutine routine;
    private Color baseColor;

    private void Awake()
    {
        if (!border) border = GetComponent<SpriteRenderer>();
        baseColor = border ? border.color : Color.white;
    }

    public bool HasTargeters()
    {
        if (targeters.Count > 0)
            return true;
        else
            return false;
    }

    public void SetBaseColor(Color c)
    {
        baseColor = c;
        if (border && routine == null) border.color = baseColor;
    }

    public void AddTargeter(int targeterId)
    {
        if (!border) return;

        if (targeters.Add(targeterId))
        {
            if (targeters.Count == 1)
                routine = StartCoroutine(Pulse());
        }
    }

    public void RemoveTargeter(int targeterId)
    {
        if (!border) return;

        if (targeters.Remove(targeterId))
        {
            if (targeters.Count == 0)
                StopPulse();
        }
    }

    public void ClearAllTargeters()
    {
        targeters.Clear();
        StopPulse();
    }

    private void StopPulse()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }

        if (border) border.color = baseColor;
    }

    private IEnumerator Pulse()
    {
        while (true)
        {
            float t = useUnscaledTime ? Time.unscaledTime : Time.time;
            float wave01 = (Mathf.Sin(t * pulseSpeed) + 1f) * 0.5f;
            float a = Mathf.Lerp(minAlpha, maxAlpha, wave01);

            Color c = pulseColor;
            c.a = a;

            border.color = c;
            yield return null;
        }
    }
}
