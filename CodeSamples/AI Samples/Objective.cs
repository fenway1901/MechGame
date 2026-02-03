using UnityEngine;

public class Objective : MonoBehaviour
{
    [Header("General")]
    public string objectiveName = "Objective";
    [Range(0f, 1f)]
    public float priority = 1f;   // Used by CombatDirector when picking which objective to use

    /// <summary>
    /// Desired positon to complete current objective
    /// </summary>
    public virtual Vector3 GetDesiredPosition(MechBrain mech)
    {
        // Default: stay where you are
        return mech.transform.position;
    }

    /// <summary>
    /// Desired target for the current objective
    /// </summary>
    public virtual Transform GetTarget(MechBrain mech)
    {
        return null;
    }

    /// <summary>
    /// Simple complete state of an object, (Incase I want multi objective missions)
    /// FUTURE NOTE: CombatDirector can use this to reassign objectives
    /// </summary>
    public virtual bool IsComplete(MechBrain mech)
    {
        return false;
    }

    /// <summary>
    /// Might need more set up function for clean start to a battle
    /// </summary>
    public virtual void SetUpForCombat()
    {

    }
}
