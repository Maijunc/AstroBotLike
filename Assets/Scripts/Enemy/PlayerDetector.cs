using UnityEngine;
using static Timer;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] float detectionAngle = 60f; //Cone in front of enemy
    [SerializeField] float detectionRadius = 10f; //Large circle around enemy
    [SerializeField] float innerDetectionRadius = 5f; //Small circle around enemy
    [SerializeField] float detectionCooldown = 1f; //Cooldown between detections

    public Transform Player { get; private set; }
    public Health playerHealth { get; private set; }
    CountdownTimer detectionTimer;

    IDetectionStrategy detectionStrategy;

    void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform; // Make sure to tag the player object with "Player"
        playerHealth = Player.GetComponent<Health>();
    }

    void Start()
    {
        detectionTimer = new CountdownTimer(detectionCooldown);
        detectionStrategy = new ConeDetectionStrategy(detectionAngle, detectionRadius, innerDetectionRadius);
    }

    void Update() => detectionTimer.Tick(Time.deltaTime);

    // 如果Timer.IsRunning为true，表示已经侦测到玩家，否则执行侦测策略
    public bool CanDetectPlayer()
    {
        return detectionTimer.IsRunning || detectionStrategy.Execute(Player, transform, detectionTimer);
    }

    public bool CanAttackPlayer(float attackRange)
    {
        var directionToPlayer = Player.position - transform.position;
        return directionToPlayer.magnitude <= attackRange;
    }

    public void SetDetectionStrategy(IDetectionStrategy strategy) => detectionStrategy = strategy;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // Draw a spheres for the radii
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.DrawWireSphere(transform.position, innerDetectionRadius);

        // Calculate our cone directions
        Vector3 forwardConeDirection = Quaternion.Euler(0, detectionAngle / 2, 0) * transform.forward * detectionRadius;
        Vector3 backwardConeDirection = Quaternion.Euler(0, -detectionAngle / 2, 0) * transform.forward * detectionRadius;

        // Draw lines to represent the cone
        Gizmos.DrawLine(transform.position, transform.position + forwardConeDirection);
        Gizmos.DrawLine(transform.position, transform.position + backwardConeDirection);
    }

    public void GetPlayer()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public bool RedetectPlayer()
    {
        return detectionStrategy.Execute(Player, transform, detectionTimer);
    }
}