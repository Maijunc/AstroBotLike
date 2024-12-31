using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PickedEffects : MonoBehaviour 
{
    [SerializeField] GameObject pickedVFX;
    [SerializeField] AudioClip pickupSound; // 确保这里有一个音频文件
    [SerializeField] float animationDuration = 0.6f;
    [SerializeField] float vfxScaleFactor = 0.6f; //特效大小

    public void PlayPickedSound()
    {
        if(pickupSound != null)
            GetComponent<AudioSource>().PlayOneShot(pickupSound, 0.3f);
    }


    public IEnumerator ShrinkAndDestroy()
    {
        // 使用DOTween进行缩小，并且使用Ease.OutBack效果
        transform.DOScale(Vector3.zero, animationDuration).SetEase(Ease.OutBack);

        // 生成拾取特效并缩放
        if (pickedVFX != null)
        {
            GameObject vfx = Instantiate(pickedVFX, transform.position, Quaternion.identity);

            // 设置特效的缩放
            vfx.transform.localScale = new Vector3(vfxScaleFactor, vfxScaleFactor, vfxScaleFactor);

            // 销毁特效对象，延迟销毁时间
            Destroy(vfx, animationDuration);
        }

        // 等待音效播放完毕
        yield return new WaitForSeconds(pickupSound.length);

        // 销毁物体
        Destroy(gameObject);
    }
}