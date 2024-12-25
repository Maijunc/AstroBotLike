using UnityEngine;

public class GameRespawn : MonoBehaviour
{
    public float threshold;

    public Vector3 spawnPoint;

    // Update is called once per frame
    void FixedUpdate() 
    {
        if (transform.position.y < threshold)
        {
            transform.position = spawnPoint;
        }
    }
}
