using UnityEngine;
using System.Collections;

public static class DamageTextManager
{
    private static float lastDamageTime = 0f;
    private const float damageInterval = 0.05f;
    
    public static void ShowDamageText(MonoBehaviour caller, int damage, Vector3 worldPosition, GameObject damageTextPrefab, bool isHeal = false)
    {
        if (damage <= 0) return; // 只显示大于0的伤害
        
        caller.StartCoroutine(ShowDamageTextCoroutine(damage, worldPosition, damageTextPrefab, isHeal));
    }
    
    private static IEnumerator ShowDamageTextCoroutine(int damage, Vector3 worldPosition, GameObject damageTextPrefab, bool isHeal)
    {
        if (damageTextPrefab != null)
        {
            float currentTime = Time.time;
            float waitTime = Mathf.Max(0, lastDamageTime + damageInterval - currentTime);
            lastDamageTime = currentTime + waitTime + damageInterval;
            
            if (waitTime > 0)
            {
                yield return new WaitForSeconds(waitTime);
            }
            
            Vector3 randomOffset = new Vector3(
                Random.Range(-0.8f, 0.8f),
                Random.Range(-0.6f, 0.6f),
                0
            );
            Vector3 finalPos = worldPosition + randomOffset;
            
            GameObject damageObj = Object.Instantiate(damageTextPrefab, finalPos, Quaternion.identity);
            DamageText damageScript = damageObj.GetComponent<DamageText>();
            if (damageScript != null)
            {
                damageScript.SetDamage(damage, isHeal);
            }
        }
    }
}