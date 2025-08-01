using UnityEngine;
using System.Collections.Generic;

public class B02 : Monster
{
    public override void Initialize(Vector2Int startPos)
    {
        health = 1; // 设置初始血量为1
        base.Initialize(startPos);
        type = MonsterType.Pawn;
        monsterName = "B02";
        displayName = "偷猎者";
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
        
        // 检查是否与玩家在同一条线上（水平或垂直）
        if (IsInLineWithTarget(targetPos))
        {
            // 如果在同一条线上，直接攻击
            AttackTarget(targetPos);
        }
        else
        {
            // 如果不在同一条线上，尝试移动到与玩家对齐的位置
            MoveToAlignWithTarget(targetPos);
        }
    }

    private bool IsInLineWithTarget(Vector2Int targetPos)
    {
        // 检查是否在同一行或同一列
        return position.x == targetPos.x || position.y == targetPos.y;
    }

    private void AttackTarget(Vector2Int targetPos)
    {
        // 检查攻击路径是否被阻挡
        if (HasClearLineOfSight(targetPos))
        {
            // 对目标造成1点伤害
            if (targetPos == player.position)
            {
                player.TakeDamage(1);
                Debug.Log($"{displayName} shot the player for 1 damage");
            }
            else
            {
                // 如果目标是其他怪物（瞩目目标）
                Monster targetMonster = GetMonsterAtPosition(targetPos);
                if (targetMonster != null)
                {
                    targetMonster.TakeDamage(1);
                    Debug.Log($"{displayName} shot {targetMonster.displayName} for 1 damage");
                }
            }
        }
        else
        {
            Debug.Log($"{displayName} cannot shoot - line of sight blocked");
        }
    }

    private void MoveToAlignWithTarget(Vector2Int targetPos)
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        // 所有可能的移动方向：上下左右
        possibleMoves.Add(new Vector2Int(position.x + 1, position.y));  // 右
        possibleMoves.Add(new Vector2Int(position.x - 1, position.y));  // 左
        possibleMoves.Add(new Vector2Int(position.x, position.y + 1));  // 上
        possibleMoves.Add(new Vector2Int(position.x, position.y - 1));  // 下

        // 优先选择能与目标对齐的移动
        Vector2Int bestMove = position;
        float bestScore = float.MaxValue;

        foreach (Vector2Int move in possibleMoves)
        {
            if (!IsPositionOccupied(move) && IsValidPosition(move))
            {
                float score = CalculateAlignmentScore(move, targetPos);
                if (score < bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }
        }

        if (bestMove != position)
        {
            position = bestMove;
            UpdatePosition();
            Debug.Log($"{displayName} moved to align with target at {targetPos}");
        }
    }

    private float CalculateAlignmentScore(Vector2Int movePos, Vector2Int targetPos)
    {
        // 如果移动后能与目标对齐，给予最高优先级
        if (movePos.x == targetPos.x || movePos.y == targetPos.y)
        {
            return 0f; // 最佳分数
        }
        
        // 否则计算到对齐位置的距离
        float horizontalAlignDistance = Mathf.Abs(movePos.x - targetPos.x);
        float verticalAlignDistance = Mathf.Abs(movePos.y - targetPos.y);
        
        // 返回到最近对齐线的距离
        return Mathf.Min(horizontalAlignDistance, verticalAlignDistance);
    }

    private bool HasClearLineOfSight(Vector2Int targetPos)
    {
        Vector2Int direction = new Vector2Int(
            targetPos.x > position.x ? 1 : (targetPos.x < position.x ? -1 : 0),
            targetPos.y > position.y ? 1 : (targetPos.y < position.y ? -1 : 0)
        );

        Vector2Int checkPos = position + direction;
        
        while (checkPos != targetPos)
        {
            if (IsPositionOccupied(checkPos) || !IsValidPosition(checkPos))
            {
                return false; // 路径被阻挡
            }
            checkPos += direction;
        }
        
        return true; // 路径畅通
    }

    private Monster GetMonsterAtPosition(Vector2Int pos)
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monsterObj in monsters)
        {
            Monster monster = monsterObj.GetComponent<Monster>();
            if (monster != null && monster != this && monster.position == pos)
            {
                return monster;
            }
        }
        return null;
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/B02");
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
        return new List<string> { "弓", "远程攻击" };
    }
}