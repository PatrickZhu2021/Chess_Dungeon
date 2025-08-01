using UnityEngine;
using System.Collections.Generic;

public class A06 : Monster
{
    // 定义国王的所有可能移动方向（仅限于周围一格）
    private static readonly Vector2Int[] kingDirections = new Vector2Int[]
    {
        new Vector2Int(1, 0), new Vector2Int(-1, 0),  // 水平方向
        new Vector2Int(0, 1), new Vector2Int(0, -1),  // 垂直方向
        new Vector2Int(1, 1), new Vector2Int(1, -1),  // 对角线方向
        new Vector2Int(-1, 1), new Vector2Int(-1, -1)
    };

    public override void Initialize(Vector2Int startPos)
    {
        health = 6;
        base.Initialize(startPos);
        type = MonsterType.Beast;
        monsterName = "A06";
        displayName = "年迈的狮王";
    }

    public override void PerformMovement()
    {
        if (player == null) return;
        Vector2Int targetPos = GetTargetPosition();

        // 检查是否能直接攻击到玩家（相邻位置）
        bool canAttackPlayer = false;
        foreach (Vector2Int direction in kingDirections)
        {
            Vector2Int adjacentPos = position + direction;
            if (adjacentPos == targetPos)
            {
                canAttackPlayer = true;
                break;
            }
        }

        if (canAttackPlayer)
        {
            // 可以攻击玩家，正常移动
            Vector2Int bestMove = position;
            float closestDistance = Vector2Int.Distance(position, targetPos);
            Vector2Int chosenDirection = Vector2Int.zero;

            foreach (Vector2Int direction in kingDirections)
            {
                Vector2Int potentialPosition = position + direction;

                if (IsValidPosition(potentialPosition) && !IsPositionOccupied(potentialPosition))
                {
                    float distanceToTarget = Vector2Int.Distance(potentialPosition, targetPos);
                    if (distanceToTarget < closestDistance)
                    {
                        bestMove = potentialPosition;
                        closestDistance = distanceToTarget;
                        chosenDirection = direction;
                    }
                }
            }

            position = bestMove;
            UpdatePosition();

            if (position == targetPos)
            {
                lastRelativePosition = -chosenDirection;
            }
        }
        else
        {
            // 无法攻击到玩家，触发号角召唤
            TriggerHornSummon();
        }
    }

    private void TriggerHornSummon()
    {
        // 获取相邻位置
        Vector2Int[] adjacentPositions = new Vector2Int[]
        {
            position + Vector2Int.up,
            position + Vector2Int.down,
            position + Vector2Int.left,
            position + Vector2Int.right,
            position + new Vector2Int(1, 1),   // 右上
            position + new Vector2Int(-1, 1),  // 左上
            position + new Vector2Int(1, -1),  // 右下
            position + new Vector2Int(-1, -1)  // 左下
        };

        // 筛选有效的召唤位置
        List<Vector2Int> validSummonPositions = new List<Vector2Int>();
        foreach (Vector2Int pos in adjacentPositions)
        {
            if (IsValidPosition(pos) && !IsPositionOccupied(pos) && pos != player.position)
            {
                validSummonPositions.Add(pos);
            }
        }

        // 如果有有效位置，随机选择一个召唤豺犬或郊狼
        if (validSummonPositions.Count > 0)
        {
            Vector2Int summonPos = validSummonPositions[Random.Range(0, validSummonPositions.Count)];
            SummonBeast(summonPos);
        }
        else
        {
            Debug.Log($"{displayName} tried to summon but no valid positions available");
        }
    }

    private void SummonBeast(Vector2Int summonPosition)
    {
        // 随机选择召唤豺犬(A01)或郊狼(A04)
        string[] beastTypes = { "A01", "A04" };
        string selectedType = beastTypes[Random.Range(0, beastTypes.Length)];

        MonsterManager monsterManager = FindObjectOfType<MonsterManager>();
        if (monsterManager != null)
        {
            Monster beast = monsterManager.CreateMonsterByType(selectedType);
            if (beast != null)
            {
                beast.transform.position = player.CalculateWorldPosition(summonPosition);
                beast.Initialize(summonPosition);
                
                // 设置召唤的野兽血量为1
                beast.health = 1;
                beast.maxHealth = 1;
                
                // 将召唤的怪物添加到怪物管理器
                monsterManager.monsters.Add(beast);
                
                string beastName = selectedType == "A01" ? "豺犬" : "郊狼";
                Debug.Log($"{displayName} summoned a {beastName} at {summonPosition}");
            }
        }
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/A06");
    }

    public override List<Vector2Int> CalculatePossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        foreach (Vector2Int direction in kingDirections)
        {
            Vector2Int potentialPosition = position + direction;
            if (IsValidPosition(potentialPosition) && !IsPositionOccupied(potentialPosition))
            {
                possibleMoves.Add(potentialPosition);
            }
        }

        return possibleMoves;
    }

    public override List<string> GetSkills()
    {
        return new List<string> { "号角" };
    }
}