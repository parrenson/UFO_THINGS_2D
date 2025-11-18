using UnityEngine;
using UnityEngine.Rendering.Universal; // Para Light2D

public class EnemyRandomMovement : MonoBehaviour
{
    [Header("Patrulla aleatoria")]
    public float speed = 2f;
    public Vector2 areaSize = new Vector2(6f, 6f);
    public float minIdleTime = 0.5f;
    public float maxIdleTime = 2f;
    public float maxMoveTimePerTarget = 3f;

    [Header("Persecución / Detección")]
    public float detectionRadius = 4f;   // Radio máximo donde empieza a perseguir
    public float loseRadius = 6f;        // Distancia a la que deja de perseguir (de apoyo)
    public float chaseSpeed = 3.5f;      // Velocidad persiguiendo

    [Header("Ataque")]
    public float attackRange = 0.8f;     // Distancia a la que se detiene para atacar
    public float attackCooldown = 1.5f;  // Tiempo entre ataques (segundos)
    public int damage = 1;               // Daño por golpe

    [Header("Cono de visión del enemigo")]
    public float viewAngle = 90f;        // Ángulo total del cono (ej: 90 = 45° a cada lado)

    [Header("Detección por linterna del jugador")]
    public Transform flashlightPivot;          // GameObject que rota con la linterna
    public Light2D flashlightLight;           // Componente Light2D de la linterna
    public float flashlightDetectRadius = 8f; // Radio de detección al ser iluminado
    public float flashlightDetectAngle = 120f;// Ángulo total del cono de la linterna

    [Header("Búsqueda al perder al jugador")]
    public float searchDuration = 3f;        // Tiempo total buscando
    public float searchTurnInterval = 0.5f;  // Cada cuánto cambia de dirección al buscar

    [Header("Sonido")]
    public AudioSource audioSource;
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

    // Ataque
    private float attackTimer = 0f;

    // Búsqueda
    private float searchTimer = 0f;
    private float searchTurnTimer = 0f;

    // Animación
    private Animator animator;
    private Vector2 lastMoveDir = Vector2.down;   // Dirección inicial (mirando hacia abajo)

    // Jugador
    private Transform player;

    // Rigidbody del enemigo (DYNAMIC)
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("EnemyRandomMovement: No hay Rigidbody2D en el enemigo.");
        }

        startPosition = transform.position;
        animator = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("EnemyRandomMovement: No se encontró ningún objeto con tag 'Player'.");
        }

        // Si no se asignó la luz desde el inspector, intentamos buscarla en el pivot
        if (flashlightPivot != null && flashlightLight == null)
        {
            flashlightLight = flashlightPivot.GetComponentInChildren<Light2D>();
        }

        // Audio por defecto
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        PickNewTarget();
        UpdateAnimator(Vector2.zero, false);
    }

    private void Update()
    {
        if (player != null)
        {
            UpdateStateByVisionAndDistance();
        }

        switch (currentState)
        {
            case EnemyState.Chase:
                ChasePlayer();
                break;

            case EnemyState.Patrol:
                Patrol();
                break;

            case EnemyState.Attack:
                AttackBehaviour();
                break;

            case EnemyState.Search:
                SearchBehaviour();
                break;
        }

        // Contador de cooldown del ataque
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    // --- LÓGICA DE ESTADO (visión + linterna + búsqueda + ataque) ---

    private void UpdateStateByVisionAndDistance()
    {
        Vector2 enemyPos = rb != null ? rb.position : (Vector2)transform.position;
        Vector2 playerPos = player.position;
        float distance = Vector2.Distance(enemyPos, playerPos);

        bool canSeeByOwnVision = CanSeePlayer(enemyPos, playerPos);
        bool isIlluminatedByFlashlight = IsIlluminatedByFlashlight(enemyPos);

        bool detected = canSeeByOwnVision || isIlluminatedByFlashlight;

        if (detected && distance <= attackRange)
        {
            // Lo ve (o lo ilumina la linterna) y está cerca → Attack
            currentState = EnemyState.Attack;
        }
        else if (detected)
        {
            // Detectado pero lejos → Chase
            currentState = EnemyState.Chase;
        }
        else
        {
            // No lo ve / no lo ilumina
            if ((currentState == EnemyState.Chase || currentState == EnemyState.Attack) && distance <= loseRadius)
            {
                // Lo acaba de perder de vista estando cerca → entra en búsqueda
                if (searchTimer <= 0f)
                {
                    searchTimer = searchDuration;
                    searchTurnTimer = 0f;
                }
                currentState = EnemyState.Search;
            }
            else if (currentState == EnemyState.Search)
            {
                // SearchBehaviour decide cuándo volver a patrullar
            }
            else
            {
                currentState = EnemyState.Patrol;
            }
        }
    }

    // --- Visión propia del enemigo (cono frente a él) ---

    private bool CanSeePlayer(Vector2 enemyPos, Vector2 playerPos)
    {
        Vector2 toPlayer = playerPos - enemyPos;
        float distanceToPlayer = toPlayer.magnitude;

        if (distanceToPlayer > detectionRadius)
            return false;

        Vector2 dirToPlayer = toPlayer.normalized;
        Vector2 facingDir = lastMoveDir.sqrMagnitude > 0.001f ? lastMoveDir.normalized : Vector2.down;

        float angle = Vector2.Angle(facingDir, dirToPlayer);
        if (angle > viewAngle * 0.5f)
        {
            return false;
        }

        return true;
    }

    // --- Detección cuando la linterna lo alumbra ---

    private bool IsIlluminatedByFlashlight(Vector2 enemyPos)
    {
        if (flashlightPivot == null)
            return false;

        if (flashlightLight != null && !flashlightLight.enabled)
            return false;

        Vector2 flashlightPos = flashlightPivot.position;
        Vector2 toEnemy = enemyPos - flashlightPos;
        float distanceToEnemy = toEnemy.magnitude;

        if (distanceToEnemy > flashlightDetectRadius)
            return false;

        Vector2 dirToEnemy = toEnemy.normalized;

        // Usa .up o .right según cómo esté orientado tu pivot
        Vector2 flashlightDir = flashlightPivot.up;

        float angle = Vector2.Angle(flashlightDir, dirToEnemy);
        if (angle > flashlightDetectAngle * 0.5f)
        {
            return false;
        }

        return true;
    }

    // --- PATRULLA ---

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

    // --- PERSECUCIÓN ---

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

        if (distance <= attackRange)
        {
            return;
        }

        Vector2 direction = (target - current).normalized;
        lastMoveDir = direction;

        Vector2 newPos = current + direction * chaseSpeed * Time.deltaTime;
        rb.MovePosition(newPos);

        UpdateAnimator(direction, true);

        isIdle = false;
    }

    // --- ATAQUE ---

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
        {
            lastMoveDir = dir;
        }

        UpdateAnimator(Vector2.zero, false);

        if (attackTimer <= 0f)
        {
            if (animator != null)
            {
                animator.SetTrigger("attackTrigger");
            }

            // --- DAÑO AL PLAYER ---
            HealthSystem_Player hp = player.GetComponent<HealthSystem_Player>();
            if (hp != null)
            {
                hp.TakeDamage(damage); // El enemigo hace 'damage' puntos de daño
            }

            attackTimer = attackCooldown;
        }
    }

    // --- BÚSQUEDA ---

    private void SearchBehaviour()
    {
        if (player == null)
        {
            currentState = EnemyState.Patrol;
            return;
        }

        // Tiempo total buscando
        searchTimer -= Time.deltaTime;
        if (searchTimer <= 0f)
        {
            currentState = EnemyState.Patrol;
            PickNewTarget();
            return;
        }

        // Cada cierto tiempo cambia la dirección a la que mira
        searchTurnTimer -= Time.deltaTime;
        if (searchTurnTimer <= 0f)
        {
            Vector2[] dirs = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
            int index = Random.Range(0, dirs.Length);
            lastMoveDir = dirs[index];

            UpdateAnimator(Vector2.zero, false);

            searchTurnTimer = searchTurnInterval;
        }
        // No se mueve, solo "escanea" alrededor
    }

    // --- ANIMACIÓN ---

    private void UpdateAnimator(Vector2 dir, bool moving)
    {
        if (animator == null) return;

        animator.SetBool("isMoving", moving);

        if (dir.sqrMagnitude < 0.001f)
        {
            dir = lastMoveDir;
        }

        animator.SetFloat("moveX", dir.x);
        animator.SetFloat("moveY", dir.y);
    }

    // --- SONIDOS (llamados desde Animation Events) ---

    public void PlayFootstep()
    {
        if (audioSource != null && footstepClip != null)
        {
            audioSource.PlayOneShot(footstepClip, footstepVolume);
        }
    }

    public void PlayAttackSound()
    {
        if (audioSource != null && attackClip != null)
        {
            audioSource.PlayOneShot(attackClip, attackVolume);
        }
    }

    // --- DEBUG DE COLISIÓN ---

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Enemy chocó con: " + collision.collider.name);
    }

    // --- GIZMOS ---

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