using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyCombatDirector : MonoBehaviour
{
    public static EnemyCombatDirector instance { get; private set; }

    [SerializeField] private List<EnemyBaseMech> enemyMechs = new List<EnemyBaseMech>();
    [SerializeField] private List<Objective> objectives = new List<Objective>(); // for prototyping will only use Objective 1

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void GetReadyForCombat()
    {
        for(int i = 0; i < objectives.Count; ++i)
        {
            objectives[i].SetUpForCombat();
        }
    }


    // For prototyping
    public void StartCombat()
    {
        foreach(EnemyBaseMech mech in enemyMechs)
        {
            mech.SetObjective(objectives[0]);
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
            Objective objective = FindBestObjectiveForSquad(mech);
            if(objective != null)
                mech.SetObjective(objective);
        }
    }

    Objective FindBestObjectiveForSquad(EnemyBaseMech mech)
    {
        return null;
        // choose based on distance / importance / etc.
        //return objectives.Count > 0 ? objectives[0] : null;
    }
}
