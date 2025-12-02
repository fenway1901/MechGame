using System.Collections;
using UnityEngine;

public class MechBrain : MonoBehaviour
{
    [Header("Behavior")]
    public AIProfile profile;          // holds list of actions + tuning
    public Transform currentTarget;
    public Objective currentObjective;
    public Vector3 desiredPosition;

    [SerializeField] float thinkInterval = 0.5f;

    void Start()
    {
        StartCoroutine(ThinkLoop());
    }

    IEnumerator ThinkLoop()
    {
        while (true)
        {
            Think();
            yield return new WaitForSeconds(thinkInterval);
        }
    }

    void Think()
    {
        // 1. Update perception
        UpdatePerception();

        // 2. Build context
        AIContext ctx = new AIContext
        {
            self = this,
            target = currentTarget,
            currentObjective = currentObjective,
            director = EnemyCombatDirector.instance
        };

        // 3. Score actions
        AIAction best = null;
        float bestScore = float.NegativeInfinity;

        foreach (var action in profile.actions)
        {
            float score = action.Score(ctx);

            // add per-profile weights, randomness, difficulty scaling here if desired
            if (score > bestScore)
            {
                bestScore = score;
                best = action;
            }
        }

        // 4. Execute best
        if (best != null && bestScore > 0f)
        {
            best.Execute(ctx);
        }
    }

    void UpdatePerception()
    {
        if (currentObjective != null)
        {
            currentTarget = currentObjective.GetTarget(this);
            desiredPosition = currentObjective.GetDesiredPosition(this);
        }
        else
        {
            // Fallback behaviour if no objective assigned
            currentTarget = null;
            desiredPosition = transform.position;
        }
    }

    // Stub: call your existing movement system
    public void MoveTowards(Vector2 worldPos, float stopAtRange)
    {
        // plug into your tactical movement / MechMovementAgent here
    }

    public void TryFireWeaponsAt(Transform target)
    {
        // call into your weapon system
    }
}

