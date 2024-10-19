using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    /// VARIABLE DECLARATIONS
    /// VARIABLE DECLARATIONS
    /// VARIABLE DECLARATIONS

    //Player stats
    public float health;
    public float maxHealth;
    public float mana;
    public float maxMana;
    public float movementSpeed;
    public float attackRange;
    public float visionRange;
    public float attackCooldown;
    public float damage;
    public float healingRate;
    public float armor;
    public int crystals = 0;
    public int vespene = 0;
    float currentExperience;
    float neededExperience;
    int level = 1;

    public float rotationSpeed = 5;
    [SerializeField] private GameObject healthBar;
    PlayerHealthBarScript healthBarScript;



    //Enemy related
    public LayerMask enemyLayer; //All units on this layer are enemies
    


    //Ability related
    public Ability[] abilities = new Ability[4]; //Ability class files can be dragged in inside unity

    //Player Realted - needed for functionality
    private UnityEngine.AI.NavMeshAgent agent;
    private Animator anim;
    private Transform currentTarget; // Current enemy being attacked
    private float attackTimer = 0;
    private Vector3 originalPosition; // The position the player is supposed to return to after chasing target he auto attacked while idle
    private Vector3 attackMoveDestination;

    [SerializeField] 

    //State Machine
    private enum State { Idle, Moving, Attacking, AttackMove, AttackMoveTargeting }
    private State currentState = State.Idle;

    //------------------------------------------------------------//

    /// GAMEPLAY
    /// GAMEPLAY
    /// GAMEPLAY


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        agent.speed = movementSpeed;
        agent.acceleration = 100f;
        originalPosition = transform.position;
        //agent.updateRotation = false;
        healthBarScript = healthBar.GetComponent<PlayerHealthBarScript>();

        

    }

    // Update is called once per frame
    void Update()
    {

       

        attackTimer -= Time.deltaTime;
        if (Input.GetMouseButtonDown(1))
        {
            //Right click to check what is clicked
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
                if (hit.transform.CompareTag("Enemy"))
                {
                    SetTarget(hit.transform);
                }
                else
                {
                    MoveToLocation(hit.point);

                }
        }

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
            //case State.AttackMove:
              //  HandleAttackMove();
                //break;
        }

        HandleAbilities(); // Always check for ability inputs
        UpdateAnimator();  // Update animation based on movement


        //Attack Move enable button
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    currentState = State.AttackMoveTargeting;
        //}
    }

    //------------------------------------------------------------//

    /// FUNCTION DECLARATIONS
    /// FUNCTION DECLARATIONS
    /// FUNCTION DECLARATIONS

    void HandleIdle()
    {
        
            FindNearestEnemy();
           
        
    }


    void HandleMoving()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            SetState(State.Idle);
           
        }
    }

    void HandleAttacking()
    {
        if (currentTarget == null)
        {
            Debug.Log("Current target is null, switching to Idle state");
            SetState(State.Idle);
            return;
        }

        EnemyScript enemyScript = currentTarget.GetComponent<EnemyScript>();
        if (enemyScript != null)
        {
            enemyScript.TargetedByPlayer();
        }

        if (IsTargetOutOfRange())
        {
            Debug.Log("Target is out of range, switching to Idle state");
            SetState(State.Idle);
            return;
        }

        RotateTowards(currentTarget.position);

        if (attackTimer <= 0f)
        {
            Debug.Log("Triggering attack animation");
            if (anim != null)
            {
                anim.SetTrigger("Shoot");
            }
            else
            {
                Debug.LogError("Animator is null");
            }
            StartCoroutine(AttackCoroutine());

        }

        agent.isStopped = true;
        
       
        
    }


    //void HandleAttackMoveTargetting()
    //{


    //    Ray attackMoveRay = Camera.Main.ScreenPointToRay(Input.mousePosition);
    //    if(Physics.Raycast(attackMoveRay, out RaycastHit hit))
    //    {
    //        if (hit.transform.CompareTag("Enemy"))
    //        {
    //            currentTarget = hit.transform;
    //            currentState = State.Attacking;
    //        }
    //        else
    //        {
    //            attackMoveDestination = hit.point.
    //            currentState = State.AttackMove;
    //        }
    //    }
    //}

    //void HandleAttackMove()
    //{

    //}


    void MoveToLocation(Vector3 destination)
    {
        StopAttack();
        agent.isStopped = false;
        agent.SetDestination(destination);
        RotateTowards(destination);
        SetState(State.Moving);
        
    }

    IEnumerator AttackCoroutine()
    {
        attackTimer = attackCooldown;
        EnemyScript enemyScript = currentTarget.GetComponent<EnemyScript>();
        if (enemyScript != null)
        {
            enemyScript.TakeDamage(damage);
            if (enemyScript.IsDead)
            {
                SetState(State.Idle);
            }

            yield return new WaitForSeconds(attackCooldown);
        }


    }

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
                SetTarget(nearestEnemy);
            }
        }
    }


    bool IsTargetOutOfRange()
    {
        if (currentTarget == null) return true;
        return Vector3.Distance(transform.position, currentTarget.position) > attackRange;
    }


    void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            float angle = Vector3.Angle(transform.forward, direction);
            float dynamicRotationSpeed = Mathf.Lerp(rotationSpeed, rotationSpeed * 10, angle / 100);

            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * dynamicRotationSpeed);
        }
    }

    void StopAttack()
    {
        if (currentTarget != null)
        {
            EnemyScript enemyScript = currentTarget.GetComponent<EnemyScript>();
            if (enemyScript != null)
            {
                enemyScript.UnTargetedByPlayer();
            }
        }
        currentTarget = null;
       
        agent.isStopped = false;
        Debug.Log("Attack stopped");
    }


    void HandleAbilities()
    {
        // Check for ability input (Q, W, E, R) and trigger the corresponding ability
        if (Input.GetKeyDown(KeyCode.Q)) UseAbility(0);
        if (Input.GetKeyDown(KeyCode.W)) UseAbility(1);
        if (Input.GetKeyDown(KeyCode.E)) UseAbility(2);
        if (Input.GetKeyDown(KeyCode.R)) UseAbility(3);
    }

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
        float speed = agent.velocity.magnitude;
        anim.SetFloat("Speed", speed);

        if(speed > 0.1f)
        {
            anim.SetBool("IsWalking", true);
        }

        else
        {
            anim.SetBool("IsWalking", false);
        }
        
        
    }
    
    public void TakeDamage(float damage)
    {
        health -= damage;
        health = Mathf.Max(health, 0);
        healthBarScript.UpdateHealthBar(health, maxHealth);
    }

    void SetTarget(Transform target)
    {
        if (currentTarget != target)
        {
            StopAttack();
            currentTarget = target;
            currentState = State.Attacking;
        }
    }

    void SetState(State newState)
    {
        if (currentState != newState)
        {
            Debug.Log($"Changing state from {currentState} to {newState}");
            currentState = newState;

            switch (newState)
            {
                case State.Idle:
                    StopAttack();
                    break;

                case State.Moving:

                    break;

                case State.Attacking:

                    break;
            }

        }
    }
}
