using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SpawnEffects : MonoBehaviour
{
    [SerializeField] GameObject spawnVFX;
    [SerializeField] float animationDuration = 1f;

    void Start() 
    {
        // 物体的初始缩放设置为 (0, 0, 0)，即物体是不可见的。
        transform.localScale = Vector3.zero;
        // SetEase(Ease.OutBack) 让动画有一个弹回效果，开始时较慢，结束时快速，形成“弹回”效果
        transform.DOScale(new Vector3(0.3f, 0.3f, 0.3f), animationDuration).SetEase(Ease.OutBack);

        if (spawnVFX != null)
        {
            Instantiate(spawnVFX, transform.position, Quaternion.identity);
        }

        GetComponent<AudioSource>().Play();
    }

}
