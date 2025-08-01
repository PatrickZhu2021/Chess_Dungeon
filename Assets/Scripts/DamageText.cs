using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageText : MonoBehaviour
{
    public Text damageText;
    public float moveSpeed = 10f;
    public float fadeSpeed = 2f;
    public float lifetime = 5f;
    
    private Color originalColor;
    
    void Start()
    {
        if (damageText == null)
            damageText = GetComponent<Text>();
            
        originalColor = damageText.color;
        
        // 设置渲染层级
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
        }
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.overrideSorting = true;
        canvas.sortingLayerName = "UI";
        canvas.sortingOrder = 100;
        
        // 设置合适的缩放
        transform.localScale = Vector3.one * 0.01f;
        
        StartCoroutine(AnimateDamageText()); // 暂时注释掉动画
    }
    
    private IEnumerator AnimateDamageText()
    {
        float timer = 0f;
        Vector3 startPos = transform.position;
        
        while (timer < lifetime)
        {
            timer += Time.deltaTime;
            
            // 向上移动
            transform.position = startPos + Vector3.up * (moveSpeed * timer);
            
            // 淡出
            float alpha = Mathf.Lerp(1f, 0f, timer / lifetime);
            damageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            
            yield return null;
        }
        
        Destroy(gameObject);
    }
    
    public void SetDamage(int damage, bool isHeal = false)
    {
        damageText.text = (isHeal ? "+" : "-") + damage.ToString();
        damageText.color = isHeal ? Color.green : Color.red;
        originalColor = damageText.color;
    }
}