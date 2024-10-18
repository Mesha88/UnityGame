using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public float health = 100f;
    public float attackRange = 5f; // Distance within which the enemy attacks
    public float attackDamage = 10f; // Damage dealt to the player
    public float attackCooldown = 1.5f; // Time between attacks
    public float moveSpeed = 3.5f;

    public delegate void EnemyDeathDelegate();
    public event EnemyDeathDelegate OnEnemyDeath;

    // Property to check if the enemy is dead
    public bool IsDead
    {
        get { return health <= 0; }
    }

    // Method to take damage from the player
    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"Enemy took {damage} damage. Remaining health: {health}");

        if (IsDead)
        {
            Die();
        }
    }

    // Handle enemy death
    private void Die()
    {
        OnEnemyDeath?.Invoke();
        Debug.Log("Enemy has died!");
        Destroy(gameObject); // Destroy the enemy GameObject
    }
}
