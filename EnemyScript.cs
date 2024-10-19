using System.Runtime;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    // Enemy stats
    public float health;
    public float maxHealth = 100f; // Ensure this is set to a sensible value in the inspector
    public float attackRange = 5f; // Distance within which the enemy attacks
    public float attackDamage = 10f; // Damage dealt to the player
    public float attackCooldown = 1.5f; // Time between attacks
    public float moveSpeed = 3.5f;

    private Transform player; // Reference to the player's transform
    private NavMeshAgent agent;
    private float nextAttackTime = 0f;
    private PlayerController playerController; // Reference to the player's CharacterClass
    [SerializeField] private GameObject targetIndicator;

    public delegate void EnemyDeathDelegate();
    public event EnemyDeathDelegate OnEnemyDeath;

    [SerializeField]
    private FloatingHealthBar healthBar;

    // Property to check if the enemy is dead
    public bool IsDead => health <= 0;
    public bool isTargeted = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed; // Set movement speed from stats
        healthBar = GetComponentInChildren<FloatingHealthBar>();
    }

    private void Start()
    {
        // Automatically find the player in the scene using the tag "Player"
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>(); // Get the CharacterClass component from the player
        health = maxHealth; // Initialize health to maxHealth

        
    }

    private void Update()
    {
       

        if (player == null) return; // No player, no action

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Move towards player if out of attack range
        if (distanceToPlayer > attackRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            // Stop moving when in range
            agent.isStopped = true;

            // Attack if it's time to attack again
            if (Time.time >= nextAttackTime)
            {
                AttackPlayer();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // Check if player has a CharacterClass and deal damage to it
        if (playerController != null)
        {
            playerController.TakeDamage(attackDamage);
            // Debug.Log("Enemy attacking the player!");
        }
    }

    // Method to take damage from the player
    public void TakeDamage(float damage)
    {
        health -= damage;
        health = Mathf.Max(health, 0); // Ensure health doesn't go below 0
        //Debug.Log($"Enemy took {damage} damage. Remaining health: {health}");

        // Update health bar
        healthBar.UpdateHealthBar(health, maxHealth);

        // Check if dead
        if (IsDead)
        {
            Die();
        }
    }

    // Handle enemy death
    private void Die()
    {
        OnEnemyDeath?.Invoke();
        //Debug.Log("Enemy has died!");
        GiveResources();
        Destroy(gameObject, 0.1f); 
    }

    public void TargetedByPlayer()
    {
        targetIndicator.SetActive(true);
        
    }

    public void UnTargetedByPlayer()
    {
        targetIndicator.SetActive(false);
    }

    void GiveResources()
    {
        playerController.resources["Crystals"] += 5;
        playerController.resources["Vespene"] += 1;
    }
}
