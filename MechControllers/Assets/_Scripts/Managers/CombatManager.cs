using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;

    public List<BaseMech> mechsInCombat;
    public BaseMech playerMech;


    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void SetUpForCombat()
    {
        // Find Mechs in fight
        mechsInCombat = new List<BaseMech>(Object.FindObjectsByType<BaseMech>(FindObjectsSortMode.None));

        if(mechsInCombat.Count == 0)
        {
            Debug.LogError("No mechs found for combat aborting combat Set up");
            return;
        }

        for(int i = 0; i < mechsInCombat.Count; ++i)
        {
            if (mechsInCombat[i].gameObject.CompareTag("Player"))
            {
                playerMech = mechsInCombat[i];
                break;
            }
        }
    }

    public void StartCombat()
    {
        for(int i = 0; i < mechsInCombat.Count; ++i)
        {
            mechsInCombat[i].Init();
        }
    }
}
