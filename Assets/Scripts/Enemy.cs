using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Transform m_Player;
    [SerializeField] private NavMeshAgent m_Agent;
    [SerializeField] private LayerMask m_GroundLayer;
    [SerializeField] private LayerMask m_PlayerLayer;

    //Chasing
    private Vector3 m_TargetPosition;

    [Header("Patrol")]
    private Vector3 m_WalkPoint;
    private bool m_WalkPointSet;
    [SerializeField] private float m_WalkPointRange;


    [Header("Attack")]
    [SerializeField] private float m_AttackCooldown;
    private bool m_Attacking;

    [Header("Range")]
    [SerializeField] private float m_AttackRange;
    [SerializeField] private float m_SightRange;
    private bool m_CanSeePlayer;
    private bool m_PlayerInAttackRange;

    public string CurrentState = "Patrol";


    private void Start()
    {
        InvokeRepeating("UpdateTargetPosition", 0.1f, 0.1f);
    }

    private void Update()
    {
        m_CanSeePlayer = Physics.CheckSphere(transform.position, m_SightRange, m_PlayerLayer);
        m_PlayerInAttackRange = Physics.CheckSphere(transform.position, m_AttackRange, m_PlayerLayer);

        if(!m_CanSeePlayer && !m_PlayerInAttackRange)
        {
            Patrol();
        }
        if (m_CanSeePlayer && !m_PlayerInAttackRange)
        {
            ChasePlayer();
        }
        if (m_CanSeePlayer && m_PlayerInAttackRange)
        {
            AttackPlayer();
        }
    }

    private void Patrol()
    {
        CurrentState = "Patrol";
        LookAtPoint();

        if (!m_WalkPointSet)
        {
            UpdateWalkPoint();
        }
        if(m_WalkPointSet)
        {
            m_Agent.SetDestination(m_WalkPoint);
        }

        Vector3 distanceRemaining = transform.position - m_WalkPoint;
        if (distanceRemaining.magnitude < 1f)
        {
            m_WalkPointSet = false;
        }
    }

    private void ChasePlayer()
    {
        CurrentState = "Chase";
        m_Agent.SetDestination(m_TargetPosition);
        LookAtPlayer();
    }

    private void AttackPlayer()
    {
        CurrentState = "Attack";
        m_Agent.SetDestination(transform.position);
      
        if(!m_Attacking)
        {
            m_Attacking = true;
            Invoke(nameof(ResetAttack), m_AttackCooldown);
        }
    }

    private void ResetAttack()
    {
        m_Attacking = false;
    }

    private void UpdateTargetPosition()
    {
        m_TargetPosition = m_Player.position;
    }

    private void LookAtPlayer()
    {
        Vector3 direction = m_Player.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 15f).eulerAngles;
        transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
    }

    private void LookAtPoint()
    {
        Vector3 direction = m_WalkPoint - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 1f).eulerAngles;
        transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
    }

    private void UpdateWalkPoint()
    {
        float randomX = Random.Range(-m_WalkPointRange, m_WalkPointRange);
        float randomZ = Random.Range(-m_WalkPointRange, m_WalkPointRange);

        m_WalkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(m_WalkPoint, -transform.up, 2f, m_GroundLayer))
            m_WalkPointSet = true;
    }

}
