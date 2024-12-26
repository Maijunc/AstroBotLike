using UnityEngine;
using static Timer;

public interface IDetectionStrategy
{
    bool Execute(Transform player, Transform detector, CountdownTimer timer);
}


