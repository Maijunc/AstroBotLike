using UnityEngine;

public class CoinScript : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(0f, 1f, 0f, Space.Self);
    }
}
