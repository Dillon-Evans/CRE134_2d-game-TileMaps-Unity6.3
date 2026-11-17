// A simple enemy "brain" built as a state machine. The enemy is always in exactly one state, and each
// state has its own rules for what to do and when to switch to another state:
//   Idle       wait a moment, then Wander (or Chase if the player comes close)
//   Wander     walk to a random spot near home, then Idle (or Chase if the player comes close)
//   Chase      run at the player; back to Idle if the player gets away
// T06b adds two more states: Attack (jump on the player) and ReturnHome (walk back before wandering).
// It moves in straight lines, so trees and water get in its way: T07 fixes that with pathfinding.
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // Every state the enemy can be in. An enum is a list of named options.
    private enum State
    {
        Idle,
        Wander,
        Chase
    }

    [Header("Idle and wander")]
    [SerializeField] private float wanderSpeed = 1f;
    // How far from home it wanders.
    [SerializeField] private float wanderRadius = 3f;
    [SerializeField] private float minIdleTime = 1f;
    [SerializeField] private float maxIdleTime = 3f;
    // Gives up on a spot it can't reach (a tree in the way) after this long.
    [SerializeField] private float maxWanderTime = 4f;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 2.5f;
    // Starts chasing when the player is this close.
    [SerializeField] private float detectionRange = 4f;
    // Gives up when the player is this far away. It's bigger than detectionRange, so the enemy doesn't
    // flick between chasing and giving up when the player stands right on the edge.
    [SerializeField] private float giveUpRange = 7f;

    // Shown in the Inspector so you can watch it change during Play.
    [SerializeField] private State currentState = State.Idle;

    private Rigidbody2D body;
    private Animator animator;
    private Transform player;
    private Vector2 homePosition;
    private Vector2 wanderTarget;

    // Set by the states each frame; FixedUpdate turns them into a velocity.
    private Vector2 moveDirection;
    private float moveSpeed;

    // Seconds spent in the current state. ChangeState sets it back to 0.
    private float stateTimer;
    private float idleTime;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        // The Animator is on the Sprite child (see T03).
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        homePosition = transform.position;

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        ChangeState(State.Idle);
    }

    private void Update()
    {
        stateTimer = stateTimer + Time.deltaTime;

        // Run the rules for the current state only.
        switch (currentState)
        {
            case State.Idle:
                UpdateIdle();
                break;
            case State.Wander:
                UpdateWander();
                break;
            case State.Chase:
                UpdateChase();
                break;
        }
    }

    private void FixedUpdate()
    {
        body.linearVelocity = moveDirection * moveSpeed;
    }

    // Switches state. Everything that should happen once, as a state starts, goes here.
    private void ChangeState(State newState)
    {
        Debug.Log(gameObject.name + ": " + currentState + " -> " + newState);
        currentState = newState;
        stateTimer = 0f;
        moveDirection = Vector2.zero;

        switch (newState)
        {
            case State.Idle:
                // A random wait, so several enemies don't all move at the same moment.
                idleTime = Random.Range(minIdleTime, maxIdleTime);
                PlayAnimation("Slime_Idle");
                break;
            case State.Wander:
                // A random spot inside a circle around home, so it never wanders off for good.
                wanderTarget = homePosition + Random.insideUnitCircle * wanderRadius;
                moveSpeed = wanderSpeed;
                PlayAnimation("Slime_Move");
                break;
            case State.Chase:
                moveSpeed = chaseSpeed;
                PlayAnimation("Slime_Move");
                break;
        }
    }

    private void UpdateIdle()
    {
        if (PlayerIsClose())
        {
            ChangeState(State.Chase);
        }
        else if (stateTimer >= idleTime)
        {
            ChangeState(State.Wander);
        }
    }

    private void UpdateWander()
    {
        if (PlayerIsClose())
        {
            ChangeState(State.Chase);
        }
        else if (HasReached(wanderTarget) || stateTimer >= maxWanderTime)
        {
            ChangeState(State.Idle);
        }
        else
        {
            MoveTowards(wanderTarget);
        }
    }

    private void UpdateChase()
    {
        // The player may have been destroyed (died).
        if (player == null)
        {
            ChangeState(State.Idle);
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > giveUpRange)
        {
            ChangeState(State.Idle);
        }
        else
        {
            MoveTowards(player.position);
        }
    }

    // ---------- Helpers the states share ----------

    private bool PlayerIsClose()
    {
        if (player == null)
        {
            return false;
        }
        return Vector2.Distance(transform.position, player.position) <= detectionRange;
    }

    private void MoveTowards(Vector2 target)
    {
        Vector2 toTarget = target - (Vector2)transform.position;
        moveDirection = toTarget.normalized;
    }

    private bool HasReached(Vector2 target)
    {
        return Vector2.Distance(transform.position, target) < 0.1f;
    }

    private void PlayAnimation(string stateName)
    {
        if (animator != null)
        {
            animator.Play(stateName);
        }
    }

    // Draws the ranges in the Scene view when the enemy is selected.
    private void OnDrawGizmosSelected()
    {
        Vector3 home = transform.position;
        if (Application.isPlaying)
        {
            home = homePosition;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(home, wanderRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(transform.position, giveUpRange);
    }
}
