using UnityEngine;
using UnityEngine.AI;
public class EnemyFollow : MonoBehaviour
{
    public NavMeshAgent enemy;
    public Transform player;

    [SerializeField]
    private float timer = 5;
    private float bullerTime;

    public GameObject enemyBullet;
    public Transform spawnPoint;
    public float enemySpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        enemy.SetDestination(player.position);
        ShootAtPlayer();
    }

    void ShootAtPlayer()
    {
        bullerTime -= Time.deltaTime;

        if (bullerTime > 0)
            return;

        bullerTime = timer;

        GameObject bulletObj = Instantiate(enemyBullet, spawnPoint.transform.position, spawnPoint.transform.rotation) as GameObject;

        Destroy(bulletObj, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Destroy(bulletObj);
        }
    }
}
