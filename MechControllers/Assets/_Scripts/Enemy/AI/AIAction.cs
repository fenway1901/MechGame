using UnityEngine;

public struct AIContext
{
    public MechBrain self;
    public Transform target;
    public Objective currentObjective;
    public EnemyCombatDirector director;
    // add: distanceToTarget, hasLineOfSight, coverSpots, etc. if you want caching
}

public abstract class AIAction : ScriptableObject
{
    public string actionName;

    // Return a number; higher = more desirable
    public abstract float Score(in AIContext context);

    // Called when this action gets selected
    public abstract void Execute(AIContext context);
}
