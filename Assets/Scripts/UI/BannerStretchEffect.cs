using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BannerStretchEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("横幅设置")]
    public Image banner; // 横幅图像
    public float stretchAmount = 50f; // 拉伸量
    public float animationSpeed = 5f; // 动画速度
    
    private RectTransform bannerRect;
    private Vector2 originalSize;
    private Vector3 originalPosition;
    
    void Start()
    {
        if (banner != null)
        {
            bannerRect = banner.GetComponent<RectTransform>();
            originalSize = bannerRect.sizeDelta;
            originalPosition = bannerRect.anchoredPosition;
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (bannerRect != null)
        {
            StopAllCoroutines();
            StartCoroutine(StretchBanner(true));
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (bannerRect != null)
        {
            StopAllCoroutines();
            StartCoroutine(StretchBanner(false));
        }
    }
    
    private System.Collections.IEnumerator StretchBanner(bool stretch)
    {
        Vector2 targetSize = stretch ? new Vector2(originalSize.x + stretchAmount, originalSize.y) : originalSize;
        Vector3 targetPosition = stretch ? new Vector3(originalPosition.x - stretchAmount/2, originalPosition.y, originalPosition.z) : originalPosition;
        
        while (Vector2.Distance(bannerRect.sizeDelta, targetSize) > 0.1f)
        {
            bannerRect.sizeDelta = Vector2.Lerp(bannerRect.sizeDelta, targetSize, Time.deltaTime * animationSpeed);
            bannerRect.anchoredPosition = Vector3.Lerp(bannerRect.anchoredPosition, targetPosition, Time.deltaTime * animationSpeed);
            yield return null;
        }
        
        bannerRect.sizeDelta = targetSize;
        bannerRect.anchoredPosition = targetPosition;
    }
}