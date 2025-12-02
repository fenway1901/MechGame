using UnityEngine;

public class Objective : MonoBehaviour
{
    [Header("General")]
    public string objectiveName = "Objective";
    [Range(0f, 1f)]
    public float priority = 1f;   // Used by CombatDirector when picking which objective to use

    /// <summary>
    /// Where this mech would ideally like to be to fulfill the objective
    /// (e.g. defense point, near the player, retreat zone).
    /// </summary>
    public virtual Vector3 GetDesiredPosition(MechBrain mech)
    {
        // Default: stay where you are
        return mech.transform.position;
    }

    /// <summary>
    /// Main target this objective cares about (can be null for purely positional objectives).
    /// </summary>
    public virtual Transform GetTarget(MechBrain mech)
    {
        return null;
    }

    /// <summary>
    /// Optional: whether this objective is considered complete for this mech.
    /// CombatDirector can use this to reassign objectives.
    /// </summary>
    public virtual bool IsComplete(MechBrain mech)
    {
        return false;
    }
}
