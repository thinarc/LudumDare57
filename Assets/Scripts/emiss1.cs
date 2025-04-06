using UnityEngine;

public class emiss1 : MonoBehaviour
{
    [SerializeField] private ParticleSystem ash_particle;

    private void Update()
    {
        float emiss = ash_particle.emission.rateOverTime.constant + Time.deltaTime * 0.5f;
        if (emiss > 700f) return;

        var newEmission = ash_particle.emission;
        newEmission.rateOverTime = emiss;
    }
}