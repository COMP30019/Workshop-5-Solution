// COMP30019 - Graphics and Interaction
// (c) University of Melbourne, 2022

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileController : MonoBehaviour
{
    [SerializeField] private Vector3 velocity;
    [SerializeField] private ParticleSystem collisionParticles;

    [SerializeField] private int damageAmount = 50;
    [SerializeField] private string tagToDamage;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        this._rigidbody = GetComponent<Rigidbody>();
        this._rigidbody.velocity = this.velocity;
    }

    // Note: Now using OnCollisionEnter instead of OnTriggerEnter.
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == this.tagToDamage)
        {
            // Damage object with relevant tag. Now updated to selectively
            // check if health manager exists. We also allow for the possibility
            // that the health manager could be in a parent object.
            var healthManager =
                col.gameObject.GetComponentInParent<HealthManager>();
            if (healthManager)
                healthManager.ApplyDamage(this.damageAmount);
        }

        // Create collision particles in opposite direction to movement.
        var particles = Instantiate(this.collisionParticles);
        particles.transform.position = transform.position;
        particles.transform.rotation =
            Quaternion.LookRotation(col.GetContact(0).normal);

        // Destroy self.
        Destroy(gameObject);
    }

    public void SetVelocity(Vector3 velocity)
    {
        this._rigidbody.velocity = velocity;
    }
}
