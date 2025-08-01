using UnityEngine;
using System.Collections.Generic;

public class I05 : Monster
{
    // 象的移动方向（四个对角线方向）
    private static readonly Vector2Int[] bishopDirections = new Vector2Int[]
    {
        new Vector2Int(1, 1),   // 右上
        new Vector2Int(-1, 1),  // 左上
        new Vector2Int(1, -1),  // 右下
        new Vector2Int(-1, -1)  // 左下
    };

    public override void Initialize(Vector2Int startPos)
    {
        health = 2; // 设置初始血量为2
        base.Initialize(startPos);
        type = MonsterType.Bishop;
        monsterName = "I05";
        displayName = "灵魂牧者";
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
        bool canReachPlayer = false;

        // 检查是否能直接到达玩家位置
        foreach (Vector2Int direction in bishopDirections)
        {
            Vector2Int potentialPosition = position;

            // 沿着当前方向移动直到遇到障碍或越界
            while (true)
            {
                potentialPosition += direction;

                if (!IsValidPosition(potentialPosition) || IsPositionOccupied(potentialPosition))
                    break;

                if (potentialPosition == targetPos)
                {
                    canReachPlayer = true;
                    break;
                }
            }
            
            if (canReachPlayer) break;
        }

        // 如果能直接到达玩家位置，就移动攻击
        if (canReachPlayer)
        {
            position = targetPos;
            UpdatePosition();
            Debug.Log("I05 directly attacked the player.");
        }
        // 否则召唤爬行者
        else
        {
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

        // 如果有有效位置，随机选择一个召唤爬行者
        if (validSummonPositions.Count > 0)
        {
            Vector2Int summonPos = validSummonPositions[Random.Range(0, validSummonPositions.Count)];
            SummonCrawler(summonPos);
        }
        else
        {
            Debug.Log($"{displayName} tried to summon but no valid positions available");
        }
    }

    private void SummonCrawler(Vector2Int summonPosition)
    {
        // 创建I01爬行者
        MonsterManager monsterManager = FindObjectOfType<MonsterManager>();
        if (monsterManager != null)
        {
            Monster crawler = monsterManager.CreateMonsterByType("I01");
            if (crawler != null)
            {
                crawler.transform.position = player.CalculateWorldPosition(summonPosition);
                crawler.Initialize(summonPosition);
                
                // 将召唤的怪物添加到怪物管理器
                monsterManager.monsters.Add(crawler);
                
                Debug.Log($"{displayName} summoned a Crawler at {summonPosition}");
            }
        }
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/I05");
    }

    public override List<Vector2Int> CalculatePossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        
        foreach (Vector2Int direction in bishopDirections)
        {
            Vector2Int currentPos = position + direction;
            while (IsValidPosition(currentPos) && !IsPositionOccupied(currentPos))
            {
                possibleMoves.Add(currentPos);
                currentPos += direction;
            }
        }
        
        return possibleMoves;
    }

    public override List<string> GetSkills()
    {
        return new List<string> { "号角" };
    }
}