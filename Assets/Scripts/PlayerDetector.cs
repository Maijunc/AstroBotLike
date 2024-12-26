using UnityEngine;
using static Timer;

public class PlayerDetector : MonoBehaviour 
{
    [SerializeField] float detectionAngle = 60f; //Cone in front of enemy
    [SerializeField] float detectionRadius = 10f; //Large circle around enemy
    [SerializeField] float innerDetectionRadius = 5f; //Small circle around enemy
    [SerializeField] float detectionCooldown = 1f; //Cooldown between detections

    public Transform Player { get; private set; }
    CountdownTimer detectionTimer; 
    
    IDetectionStrategy detectionStrategy;

    void Start()
    {
        detectionTimer = new CountdownTimer(detectionCooldown);
        Player = GameObject.FindGameObjectWithTag("Player").transform; // Make sure to tag the player object with "Player"
        detectionStrategy = new ConeDetectionStrategy(detectionAngle, detectionRadius, innerDetectionRadius);
    }

    void Update() => detectionTimer.Tick(Time.deltaTime);

    // 如果Timer.IsRunning为true，表示已经侦测到玩家，否则执行侦测策略
    public bool CanDetectPlayer()
    {
        return detectionTimer.IsRunning || detectionStrategy.Execute(Player, transform, detectionTimer);
    }

    public void SetDetectionStrategy(IDetectionStrategy strategy) => detectionStrategy = strategy;
}


