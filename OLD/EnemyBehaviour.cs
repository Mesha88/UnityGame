using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private Transform player; // Reference to the player's transform
    private NavMeshAgent agent;
    private float nextAttackTime = 0f;

    private EnemyStats enemyStats; // Reference to the EnemyStats class
    private CharacterClass playerCharacterClass; // Reference to the player's CharacterClass

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyStats = GetComponent<EnemyStats>(); // Get the EnemyStats component
        agent.speed = enemyStats.moveSpeed; // Set movement speed from stats
    }

    private void Start()
    {
        // Automatically find the player in the scene using the tag "Player"
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerCharacterClass = player.GetComponent<CharacterClass>(); // Get the CharacterClass component from the player
    }

    private void Update()
    {
        if (player == null) return; // No player, no action

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Move towards player if out of attack range
        if (distanceToPlayer > enemyStats.attackRange)
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
                nextAttackTime = Time.time + enemyStats.attackCooldown;
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
        if (playerCharacterClass != null)
        {
            playerCharacterClass.TakeDamage(enemyStats.attackDamage);
            //Debug.Log("Enemy attacking the player!");
        }
    }
}