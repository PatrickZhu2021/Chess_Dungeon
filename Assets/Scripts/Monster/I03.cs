using UnityEngine;
using System.Collections.Generic;

public class I03 : Monster
{
    private bool bloodFeastUsed = false; // 血食技能是否已使用
    private bool needsBloodFeastCheck = false; // 是否需要检查血食

    public override void Initialize(Vector2Int startPos)
    {
        health = 4; // 设置初始血量为4
        base.Initialize(startPos);
        type = MonsterType.Pawn;
        monsterName = "I03";
        displayName = "肿胀者";
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        
        // 受伤后标记需要检查血食
        if (!bloodFeastUsed && health > 0)
        {
            needsBloodFeastCheck = true;
        }
    }

    public override void Die()
    {
        base.Die();
    }

    public override void MoveTowardsPlayer()
    {
        // 检查是否需要血食
        if (needsBloodFeastCheck && !bloodFeastUsed && health <= maxHealth / 2)
        {
            needsBloodFeastCheck = false;
            TriggerBloodFeast();
            return; // 血食回合不进行移动
        }
        
        needsBloodFeastCheck = false; // 清除标记
        
        // 调用父类的移动逻辑
        base.MoveTowardsPlayer();
    }
    
    public override void PerformMovement()
    {

        // 正常移动逻辑（与I01相同）
        if (player == null) return;
        lastRelativePosition = position - player.position;
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        // 所有可能的移动方向：上下左右
        possibleMoves.Add(new Vector2Int(position.x + 1, position.y));  // 右
        possibleMoves.Add(new Vector2Int(position.x - 1, position.y));  // 左
        possibleMoves.Add(new Vector2Int(position.x, position.y + 1));  // 上
        possibleMoves.Add(new Vector2Int(position.x, position.y - 1));  // 下

        Vector2Int targetPos = GetTargetPosition();
        // 按照接近目标的优先级排序
        possibleMoves.Sort((a, b) => Vector2Int.Distance(a, targetPos).CompareTo(Vector2Int.Distance(b, targetPos)));

        // 遍历所有可能的移动方向，找到第一个有效的移动
        foreach (Vector2Int move in possibleMoves)
        {
            if (!IsPositionOccupied(move) && IsValidPosition(move))
            {
                position = move;
                UpdatePosition();
                break;
            }
        }

        // 检测是否接触到目标
        if (position == targetPos)
        {
            Debug.Log("Target reached by I03.");
        }
    }

    private void TriggerBloodFeast()
    {
        health += 2; // 回复2点生命
        if (health > maxHealth)
        {
            health = maxHealth; // 不超过最大生命值
        }
        
        bloodFeastUsed = true; // 标记技能已使用
        
        // 显示治疗数字
        ShowDamageText(2, true);
        
        Debug.Log($"{displayName} triggered Blood Feast: healed 2 HP, current health: {health}");
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/I03");
    }

    public override List<Vector2Int> CalculatePossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>
        {
            position + new Vector2Int(1, 0),   // 右
            position + new Vector2Int(-1, 0),  // 左
            position + new Vector2Int(0, 1),   // 上
            position + new Vector2Int(0, -1)   // 下
        };

        // 过滤掉无效位置或被占据的位置
        possibleMoves.RemoveAll(pos => !IsValidPosition(pos) || IsPositionOccupied(pos));
        return possibleMoves;
    }

    public override List<string> GetSkills()
    {
        if (bloodFeastUsed)
        {
            return new List<string>(); // 技能已使用，不显示
        }
        return new List<string> { "血食" };
    }
}