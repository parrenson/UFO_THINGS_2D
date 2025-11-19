using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemyRandomMovement : MonoBehaviour
{
    public float speed = 2f;
    public Vector2 areaSize = new Vector2(6f, 6f);
    public float minIdleTime = 0.5f;
    public float maxIdleTime = 2f;
    public float maxMoveTimePerTarget = 3f;

    public float detectionRadius = 4f;
    public float loseRadius = 6f;
    public float chaseSpeed = 3.5f;

    public float attackRange = 0.8f;
    public float attackCooldown = 1.5f;
    public int damage = 1;

    public float viewAngle = 90f;

    public Transform flashlightPivot;
    public Light2D flashlightLight;
    public float flashlightDetectRadius = 8f;
    public float flashlightDetectAngle = 120f;

    public float searchDuration = 3f;
    public float searchTurnInterval = 0.5f;

    public AudioClip footstepClip;
    [Range(0f, 1f)] public float footstepVolume = 0.7f;
    public AudioClip attackClip;
    [Range(0f, 1f)] public float attackVolume = 1f;

    private enum EnemyState { Patrol, Chase, Attack, Search }
    private EnemyState currentState = EnemyState.Patrol;

    private Vector2 startPosition;
    private Vector2 targetPosition;
    private float idleTimer = 0f;
    private float movingTimer = 0f;
    private bool isIdle = false;
    private float attackTimer = 0f;
    private float searchTimer = 0f;
    private float searchTurnTimer = 0f;
    private Animator animator;
    private Vector2 lastMoveDir = Vector2.down;
    private Transform player;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        animator = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        if (flashlightPivot != null && flashlightLight == null)
            flashlightLight = flashlightPivot.GetComponentInChildren<Light2D>();

        PickNewTarget();
        UpdateAnimator(Vector2.zero, false);
    }

    private void Update()
    {
        if (player != null)
            UpdateStateByVisionAndDistance();

        switch (currentState)
        {
            case EnemyState.Chase: ChasePlayer(); break;
            case EnemyState.Patrol: Patrol(); break;
            case EnemyState.Attack: AttackBehaviour(); break;
            case EnemyState.Search: SearchBehaviour(); break;
        }

        if (attackTimer > 0f) attackTimer -= Time.deltaTime;
    }

    private void UpdateStateByVisionAndDistance()
    {
        Vector2 enemyPos = rb != null ? rb.position : (Vector2)transform.position;
        Vector2 playerPos = player.position;
        float distance = Vector2.Distance(enemyPos, playerPos);

        bool canSeeByOwnVision = CanSeePlayer(enemyPos, playerPos);
        bool isIlluminatedByFlashlight = IsIlluminatedByFlashlight(enemyPos);
        bool detected = canSeeByOwnVision || isIlluminatedByFlashlight;

        if (detected && distance <= attackRange)
            currentState = EnemyState.Attack;
        else if (detected)
            currentState = EnemyState.Chase;
        else
        {
            if ((currentState == EnemyState.Chase || currentState == EnemyState.Attack) && distance <= loseRadius)
            {
                if (searchTimer <= 0f)
                {
                    searchTimer = searchDuration;
                    searchTurnTimer = 0f;
                }
                currentState = EnemyState.Search;
            }
            else if (currentState == EnemyState.Search)
            {
            }
            else
            {
                currentState = EnemyState.Patrol;
            }
        }
    }

    private bool CanSeePlayer(Vector2 enemyPos, Vector2 playerPos)
    {
        Vector2 toPlayer = playerPos - enemyPos;
        float distanceToPlayer = toPlayer.magnitude;
        if (distanceToPlayer > detectionRadius) return false;
        Vector2 dirToPlayer = toPlayer.normalized;
        Vector2 facingDir = lastMoveDir.sqrMagnitude > 0.001f ? lastMoveDir.normalized : Vector2.down;
        float angle = Vector2.Angle(facingDir, dirToPlayer);
        if (angle > viewAngle * 0.5f) return false;
        return true;
    }

    private bool IsIlluminatedByFlashlight(Vector2 enemyPos)
    {
        if (flashlightPivot == null) return false;
        if (flashlightLight != null && !flashlightLight.enabled) return false;
        Vector2 flashlightPos = flashlightPivot.position;
        Vector2 toEnemy = enemyPos - flashlightPos;
        float distanceToEnemy = toEnemy.magnitude;
        if (distanceToEnemy > flashlightDetectRadius) return false;
        Vector2 dirToEnemy = toEnemy.normalized;
        Vector2 flashlightDir = flashlightPivot.up;
        float angle = Vector2.Angle(flashlightDir, dirToEnemy);
        if (angle > flashlightDetectAngle * 0.5f) return false;
        return true;
    }

    private void Patrol()
    {
        if (isIdle)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0f)
            {
                isIdle = false;
                movingTimer = 0f;
                PickNewTarget();
            }
            UpdateAnimator(lastMoveDir, false);
        }
        else
        {
            MoveTowardsTarget();
        }
    }

    private void MoveTowardsTarget()
    {
        Vector2 current = rb.position;
        float distance = Vector2.Distance(current, targetPosition);

        if (distance < 0.1f)
        {
            EnterIdleState();
            return;
        }

        Vector2 direction = (targetPosition - current).normalized;
        lastMoveDir = direction;
        Vector2 newPos = current + direction * speed * Time.deltaTime;
        rb.MovePosition(newPos);

        UpdateAnimator(direction, true);

        movingTimer += Time.deltaTime;
        if (movingTimer >= maxMoveTimePerTarget)
        {
            EnterIdleState();
        }
    }

    private void EnterIdleState()
    {
        isIdle = true;
        idleTimer = Random.Range(minIdleTime, maxIdleTime);
        movingTimer = 0f;
        UpdateAnimator(lastMoveDir, false);
    }

    private void PickNewTarget()
    {
        float halfWidth = areaSize.x / 2f;
        float halfHeight = areaSize.y / 2f;
        float randomX = Random.Range(startPosition.x - halfWidth, startPosition.x + halfWidth);
        float randomY = Random.Range(startPosition.y - halfHeight, startPosition.y + halfHeight);
        targetPosition = new Vector2(randomX, randomY);
    }

    private void ChasePlayer()
    {
        if (player == null)
        {
            currentState = EnemyState.Patrol;
            return;
        }
        Vector2 current = rb.position;
        Vector2 target = player.position;
        float distance = Vector2.Distance(current, target);
        if (distance <= attackRange) return;
        Vector2 direction = (target - current).normalized;
        lastMoveDir = direction;
        Vector2 newPos = current + direction * chaseSpeed * Time.deltaTime;
        rb.MovePosition(newPos);
        UpdateAnimator(direction, true);
        isIdle = false;
    }

    private void AttackBehaviour()
    {
        if (player == null)
        {
            currentState = EnemyState.Patrol;
            return;
        }
        Vector2 enemyPos = rb.position;
        Vector2 playerPos = player.position;
        float distance = Vector2.Distance(enemyPos, playerPos);
        if (distance > attackRange * 1.2f)
        {
            currentState = EnemyState.Chase;
            return;
        }
        Vector2 dir = (playerPos - enemyPos).normalized;
        if (dir.sqrMagnitude > 0.001f)
            lastMoveDir = dir;
        UpdateAnimator(Vector2.zero, false);
        if (attackTimer <= 0f)
        {
            if (animator != null)
                animator.SetTrigger("attackTrigger");
            HealthSystem_Player hp = player.GetComponent<HealthSystem_Player>();
            if (hp != null)
                hp.TakeDamage(damage);
            attackTimer = attackCooldown;
        }
    }

    private void SearchBehaviour()
    {
        if (player == null)
        {
            currentState = EnemyState.Patrol;
            return;
        }
        searchTimer -= Time.deltaTime;
        if (searchTimer <= 0f)
        {
            currentState = EnemyState.Patrol;
            PickNewTarget();
            return;
        }
        searchTurnTimer -= Time.deltaTime;
        if (searchTurnTimer <= 0f)
        {
            Vector2[] dirs = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
            int index = Random.Range(0, dirs.Length);
            lastMoveDir = dirs[index];
            UpdateAnimator(Vector2.zero, false);
            searchTurnTimer = searchTurnInterval;
        }
    }

    private void UpdateAnimator(Vector2 dir, bool moving)
    {
        if (animator == null) return;
        animator.SetBool("isMoving", moving);
        if (dir.sqrMagnitude < 0.001f)
            dir = lastMoveDir;
        animator.SetFloat("moveX", dir.x);
        animator.SetFloat("moveY", dir.y);
    }

    public void PlayFootstep()
    {
        if (footstepClip != null)
            AudioSource.PlayClipAtPoint(footstepClip, Camera.main.transform.position, footstepVolume);
    }

    public void PlayAttackSound()
    {
        if (attackClip != null)
            AudioSource.PlayClipAtPoint(attackClip, Camera.main.transform.position, attackVolume);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Enemy chocó con: " + collision.collider.name);
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 center = Application.isPlaying && rb != null ? rb.position : (Vector2)transform.position;
        Gizmos.color = Color.red;
        Vector2 areaCenter = Application.isPlaying ? startPosition : center;
        Gizmos.DrawWireCube(areaCenter, areaSize);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center, detectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, loseRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(center, attackRange);

        if (Application.isPlaying)
        {
            Vector2 facingDir = lastMoveDir.sqrMagnitude > 0.001f ? lastMoveDir.normalized : Vector2.down;
            float halfAngle = viewAngle * 0.5f;
            Quaternion qLeft = Quaternion.Euler(0, 0, +halfAngle);
            Quaternion qRight = Quaternion.Euler(0, 0, -halfAngle);
            Vector2 leftDir = qLeft * facingDir;
            Vector2 rightDir = qRight * facingDir;
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(center, center + leftDir * detectionRadius);
            Gizmos.DrawLine(center, center + rightDir * detectionRadius);
        }
    }
}
