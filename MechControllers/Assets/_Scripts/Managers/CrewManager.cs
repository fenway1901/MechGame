using UnityEngine;
using System.Collections.Generic;

public class CrewManager : MonoBehaviour
{
    List<BaseCrew> crewList;


    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public void MoveCrewMember(BaseCrew crew, BaseLimb limb)
    {
        crew.MoveToLimb(limb);

        // Give buffs here i guess
    }
}
