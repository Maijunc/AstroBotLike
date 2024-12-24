using UnityEngine;

public class GameRespawn : MonoBehaviour
{
    public float threshold;

    // Update is called once per frame
    void FixedUpdate() 
    {
        if (transform.position.y < threshold)
        {
            transform.position = new Vector3(0.078f, 3.3f, -1.49f);
        }
    }
}
