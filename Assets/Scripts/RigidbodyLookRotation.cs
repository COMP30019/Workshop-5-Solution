// COMP30019 - Graphics and Interaction
// (c) University of Melbourne, 2022

using UnityEngine;

// Helper component to apply a torque such that a rigidbody will orient to look
// in a particular direction (smooth movement/compatible with Unity physics).
[RequireComponent(typeof(Rigidbody))]
public class RigidbodyLookRotation : MonoBehaviour
{
    [SerializeField] private float strength;
    [SerializeField] private Vector3 forward = Vector3.forward;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        this._rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        var torque =
            Vector3.Cross(this._rigidbody.transform.up, Vector3.up) +
            Vector3.Cross(this._rigidbody.transform.forward, this.forward) -
            this._rigidbody.angularVelocity * 0.2f;

        this._rigidbody.AddTorque(torque * this.strength,
            ForceMode.Acceleration);
    }

    public void SetLookDirection(Vector3 forward)
    {
        this.forward = forward;
    }
}
