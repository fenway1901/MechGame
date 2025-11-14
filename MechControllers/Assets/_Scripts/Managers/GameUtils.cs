using UnityEngine;

public class GameUtils : MonoBehaviour
{
    public static float GetDistance(Vector3 a, Vector3 b)
    {
        float distance = Vector3.Distance(a, b);
        return distance;
    }
}
