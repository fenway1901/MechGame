using UnityEngine;

public class BaseCrew : MonoBehaviour
{
    public float stress;
    public float speed;

    [SerializeField] private BaseLimb targetLimb;

    [Header("Stats")]
    public float mechanics;
    public float piolet;
    public float gunner;
    public float electrician;


    #region Movement

    public void MoveToLimb(BaseLimb target)
    {
        transform.position = target.transform.position;
    }

    #endregion

    #region Get stat function Info

    // Incase i want to add more stuff to stats

    #endregion
}
