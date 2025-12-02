using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/AI Profile")]
public class AIProfile : ScriptableObject
{
    public List<AIAction> actions;
    public float aggression;      // affects scoring logic if you want
    public float bravery;
    public float reactionDelay;   // can override thinkInterval
}