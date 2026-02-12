using UnityEngine;

public class AOEIndicator : MonoBehaviour
{
    [SerializeField] private float duration = 0.4f;
    [SerializeField] private AnimationCurve scaleCurve;
    [SerializeField] private AnimationCurve alphaCurve;

    private SpriteRenderer sr;
    private float timer;

    public void Init(float radius)
    {
        transform.localScale = Vector3.one * radius * 2f;
    }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float t = timer / duration;

        if (t >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        float scaleMul = scaleCurve.Evaluate(t);
        float alpha = alphaCurve.Evaluate(t);

        transform.localScale *= scaleMul;

        Color c = sr.color;
        c.a = alpha;
        sr.color = c;
    }
}
