// COMP30019 - Graphics and Interaction
// (c) University of Melbourne, 2022

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SpringJoint))]
public class EnemySlot : MonoBehaviour
{
    [SerializeField] private float minSpring;
    [SerializeField] private float maxSpring;

    private SpringJoint _enemyJoint;
    private SwarmManager _swarm;

    private void Awake()
    {
        this._enemyJoint = GetComponent<SpringJoint>();
        this._enemyJoint.spring = Random.Range(this.minSpring, this.maxSpring);
    }

    public void SetSwarm(SwarmManager swarm, Vector3 offset)
    {
        this._swarm = swarm;
        transform.SetParent(this._swarm.transform, false);
        transform.localPosition = offset;
    }

    public void SetEnemy(Rigidbody enemy)
    {
        this._enemyJoint.connectedBody = enemy;
        this._enemyJoint.autoConfigureConnectedAnchor = false;
        this._enemyJoint.connectedAnchor = Vector3.zero;
    }
}
