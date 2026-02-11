using UnityEngine;
using System.Collections.Generic;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager instance;

    [SerializeField] private List<ParticleSystem> sparksInScene;
    [SerializeField] private List<ScreenGlitchDriver> glitchEffects;

    void Awake()
    {
        instance = this;

        if (glitchEffects.Count > 0)
        {
            for (int i = 0; i < glitchEffects.Count; ++i)
            {
                glitchEffects[i].gameObject.SetActive(false);
            }
        }
    }

    public void PlayEffects()
    {
        // Spark effects
        if (sparksInScene.Count >= 0)
        {
            var (i1, i2) = GameUtils.GetTwoRandomDistinct(sparksInScene.Count);

            sparksInScene[i1].Play();
            sparksInScene[i2].Play();
        }

        // Glitch Effects
        if (glitchEffects.Count >= 0)
        {
            for (int i = 0; i < glitchEffects.Count; ++i)
            {
                glitchEffects[i].gameObject.SetActive(true);
                glitchEffects[i].TriggerHit();
            }
        }
    }
}
