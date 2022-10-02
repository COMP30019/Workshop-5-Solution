// COMP30019 - Graphics and Interaction
// (c) University of Melbourne, 2022

using UnityEngine;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int startingHealth = 100;

    // Using an event to maximise component re-use. Any other component can 
    // listen to this event to do arbitrary actions when this game object dies.
    [SerializeField] private UnityEvent onDeath;

    // Likewise, another event is used for health changes. The generic interface
    // to this is a fraction between 0-1 denoting the % health remaining.
    [SerializeField] private UnityEvent<float> onHealthChanged;

    private int _currentHealth;

    private int CurrentHealth
    {
        get => this._currentHealth;
        set
        {
            // Using a C# property to ensure the onHealthChanged event is
            // consistently fired when the health changes, and also to check if
            // the object has died (<= 0 health). It's not really different to
            // the concept of a "setter" as per OOP good practice, however, we
            // can still treat it like an integer variable (add, subtract, etc).
            this._currentHealth = value;
            var frac = this._currentHealth / (float)this.startingHealth;
            this.onHealthChanged.Invoke(frac);
            if (CurrentHealth <= 0) // Did we die?
            {
                // Let onDeath event listeners know that we died. 
                this.onDeath.Invoke();

                // Destroy ourselves.
                Destroy(gameObject);
            }
        }
    }

    private void Start()
    {
        ResetHealthToStarting();
    }

    public void ResetHealthToStarting()
    {
        CurrentHealth = this.startingHealth;
    }

    public void ApplyDamage(int damage)
    {
        CurrentHealth -= damage;
    }
}