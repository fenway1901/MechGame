using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public static AttackManager instance;


    void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region AttackSequence

    public void AttackEnemy(GameObject target, BaseWeapons weapon)
    {
        StartCoroutine(Attack(weapon.GetAttackSpeed(), target, weapon));
    }

    private IEnumerator Attack(float time, GameObject target, BaseWeapons weapon)
    {
        yield return new WaitForSeconds(time);

        target.GetComponent<BaseHealthComponent>().TakeDamage(weapon.GetDamage());
    }

    #endregion
}
