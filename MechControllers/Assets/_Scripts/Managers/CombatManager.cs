using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;

    public GameObject playerPanel;
    public GameObject enemyPanel;

    public List<BaseMech> mechsInCombat;
    public BasePlayerMech playerMech;


    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void SetUpForCombat()
    {
        playerPanel = GameObject.Find("Player Panel");
        enemyPanel = GameObject.Find("TargetMechPanel");

        // Find Mechs in fight
        mechsInCombat = new List<BaseMech>(Object.FindObjectsByType<BaseMech>(FindObjectsSortMode.None));

        if(mechsInCombat.Count == 0)
        {
            Debug.LogError("No mechs found for combat aborting combat Set up");
            return;
        }

        List<EnemyBaseMech> temp = new List<EnemyBaseMech>();

        for(int i = 0; i < mechsInCombat.Count; ++i)
        {
            if (mechsInCombat[i].gameObject.CompareTag("Player"))
            {
                playerMech = mechsInCombat[i] as BasePlayerMech;
            }
            else
            {
                temp.Add(mechsInCombat[i] as EnemyBaseMech);
            }
        }

        EnemyCombatDirector.instance.GetEnemyMech(temp);
        EnemyCombatDirector.instance.GetReadyForCombat();

        for (int i = 0; i < mechsInCombat.Count; ++i)
        {
            mechsInCombat[i].Init();
        }

        for (int i = 1; i < enemyPanel.transform.childCount; ++i)
        {
            if(enemyPanel.transform.GetChild(i).name.Contains("Layout"))
                enemyPanel.transform.GetChild(i).gameObject.SetActive(false);
        }

        SetUpPlayerUI();
    }

    private void SetUpPlayerUI()
    {
        UIManager.instance.CreatePlayerHealthBars(playerMech);
        UIManager.instance.CreateWeaponUI(playerMech);
    }


    public void StartCombat()
    {
        EnemyCombatDirector.instance.StartCombat();

        foreach (BaseMech mech in mechsInCombat)
        {
            mech.EquipWeapons();
        }

        // Test Apply Buff
        //GetComponent<BuffTestScript>().ApplyBuff();
    }
}
