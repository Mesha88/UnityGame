using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MOBACharacterController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float attackRange = 5f;
    public float visionRange = 7f;
    public float attackCooldown = 1f;
    public int damage = 20;
    public LayerMask enemyLayer;

    public Ability[] abilities = new Ability[4]; // Array for abilities (Q, W, E, R)

    private NavMeshAgent agent;
    private Animator anim; // Reference to the Animator component
    private Transform currentTarget;
    private float attackTimer = 0f;
    private Vector3 originalPosition;

    // Define the states
    private enum State { Idle, Moving, Attacking, AttackMove }
    private State currentState = State.Idle;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();  // Get reference to Animator
        agent.speed = moveSpeed;
        originalPosition = transform.position;
        agent.updateRotation = false; // Disable automatic rotation by the NavMeshAgent
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;

        // Prioritize movement over attacking
        if (Input.GetMouseButtonDown(1)) // Right-click for move
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                MoveToLocation(hit.point);
                return;  // Exit to avoid overriding with attack logic
            }
        }

        // Handle state logic if not moving
        switch (currentState)
        {
            case State.Idle:
                HandleIdle();
                break;
            case State.Moving:
                HandleMoving();
                break;
            case State.Attacking:
                HandleAttacking();
                break;
            case State.AttackMove:
                HandleAttackMove();
                break;
        }

        HandleAbilities(); // Always check for ability inputs
        UpdateAnimator();  // Update animation based on movement
    }
    void HandleIdle()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // Right-click to move to a location
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                MoveToLocation(hit.point);

            }
        }
        else if (Input.GetKeyDown(KeyCode.A) && Input.GetMouseButtonDown(0))
        {
            // Attack-move (A + left-click)
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                AttackMoveToLocation(hit.point);
                
            }
        }
        else
        {
            // Look for enemies to auto-attack
            FindNearestEnemy();
        }
    }

    // Handle Moving State (moving to a location)
    void HandleMoving()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Arrived at destination
            currentState = State.Idle; // Go back to idle once destination is reached
        }
    }

    // Handle Attack State (attacking an enemy)
    void HandleAttacking()
    {
        if (currentTarget == null || IsTargetOutOfRange())
        {
            currentState = State.Idle;  // If target is gone or out of range, go back to idle
            agent.isStopped = false;    // Ensure movement is allowed again
            return;
        }

        RotateTowards(currentTarget.position); // Rotate instantly toward the target

        if (attackTimer <= 0f)
        {
            anim.SetTrigger("Shoot");  // Trigger attack animation
            StartCoroutine(AttackCoroutine());  // Attack the target
        }

        // Allow movement during attack (if needed)
        agent.isStopped = false;
    }
    // Handle Attack Move State (moving and attacking)
    void HandleAttackMove()
    {
        if (currentTarget != null)
        {
            currentState = State.Attacking;  // Switch to attack mode if we find a target
        }
        else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            currentState = State.Idle;  // Reached destination, go back to idle
        }
        else
        {
            FindNearestEnemy(); // Continuously check for enemies while moving
        }
    }

    // Move to a location and cancel any ongoing attack
    void MoveToLocation(Vector3 destination)
    {
        if (currentState != State.Moving)
        {
            StopAttack();  // Stop any current attacks
        }
        agent.isStopped = false;  // Ensure agent is allowed to move
        agent.SetDestination(destination);  // Set new movement destination
        RotateTowards(destination);  // Rotate towards destination
        currentState = State.Moving;  // Switch to Moving state
    }

    // Attack-move to a location, attacking enemies along the way
    void AttackMoveToLocation(Vector3 destination)
    {
        StopAttack();  // Stop any current attacks
        agent.isStopped = false;  // Ensure agent is allowed to move
        agent.SetDestination(destination);  // Set attack-move destination
        RotateTowards(destination);  // Rotate towards destination
        currentState = State.AttackMove;  // Switch to attack-move state
    }

    // Automatically attacks the current target when in range
    IEnumerator AttackCoroutine()
    {
        attackTimer = attackCooldown;

        // Deal damage to the target (assuming the enemy has an EnemyStats component)
        EnemyStats enemyStats = currentTarget.GetComponent<EnemyStats>();
        if (enemyStats != null)
        {
            enemyStats.TakeDamage(damage); // Deal damage to the enemy
            if (enemyStats.IsDead)
            {
                currentTarget = null;  // Target is dead, reset the target
                currentState = State.Idle;  // Return to idle after killing the target
            }
        }

        // Wait for the attack cooldown to finish
        yield return new WaitForSeconds(attackCooldown);
    }

    // Finds the nearest enemy within vision range and sets it as the target
    void FindNearestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, visionRange, enemyLayer);

        if (enemies.Length > 0)
        {
            float nearestDistance = Mathf.Infinity;
            Transform nearestEnemy = null;

            foreach (Collider enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemy.transform;
                }
            }

            if (nearestEnemy != null)
            {
                currentTarget = nearestEnemy;  // Assign nearest enemy as the target
                currentState = State.Attacking;  // Switch to attacking state
            }
        }
    }

    // Check if target is out of attack range
    bool IsTargetOutOfRange()
    {
        if (currentTarget == null) return true;
        return Vector3.Distance(transform.position, currentTarget.position) > attackRange;
    }

    // Instantly rotates the player towards the given point
    void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        }
    }

    // Cancels any ongoing attack
    void StopAttack()
    {
        currentTarget = null;  // Clear the current target
        StopAllCoroutines();   // Stop attack coroutine if running
        agent.isStopped = false; // Ensure movement is not stopped
        currentState = State.Idle;  // Reset state to idle
    }

    // Check for ability input and trigger abilities (empty placeholders to be filled in Unity)
    void HandleAbilities()
    {
        // Check for ability input (Q, W, E, R) and trigger the corresponding ability
        if (Input.GetKeyDown(KeyCode.Q)) UseAbility(0);
        if (Input.GetKeyDown(KeyCode.W)) UseAbility(1);
        if (Input.GetKeyDown(KeyCode.E)) UseAbility(2);
        if (Input.GetKeyDown(KeyCode.R)) UseAbility(3);
    }

    // Method to trigger abilities (empty placeholders, filled in Unity)
    void UseAbility(int abilityIndex)
    {
        if (abilities[abilityIndex] != null)
        {
            abilities[abilityIndex].Activate();
        }
    }

    // Updates animation based on the character's movement speed
    private void UpdateAnimator()
    {
        anim.SetFloat("Speed", agent.velocity.magnitude);  // Set "Speed" parameter in the animator
    }
}
