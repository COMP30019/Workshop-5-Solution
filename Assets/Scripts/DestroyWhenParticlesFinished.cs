// COMP30019 - Graphics and Interaction
// (c) University of Melbourne, 2022

using UnityEngine;

public class DestroyWhenParticlesFinished : MonoBehaviour
{
    [SerializeField] private ParticleSystem targetParticleSystem;

    private void Update()
    {
        // Unfortunately there is no event-based API for particle systems so we
        // have to poll whether the system is alive on every update.
        if (!this.targetParticleSystem.IsAlive())
            Destroy(this.targetParticleSystem.gameObject);
    }
}