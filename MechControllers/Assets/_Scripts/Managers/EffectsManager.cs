using UnityEngine;
using System.Collections.Generic;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager instance;

    [SerializeField] private List<ParticleSystem> sparksInScene;


    void Awake()
    {
        instance = this;
    }

    public void PlaySparks()
    {
        if (sparksInScene.Count <= 0) return;

        for (int i = 0; i < sparksInScene.Count; ++i)
        {
            sparksInScene[i].Play();
        }
    }
}
