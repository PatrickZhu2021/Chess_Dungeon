using UnityEngine;
using System.Collections.Generic;

public class I01 : Monster
{
    public override void Initialize(Vector2Int startPos)
    {
        base.Initialize(startPos);
        type = MonsterType.Pawn;
        monsterName = "I01";
        displayName = "爬行者";
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
        Vector2Int direction = player.position - position;
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
            Debug.Log("Target reached by I01.");
        }
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/I01");
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
}