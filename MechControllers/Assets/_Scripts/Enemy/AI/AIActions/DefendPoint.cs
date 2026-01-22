using UnityEngine;

public class DefendPoint : Objective
{
    public Transform defendPoint;
    public float defendRadius = 10f;

    public override Vector3 GetDesiredPosition(MechBrain mech)
    {
        return defendPoint != null ? defendPoint.position : mech.transform.position;
    }

    public override bool IsComplete(MechBrain mech)
    {
        if (defendPoint == null) return true;

        float dist = Vector3.Distance(mech.transform.position, defendPoint.position);
        return dist <= defendRadius;
    }
}
