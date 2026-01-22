using UnityEngine;

public class HuntPlayerObjective : Objective
{
    [Header("Hunt Player Settings")]
    public Transform player;          // Assign in inspector or at runtime

    public override void SetUpForCombat()
    {
        player = CombatManager.instance.playerMech.gameObject.transform;
    }

    public override Vector3 GetDesiredPosition(MechBrain mech)
    {
        // We want to be where the player is (your AIAction will usually stop at a certain range)
        if (player == null)
            return mech.transform.position;

        return player.position;
    }

    public override Transform GetTarget(MechBrain mech)
    {
        // All attacks should focus this target
        return player;
    }

    public override bool IsComplete(MechBrain mech)
    {
        // Example: objective is complete if player is dead or missing
        return player == null;
    }
}
