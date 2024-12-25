using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnCoinScript : MonoBehaviour
{
    public GameObject coinPrefab;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 randomPosition = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            Instantiate(coinPrefab, randomPosition, Quaternion.identity);
        }
    }
}
