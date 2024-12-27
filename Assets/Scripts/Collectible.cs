using UnityEngine;
public class Collectible : Entity 
{
    [SerializeField] int score = 1; //FIXME set using factory
    [SerializeField] IntEventChannel scoreChannel;

    // 当碰撞到玩家时，增加分数并销毁自身
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            // 向Channel发布分数
            scoreChannel.Invoke(score);
            Destroy(gameObject);
        }
    }
}
