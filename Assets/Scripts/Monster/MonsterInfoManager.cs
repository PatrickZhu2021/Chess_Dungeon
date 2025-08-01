using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MonsterInfoManager : MonoBehaviour
{
    public GameObject MonsterInfoPanel; // 显示怪物信息的面板
    public Text MonsterNameText;        // 用于显示怪物名称的文本
    public Text MonsterHealthText;     // 用于显示怪物血量的文本
    public Text MonsterPositionText;   // 用于显示怪物位置的文本
    public Text MonsterEffectsText;    // 用于显示特殊效果的文本

    // 更新怪物信息的面板内容
    public void UpdateMonsterInfo(string name, int health, Vector2Int position, Monster monster = null)
    {
        if (MonsterInfoPanel != null)
        {
            MonsterInfoPanel.SetActive(true); // 确保面板是可见的
            MonsterNameText.text = $"Name: {name}";
            MonsterHealthText.text = $"Health: {health}";
            MonsterPositionText.text = $"Position: {position.x}, {position.y}";
            
            // 显示特殊效果
            if (MonsterEffectsText != null && monster != null)
            {
                string effects = GetEffectsText(monster);
                MonsterEffectsText.text = effects;
            }
        }
        else
        {
            Debug.LogWarning("MonsterInfoPanel is not assigned in the Inspector.");
        }
    }
    
    private string GetEffectsText(Monster monster)
    {
        List<string> effects = new List<string>();
        
        // 眩晕效果
        if (monster.stunnedStacks > 0)
        {
            effects.Add($"眩晕 {monster.stunnedStacks}");
        }
        
        // 瞩目效果
        if (monster.lureStacks > 0)
        {
            effects.Add($"瞩目 {monster.lureStacks}");
        }
        
        // 业力连接
        if (monster.HasKarmaLink())
        {
            effects.Add("业力连接");
        }
        
        return effects.Count > 0 ? $"Effects: {string.Join(", ", effects)}" : "Effects: 无";
    }

    // 隐藏怪物信息的面板
    public void HideMonsterInfo()
    {
        if (MonsterInfoPanel != null)
        {
            MonsterInfoPanel.SetActive(false); // 隐藏面板
        }
        else
        {
            Debug.LogWarning("MonsterInfoPanel is not assigned in the Inspector.");
        }
    }
}
