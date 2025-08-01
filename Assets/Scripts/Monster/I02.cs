using UnityEngine;
using System.Collections.Generic;

public class I02 : Monster
{
    public override void Initialize(Vector2Int startPos)
    {
        health = 1; // 设置初始血量为1
        base.Initialize(startPos);
        type = MonsterType.Beast;
        monsterName = "I02";
        displayName = "剜目者";
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }

    public override void Die()
    {
        base.Die();
    }

    public override void PerformMovement()
    {
        if (player == null) return;
        lastRelativePosition = position - player.position;
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        // 斜向移动：四个对角方向
        possibleMoves.Add(new Vector2Int(position.x + 1, position.y + 1));  // 右上
        possibleMoves.Add(new Vector2Int(position.x - 1, position.y + 1));  // 左上
        possibleMoves.Add(new Vector2Int(position.x + 1, position.y - 1));  // 右下
        possibleMoves.Add(new Vector2Int(position.x - 1, position.y - 1));  // 左下

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
            Debug.Log("Target reached by I02.");
        }
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/I02");
    }

    public override List<Vector2Int> CalculatePossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>
        {
            position + new Vector2Int(1, 1),   // 右上
            position + new Vector2Int(-1, 1),  // 左上
            position + new Vector2Int(1, -1),  // 右下
            position + new Vector2Int(-1, -1)  // 左下
        };

        // 过滤掉无效位置或被占据的位置
        possibleMoves.RemoveAll(pos => !IsValidPosition(pos) || IsPositionOccupied(pos));
        return possibleMoves;
    }

    public override List<string> GetSkills()
    {
        return new List<string> { "锋利" };
    }
    
    public override int GetAttackDamage()
    {
        return 2; // 锋利效果：额外造成一点伤害（1+1=2）
    }
}