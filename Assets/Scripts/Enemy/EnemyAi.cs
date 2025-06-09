using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    public NavMeshAgent agent;

    [SerializeField] private Transform player; // Ahora se puede asignar en el Inspector

    public LayerMask whatIsGround, whatIsPlayer, whatIsCeiling;
    public float health;

    // Patrullaje
    public Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;

    // Ataque
    public float timeBetweenAttacks;
    private bool alreadyAttacked;
    public GameObject projectile;

    // Estados
    public float sightRange, attackRange;
    private bool playerInSightRange, playerInAttackRange;

    // Agacharse
    public float ceilingCheckDistance = 1.5f;
    public float crouchHeight = 0.5f;
    public float normalHeight = 1f;

    [Header("Salto")]
    public float jumpDuration = 0.8f;
    public float jumpHeight = 1.5f;
    public float landingOffset = 0.05f;
    private bool isJumping = false;
    private CapsuleCollider enemyCollider;

    [Header("Zona de actividad")]
    public float zoneRadius = 20f;
    private bool playerInZone = true;
    private float timeOutsideZone = 0f;
    public float timeBeforeDespawn = 5f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyCollider = GetComponent<CapsuleCollider>();

        // Asignar `player` solo si no se ha establecido en el Inspector
        if (player == null)
        {
            GameObject playerObj = GameObject.Find("PlayerObj");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("No se encontró el objeto 'PlayerObj'. Asegúrate de asignar el Player en el prefab.");
            }
        }
    }

    private void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            playerInZone = distanceToPlayer <= zoneRadius;

            if (!playerInZone)
            {
                timeOutsideZone += Time.deltaTime;
                if (timeOutsideZone >= timeBeforeDespawn)
                {
                    NotifyDespawnAndDestroy();
                    return;
                }
            }
            else
            {
                timeOutsideZone = 0f;
            }
        }

        CheckCeiling();

        if (agent.isOnOffMeshLink && !isJumping)
        {
            StartCoroutine(JumpOverLink());
            return;
        }

        if (isJumping) return;

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private System.Collections.IEnumerator JumpOverLink()
    {
        isJumping = true;

        OffMeshLinkData linkData = agent.currentOffMeshLinkData;
        Vector3 startPos = transform.position;
        Vector3 endPos = linkData.endPos;

        float originalBaseOffset = agent.baseOffset;
        agent.isStopped = true;
        agent.updatePosition = false;
        agent.updateRotation = false;

        bool earlyLanding = false;
        float minJumpHeight = 0.2f;

        float time = 0f;
        while (time < jumpDuration && !earlyLanding)
        {
            float t = time / jumpDuration;
            float verticalOffset = Mathf.Sin(t * Mathf.PI) * jumpHeight;

            Vector3 newPos = Vector3.Lerp(startPos, endPos, t) + Vector3.up * verticalOffset;
            transform.position = newPos;

            if (verticalOffset > minJumpHeight)
            {
                if (NavMesh.SamplePosition(newPos, out NavMeshHit hit, 0.5f, NavMesh.AllAreas))
                {
                    if (Physics.Raycast(newPos + Vector3.up * 0.5f, Vector3.down, out RaycastHit groundHit, 1f, whatIsGround))
                    {
                        endPos = new Vector3(newPos.x, groundHit.point.y + (enemyCollider.height / 2 * transform.localScale.y) + landingOffset, newPos.z);
                        earlyLanding = true;
                    }
                }
            }

            time += Time.deltaTime;
            yield return null;
        }

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit finalGroundHit, 2f, whatIsGround))
        {
            endPos = new Vector3(endPos.x,
                               finalGroundHit.point.y + (enemyCollider.height / 2 * transform.localScale.y) + landingOffset,
                               endPos.z);
        }

        transform.position = endPos;
        agent.Warp(endPos);

        agent.baseOffset = originalBaseOffset;
        agent.isStopped = false;
        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.CompleteOffMeshLink();

        yield return null;

        isJumping = false;

        if (playerInSightRange) ChasePlayer();
        else Patroling();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        if ((transform.position - walkPoint).magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        if (player != null)
            agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            GameObject projectileInstance = Instantiate(projectile, transform.position, Quaternion.identity);
            Rigidbody rb = projectileInstance.GetComponent<Rigidbody>();

            Projectile projectileScript = projectileInstance.AddComponent<Projectile>();
            projectileScript.whatIsPlayer = whatIsPlayer;

            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void CheckCeiling()
    {
        if (Physics.Raycast(transform.position, Vector3.up, ceilingCheckDistance, whatIsCeiling))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, normalHeight, transform.localScale.z);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, zoneRadius);

    }

    public void SetPlayerReference(Transform playerTransform)
    {
        player = playerTransform;
    }

    public void SetTarget(Transform target)
    {
        player = target;
    }

    private void NotifyDespawnAndDestroy()
    {
        GameManager.Instance?.RemoveEnemyFromList(gameObject);
        Destroy(gameObject);
    }


}
