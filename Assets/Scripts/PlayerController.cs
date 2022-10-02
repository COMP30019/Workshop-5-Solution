// COMP30019 - Graphics and Interaction
// (c) University of Melbourne, 2022

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Rigidbody body;
    [SerializeField] private RigidbodyLookRotation bodyLookRotation;
    [SerializeField] private float speed;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private ProjectileController projectilePrefab;
    
    private Plane _gamePlane;
    private Vector3 _moveDirection;
    private Vector3 _aimDirection;

    private Rigidbody _rigidbody;

    private void Start()
    {
        this._rigidbody = GetComponent<Rigidbody>();
        this._moveDirection = Vector3.zero;
        this._aimDirection = Vector3.forward;
        this._gamePlane = new Plane(Vector3.up, Vector3.zero);
    }

    private void Update()
    {
        // Update current target based on mouse position.
        Aim();
        
        // We're now using GetAxis() to abstract the means of sideways movement,
        // so both arrow keys and A/D can be used to control the player cube.
        // You can open "Edit -> Project Settings -> Input Manager" within the
        // editor to change the mapping/behaviour of axes and buttons.
        this._moveDirection = Vector3.right * Input.GetAxis("Horizontal");

        // Assuming mouse input to fire, but similar to above, it would probably
        // be worth thinking about alternative input schemes (e.g. for laptop
        // trackpads).
        if (Input.GetMouseButtonDown(0)) Fire();
    }

    private void FixedUpdate()
    {
        // Note that this is moving the kinematic rigidbody that the actual
        // player body is connected to by a spring joint. The blue cube "body"
        // of the player is a child of this object.
        this._rigidbody.position += this._moveDirection * this.speed;
    }
    
    private void Aim()
    {
        // Fire a ray into the world given the current mouse position and get
        // the projected hit point on the x-z gameplay plane (y = 0). This can
        // be used to derive the aim direction.
        var ray = this.playerCamera.ScreenPointToRay(Input.mousePosition);

        if (this._gamePlane.Raycast(ray, out var distance))
        {
            var target = ray.GetPoint(distance);
            this._aimDirection = (target - this.body.transform.position).normalized;
            
            // Update look direction target so player rotates appropriately.
            this.bodyLookRotation.SetLookDirection(this._aimDirection);
        }
    }

    private void Fire()
    {
        Instantiate(this.projectilePrefab, this.body.transform.position, Quaternion.identity)
            .SetVelocity(this._aimDirection * this.projectileSpeed);

        // Apply recoil impulse.
        this.body.AddForce(
            -this._aimDirection * this.projectileSpeed, ForceMode.Impulse);
    }
}
