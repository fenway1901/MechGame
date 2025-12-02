using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyCombatDirector : MonoBehaviour
{
    public static EnemyCombatDirector instance { get; private set; }

    private List<EnemyBaseMech> enemyMechs = new List<EnemyBaseMech>();

    [Header("PROTO Varibles")]
    public Objective protoObjective;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }


    // For prototyping
    public void StartCombat()
    {
        foreach(EnemyBaseMech mech in enemyMechs)
        {
            mech.SetObjective(protoObjective);
        }
    }

    #region Get/Set Functions

    public void GetEnemyMech(List<EnemyBaseMech> mechs)
    {
        enemyMechs = new List<EnemyBaseMech>(mechs);
    }

    #endregion


    IEnumerator DirectorLoop()
    {
        while (true)
        {
            EvaluateBattlefield();
            yield return new WaitForSeconds(2f);
        }
    }

    void EvaluateBattlefield()
    {
        // Example: assign squads to objectives / flanks based on player position, etc.
        foreach (EnemyBaseMech mech in enemyMechs)
        {
            // Simplified example: prioritize nearest objective
            var objective = FindBestObjectiveForSquad(mech);
            mech.CurrentObjective = objective;
        }
    }

    Objective FindBestObjectiveForSquad(EnemyBaseMech mech)
    {
        return null;
        // choose based on distance / importance / etc.
        //return objectives.Count > 0 ? objectives[0] : null;
    }
}
