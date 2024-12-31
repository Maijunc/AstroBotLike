using UnityEngine;
using UnityEngine.UI;  // 如果使用 TextMeshPro，请使用 TMPro
using UnityEngine.EventSystems;
using TMPro;

public class ButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TMP_Text buttonText; // 普通UI Text，如果使用TextMeshPro，则改为 TMP_Text
    public Color hoverColor = Color.yellow; // 鼠标悬停时的颜色
    public Color normalColor = Color.white; // 正常状态下的颜色
    public Vector3 overdScale = new Vector3(1.2f, 1.2f, 1.2f); // 悬停时的缩放比例
    public Vector3 clickedScale = new Vector3(0.9f, 0.9f, 1f); // 点击时的缩放比例
    public Vector3 normalScale = new Vector3(1f, 1f, 1f); // 正常状态下的缩放比例
    public float scaleSpeed = 0.2f;     // 缩放平滑过渡的速度


    private Button button;
    private Vector3 targetScale;
    private bool isScaling = false;

    void Start()
    {
        button = GetComponent<Button>();
        if (buttonText == null)
        {
            buttonText = GetComponentInChildren<TMP_Text>(); // 自动寻找按钮内的 Text 组件
            // 如果使用 TextMesh Pro
            // buttonText = GetComponentInChildren<TMP_Text>();
        }

        // 设置按钮的默认文字颜色
        if (buttonText != null)
        {
            buttonText.color = normalColor;
        }

        // 初始目标缩放大小
        targetScale = normalScale;
    }

    void Update()
    {
        // 如果正在缩放，平滑地过渡到目标缩放
        if (isScaling)
        {
            button.transform.localScale = Vector3.Lerp(button.transform.localScale, targetScale, scaleSpeed);

            // 当缩放接近目标值时，停止缩放
            if (Vector3.Distance(button.transform.localScale, targetScale) < 0.01f)
            {
                isScaling = false;
            }
        }
    }

    // 鼠标进入按钮区域时
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.color = hoverColor;  // 改变文字颜色
        }
        if (button != null)
        {
            targetScale = overdScale;
            isScaling = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.color = normalColor;  // 改变文字颜色
        }
        if (button != null)
        {
            targetScale = normalScale;
            isScaling = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (button != null)
        {
            // 按下时，按钮缩放
            button.transform.localScale = clickedScale;
        }
    }

    // 恢复按钮大小
    private void RestoreScale()
    {
        targetScale = normalScale;  // 设置目标缩放为原始大小
        isScaling = true;            // 启动缩放
    }
}
