using UnityEngine;
using System.Collections.Generic;

public class I04 : Monster
{
    private static readonly Vector2Int[] knightMoves = new Vector2Int[]
    {
        new Vector2Int(2, 1), new Vector2Int(2, -1),
        new Vector2Int(-2, 1), new Vector2Int(-2, -1),
        new Vector2Int(1, 2), new Vector2Int(1, -2),
        new Vector2Int(-1, 2), new Vector2Int(-1, -2)
    };

    public override void Initialize(Vector2Int startPos)
    {
        health = 2; // 设置初始血量为2
        base.Initialize(startPos);
        type = MonsterType.Knight;
        monsterName = "I04";
        displayName = "怒啸者";
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
        
        Vector2Int targetPos = GetTargetPosition();
        Vector2Int oldPos = position;
        Vector2Int bestMove = position;
        float closestDistance = Vector2Int.Distance(position, targetPos);

        // 遍历所有可能的马跳跃位置
        foreach (Vector2Int move in knightMoves)
        {
            Vector2Int potentialPosition = position + move;
            if (IsValidPosition(potentialPosition) && !IsPositionOccupied(potentialPosition))
            {
                float distanceToTarget = Vector2Int.Distance(potentialPosition, targetPos);
                if (distanceToTarget < closestDistance)
                {
                    bestMove = potentialPosition;
                    closestDistance = distanceToTarget;
                }
            }
        }

        // 计算这次移动的向量（用于记录进攻方向）
        Vector2Int knightMove = bestMove - oldPos;
        position = bestMove;
        UpdatePosition();

        // 移动后检查扩散技能
        CheckSpreadDamage();

        // 检测是否接触到目标
        if (position == targetPos)
        {
            lastRelativePosition = ComputeKnightPushDirection(knightMove);
            Debug.Log("Knight reached target. lastRelativePosition set to knight move vector: " + lastRelativePosition);
        }
        else
        {
            lastRelativePosition = position - player.position;
        }
    }

    private void CheckSpreadDamage()
    {
        // 检查是否与玩家十字相邻（上下左右）
        Vector2Int[] adjacentPositions = new Vector2Int[]
        {
            position + Vector2Int.up,    // 上
            position + Vector2Int.down,  // 下
            position + Vector2Int.left,  // 左
            position + Vector2Int.right  // 右
        };

        foreach (Vector2Int adjPos in adjacentPositions)
        {
            if (adjPos == player.position)
            {
                // 对玩家造成1点伤害
                player.TakeDamage(1);
                Debug.Log($"{displayName} spread damage: dealt 1 damage to player");
                break;
            }
        }
    }

    private Vector2Int ComputeKnightPushDirection(Vector2Int knightMove)
    {
        // 如果垂直分量更大，则以垂直方向为主
        if (Mathf.Abs(knightMove.y) > Mathf.Abs(knightMove.x))
        {
            return new Vector2Int(0, (int)Mathf.Sign(-knightMove.y));
        }
        else
        {
            // 否则以水平方向为主
            return new Vector2Int((int)Mathf.Sign(-knightMove.x), 0);
        }
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/I04");
    }

    public override List<Vector2Int> CalculatePossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        foreach (Vector2Int move in knightMoves)
        {
            Vector2Int potentialPosition = position + move;
            if (IsValidPosition(potentialPosition) && !IsPositionOccupied(potentialPosition))
            {
                possibleMoves.Add(potentialPosition);
            }
        }

        return possibleMoves;
    }

    public override List<string> GetSkills()
    {
        return new List<string> { "扩散" };
    }
}