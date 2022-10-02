// COMP30019 - Graphics and Interaction
// (c) University of Melbourne, 2022

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(RigidbodyLookRotation))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private ProjectileController projectilePrefab;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float minIdleTime;
    [SerializeField] private float maxIdleTime;
    [SerializeField] private float aimTime;
    [SerializeField] private float attackStateThrustForce;
    [SerializeField] private float attackLungeImpulse = 35f;

    private PlayerController _player;
    private Rigidbody _rigidbody;
    private RigidbodyLookRotation _rigidbodyLookRotation;
    private Vector3 _aimDirection;
    
    // It often makes sense to treat NPCs as "state machines", whereby a
    // coroutine switches this state over time, but there also are
    // frame-by-frame updates based on this state.
    private enum EnemyState {
        Idle,
        Attack
    }

    private EnemyState _state;

    private void Awake()
    {
        this._player = FindObjectOfType<PlayerController>();
        this._rigidbody = GetComponent<Rigidbody>();
        this._rigidbodyLookRotation = GetComponent<RigidbodyLookRotation>();

        // Similar to the swarm manager, define an attack sequence, but for an 
        // individual enemy (allows for localised behaviour). 
        StartCoroutine(AttackSequence());
    }

    private void FixedUpdate()
    {
        this._rigidbodyLookRotation.SetLookDirection(this._aimDirection);

        if (this._state == EnemyState.Idle)
        {
            // Aim "forward" while in idle state.
            this._aimDirection = Vector3.back;
        }
        else if (this._state == EnemyState.Attack && this._player)
        {
            // Aim at player while in attack state.
            var target = this._player.transform.position;
            this._aimDirection = (target - transform.position).normalized;
            
            // Upwards "thrust" to make state change more evident.
            this._rigidbody.AddForce(
                Vector3.up * this.attackStateThrustForce, ForceMode.Force);
        }
    }

    private IEnumerator AttackSequence()
    {
        // Idle mode to start (give the player a bit of a break!).
        yield return Idle();

        // Switch between attack and idle states indefinitely.
        while (this._player)
        {
            yield return Attack();
            yield return Idle();
        }
    }

    private IEnumerator Attack()
    {
        this._state = EnemyState.Attack;
        
        // Allow for a bit of "aim time" to give player notice. Subtle
        // parameters like this can be varied to change opponent difficulty. 
        yield return new WaitForSeconds(this.aimTime);
        
        // Lunge upwards before firing.
        yield return Lunge(Vector3.up * this.attackLungeImpulse);
        yield return Fire();
    }

    private IEnumerator Lunge(Vector3 lungeVec)
    {
        this._rigidbody.AddForce(lungeVec, ForceMode.Impulse);

        // Lunge begins when velocity positive in lunge direction.
        yield return new WaitUntil(() =>
            Vector3.Dot(this._rigidbody.velocity, lungeVec) > 0f
        );

        // Lunge peaks when velocity switches to negative in lunge direction.
        yield return new WaitUntil(() =>
            Vector3.Dot(this._rigidbody.velocity, lungeVec) < 0f
        );
    }

    private IEnumerator Idle()
    {
        this._state = EnemyState.Idle;
        
        yield return new WaitForSeconds(
            Random.Range(this.minIdleTime, this.maxIdleTime));
    }

    private IEnumerator Fire()
    {
        Instantiate(this.projectilePrefab, transform.position, Quaternion.identity)
            .SetVelocity(this._aimDirection * this.projectileSpeed);
        
        // Recoil impulse
        this._rigidbody.AddForce(
            -this._aimDirection * this.projectileSpeed, ForceMode.Impulse);
        
        yield return new WaitForSeconds(0.5f); // Small delay after fire.
    }
}
