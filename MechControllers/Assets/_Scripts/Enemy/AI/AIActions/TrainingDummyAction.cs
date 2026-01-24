using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/TrainingDummy")]
public class TrainingDummyAction : AIAction
{
    public override float Score(in AIContext context)
    {
        return 0.0f;
    }

    public override void Execute(AIContext context)
    {

    }
}
