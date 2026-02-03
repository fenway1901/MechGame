using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MechBrain : MonoBehaviour
{
    [Header("Behavior")]
    public AIProfile profile;
    public Transform currentTarget;
    public Objective currentObjective;
    public Vector3 desiredPosition;

    [SerializeField] float thinkInterval = 0.5f;

    private NavMeshAgent agent;

    public BaseMech mech;

    private BaseLimb currentTargetLimb;
    private int targeterId;


    #region Unity Function

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

        targeterId = GetInstanceID();
    }

    void Start()
    {
        StartCoroutine(ThinkLoop());
    }

    private void OnDisable()
    {
        
    }

    #endregion


    #region Thinking Functions

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

        UpdatePerception();

        // getting context
        AIContext ctx = new AIContext
        {
            self = this,
            target = currentTarget,
            currentObjective = currentObjective,
            director = EnemyCombatDirector.instance
        };

        // getting score
        AIAction best = null;
        float bestScore = float.NegativeInfinity;

        foreach (var action in profile.actions)
        {
            float score = action.Score(ctx);

            if (score > bestScore)
            {
                bestScore = score;
                best = action;
            }
        }

        // execute
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
            // incase no behaviour is found
            currentTarget = null;
            desiredPosition = transform.position;
        }
    }

    #endregion


    #region Attack Functions

    public void AttackWithDesiredWeapon(BaseWeapons weapon, Transform target, bool stopMovement = false)
    {
        if (weapon == null || target == null || mech == null)
            return;

        if (stopMovement)
            StopMovement();

        mech.activeWeapon = weapon;

        // Attack is starting here
        weapon.Attack(target.gameObject);
    }


    public BaseWeapons GetBestWeapon(Transform target)
    {
        if (mech == null || mech.Weapons == null || mech.Weapons.Count == 0)
            return null;

        // Test: just their first weapon
        return mech.Weapons[0];

        // Later: pick off of damage, range, time to attack, aoe damage, etc
    }

    // Returns a limb on the target mech, or null if none found
    public BaseLimb GetTargetLimb(Transform mechRoot)
    {
        if (mechRoot == null) return null;

        // if limb already given skip rest
        BaseLimb limb = mechRoot.GetComponent<BaseLimb>();
        if (limb != null && !limb.isDestroyed)
            return limb;

        // treating as mech root getting all limbs
        BaseLimb[] limbs = mechRoot.GetComponentsInChildren<BaseLimb>();
        if (limbs == null || limbs.Length == 0)
            return null;

        // Test: destroying each limb in order
        // Later: evealuate limb health, killing limb status effect, etc
        foreach (var l in limbs)
        {
            if (!l.isDestroyed)
                return l;
        }

        // All limbs destroyed, end sequence will be running so just let it keep "attacking"
        return limbs[0];
    }

    #endregion


    #region Movement Functions

    public void MoveTowards(Vector2 worldPos, float stopAtRange)
    {
        if (agent == null)
        {
            Debug.Log(name + " does not have an assigned Move Agent!");
            return;
        }

        // Convert Vector2 to Vector3 for NavMesh
        Vector3 destination = new Vector3(worldPos.x, transform.position.y, worldPos.y);

        agent.stoppingDistance = stopAtRange;
        agent.isStopped = false;
        agent.SetDestination(destination);
    }

    // Incase I want 3D movement
    public void MoveTowards(Vector3 worldPos, float stopAtRange)
    {
        if (agent == null)
        {
            Debug.Log(name + " does not have an assigned Move Agent!");
            return;
        }

        agent.stoppingDistance = stopAtRange;
        agent.isStopped = false;
        agent.SetDestination(worldPos);
    }

    // Incase I want to use a transform
    public void MoveTowards(Transform target, float stopAtRange)
    {
        if (agent == null)
        {
            Debug.Log(name + " does not have an assigned Move Agent!");
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


    #region Targeting Functions

    public void SetTargetedLimb(BaseLimb limb)
    {
        if (currentTargetLimb == limb) return;

        if(currentTargetLimb != null)
            currentTargetLimb.RemoveTargeter(targeterId);

        currentTargetLimb = limb;

        if(currentTargetLimb != null)
            currentTargetLimb.AddTargeter(targeterId);
    }

    public void ClearTargetedLimb()
    {
        SetTargetedLimb(null);
    }

    #endregion
}

