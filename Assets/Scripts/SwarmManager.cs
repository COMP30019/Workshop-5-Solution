// COMP30019 - Graphics and Interaction
// (c) University of Melbourne, 2022

using System.Collections;
using UnityEngine;

public class SwarmManager : MonoBehaviour
{
    // External parameters/variables
    [SerializeField] private GameObject enemyTemplate;
    [SerializeField] private EnemySlot enemySlotTemplate;
    [SerializeField] private int enemyRows;
    [SerializeField] private int enemyCols;
    [SerializeField] private float enemySpacing;
    [SerializeField] private float stepSize;
    [SerializeField] private float stepTime;
    [SerializeField] private float leftBoundaryX;
    [SerializeField] private float rightBoundaryX;

    private int _direction = 1; // Start moving to the right (positive x)

    private void Start()
    {
        // Initial swarm position (parent offset).
        transform.localPosition = new Vector3(this.leftBoundaryX, 0f, 0f);

        // Use a coroutine to define the swarm attack sequence. Here we are
        // nesting coroutines to define a "higher level" sequence, which could
        // easily be built upon to create interesting variations in gameplay.
        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        yield return GenerateSwarm();
        yield return StepSwarmPeriodically();
    }

    private IEnumerator StepSwarmPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(this.stepTime); // Not blocking!
            StepSwarm();
        }
    }

    // Automatically generate swarm of enemies based on the given serialized
    // attributes/parameters. This has been modified to be a coroutine that
    // spawns enemies in quick succession (creates an "entrance sequence").
    private IEnumerator GenerateSwarm()
    {
        // Create swarm of enemies in a grid formation
        for (var col = 0; col < this.enemyCols; col++)
        for (var row = 0; row < this.enemyRows; row++)
        {
            var offset = new Vector3(col, 0.0f, row) * this.enemySpacing;
            var spawnPosition = transform.position +
                                new Vector3(offset.x, 0.0f, 13.0f);
            var enemy = Instantiate(
                this.enemyTemplate, spawnPosition, 
                this.enemyTemplate.transform.rotation); // Use prefab rotation

            var enemySlot = Instantiate(this.enemySlotTemplate);
            enemySlot.SetSwarm(this, offset);
            enemySlot.SetEnemy(enemy.GetComponent<Rigidbody>());

            // A short delay between spawning each enemy allows us to create a
            // sequential "fly in" of enemies to their slot positions. 
            yield return new WaitForSeconds(0.03f);
        }
    }

    // Step the swarm across the screen, based on the current direction, or down
    // and reverse when it reaches the edge.
    private void StepSwarm()
    {
        // Compute the left and right swarm side x positions.
        var swarmWidth = (this.enemyCols - 1) * this.enemySpacing;
        var swarmMinX = transform.localPosition.x;
        var swarmMaxX = swarmMinX + swarmWidth;

        // Check if the swarm has reached a boundary on either side. If so swarm
        // should move down; otherwise, it should move sideways.
        if ((swarmMinX < this.leftBoundaryX && this._direction == -1) ||
            (swarmMaxX > this.rightBoundaryX && this._direction == 1))
        {
            // Move swarm down and flip direction
            transform.Translate(Vector3.back * this.stepSize);
            this._direction = -this._direction;
        }
        else
        {
            // Move swarm sideways
            transform.Translate(Vector3.right *
                                (this._direction * this.stepSize));
        }
    }
}
