using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/AI Profile")]
public class AIProfile : ScriptableObject
{
    public List<AIAction> actions;
    public float aggression; 
    public float bravery;
    public float reactionDelay;   // allows override of thinkInterval
}