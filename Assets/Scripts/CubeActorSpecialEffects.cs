// COMP30019 - Graphics and Interaction
// (c) University of Melbourne, 2022

using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class CubeActorSpecialEffects : MonoBehaviour
{
    [SerializeField] private ParticleSystemRenderer deathEffect;
    private Color _baseColor;

    private MeshRenderer _renderer;

    private void Awake()
    {
        this._renderer = gameObject.GetComponent<MeshRenderer>();
        this._baseColor = this._renderer.material.color;
    }

    public void UpdateDamageLevel(float healthFrac)
    {
        this._renderer.material.color = this._baseColor * healthFrac;
    }

    public void Explode()
    {
        var particles = Instantiate(this.deathEffect);
        particles.transform.position = transform.position;
        particles.material.color = this._baseColor;
    }
}
