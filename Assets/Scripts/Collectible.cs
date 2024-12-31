using UnityEngine;

public class Collectible : Entity 
{
    [SerializeField] int score = 1; //FIXME set using factory
    [SerializeField] IntEventChannel scoreChannel;
    [SerializeField] PickedEffects pickedEffects;


    // 当碰撞到玩家时，增加分数并销毁自身
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            // 向Channel发布分数
            scoreChannel.Invoke(score);
            if(pickedEffects != null)
            {
                // 禁用碰撞体，避免再次触发碰撞
                GetComponent<Collider>().enabled = false;
                pickedEffects.PlayPickedSound();
                // 使用 Coroutine 等待音效播放完毕后销毁对象
                StartCoroutine(pickedEffects.ShrinkAndDestroy());
            }
        }
    }
}
