using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MechBrain : MonoBehaviour
{
    [Header("Behavior")]
    public AIProfile profile;          // holds list of actions + tuning
    public Transform currentTarget;
    public Objective currentObjective;
    public Vector3 desiredPosition;

    [SerializeField] float thinkInterval = 0.5f;

    private NavMeshAgent agent;

    public BaseMech mech;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.Log(name + " MechBrain is not on the same object as the MoveAgent component!!!");
            return;
        }

        agent.updateRotation = false;
        agent.updateUpAxis = false;

    }

    void Start()
    {
        StartCoroutine(ThinkLoop());
    }

    // TO DO update this do be in Update() so  a lag spike dosnt slow down the thinking time (or maybe not, might make lag worse)
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
        //Debug.Log("Enemy thinking");

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

    #region Attack Functions

    public void AttackWithDesiredWeapon(BaseWeapons weapon, Transform target, bool stopMovement = false)
    {
        if (weapon == null || target == null || mech == null)
            return;

        if (stopMovement)
            StopMovement();

        // Set active weapon so BaseMech/AttackManager can use it
        mech.activeWeapon = weapon;

        // Use your existing attack flow; equivalent to BaseMech.TargetSelected(target)
        weapon.Attack(target.gameObject);
        //AttackManager.instance.AttackEnemy(target.gameObject, weapon);
    }


    public BaseWeapons GetBestWeapon(Transform target)
    {
        if (mech == null || mech.Weapons == null || mech.Weapons.Count == 0)
            return null;

        // Simple: first weapon
        return mech.Weapons[0];

        // Later: pick by range/damage, or based on the specific target.
    }

    // Returns a limb on the target mech, or null if none found
    public BaseLimb GetTargetLimb(Transform mechRoot)
    {
        if (mechRoot == null) return null;

        // 1) If we were given a limb directly, just use it
        BaseLimb limb = mechRoot.GetComponent<BaseLimb>();
        if (limb != null && !limb.isDestroyed)
            return limb;

        // 2) Otherwise, treat this as a mech root and look for child limbs
        BaseLimb[] limbs = mechRoot.GetComponentsInChildren<BaseLimb>();
        if (limbs == null || limbs.Length == 0)
            return null;

        // Simple prototype: first non-destroyed limb
        foreach (var l in limbs)
        {
            if (!l.isDestroyed)
                return l;
        }

        // All limbs destroyed; return any as fallback (or null if you want to stop)
        return limbs[0];
    }

    #endregion


    #region Movement Functions


    // Called by your AIActions
    public void MoveTowards(Vector2 worldPos, float stopAtRange)
    {
        if (agent == null)
        {
            Debug.Log(name + " does not have an assigned Move Agent!!!");
            return;
        }

        // Convert Vector2 to Vector3 for NavMesh
        // Assuming X/Y = horizontal plane; keep current Z (or Y depending on setup)
        Vector3 destination = new Vector3(worldPos.x, transform.position.y, worldPos.y);

        agent.stoppingDistance = stopAtRange;
        agent.isStopped = false;
        agent.SetDestination(destination);
    }

    // Overload if I have 3D world Position
    public void MoveTowards(Vector3 worldPos, float stopAtRange)
    {
        if (agent == null)
        {
            Debug.Log(name + " does not have an assigned Move Agent!!!");
            return;
        }

        agent.stoppingDistance = stopAtRange;
        agent.isStopped = false;
        agent.SetDestination(worldPos);
    }

    // Overload if I have a Transform
    public void MoveTowards(Transform target, float stopAtRange)
    {
        if (agent == null)
        {
            Debug.Log(name + " does not have an assigned Move Agent!!!");
            return;
        }

        if (target == null) return;
        MoveTowards(new Vector2(target.position.x, target.position.z), stopAtRange);
    }

    public bool HasReachedDestination(float extraTolerance = 0.1f)
    {
        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance + extraTolerance)
        {
            return true;
        }

        return false;
    }

    public void StopMovement()
    {
        if (agent == null) return;
        agent.isStopped = true;
        agent.ResetPath();
    }

    #endregion
}

