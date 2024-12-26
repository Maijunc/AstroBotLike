using UnityEngine;

public class Bullet : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}
