using UnityEngine;
using System.Collections.Generic;

public class B07 : Monster
{
    private bool flagEffectApplied = false;

    public override void Initialize(Vector2Int startPos)
    {
        health = 1;
        base.Initialize(startPos);
        type = MonsterType.Pawn;
        monsterName = "B07";
        displayName = "队长";
        
        // 应用军旗效果
        ApplyFlagEffect();
    }

    private void ApplyFlagEffect()
    {
        if (flagEffectApplied) return;
        
        // 延迟应用效果，确保所有怪物都已初始化
        Invoke("DelayedApplyFlagEffect", 0.1f);
        flagEffectApplied = true;
    }
    
    private void DelayedApplyFlagEffect()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monsterObj in monsters)
        {
            Monster monster = monsterObj.GetComponent<Monster>();
            if (monster != null && monster != this && IsBandit(monster.monsterName))
            {
                monster.health += 2;
                monster.maxHealth += 2;
                Debug.Log($"Flag effect: {monster.displayName} gained 2 health (now {monster.health}/{monster.maxHealth})");
            }
        }
    }

    public override void Die()
    {
        // 死亡时移除军旗效果
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monsterObj in monsters)
        {
            Monster monster = monsterObj.GetComponent<Monster>();
            if (monster != null && monster != this && IsBandit(monster.monsterName))
            {
                monster.TakeDamage(2);
                Debug.Log($"Flag removed: {monster.displayName} lost 2 health");
            }
        }
        
        base.Die();
    }

    public override void PerformMovement()
    {
        if (player == null) return;
        
        Vector2Int targetPos = GetTargetPosition();
        float currentDistance = Vector2Int.Distance(position, targetPos);
        Vector2Int bestMove = position;
        float bestDistance = currentDistance;

        // 检查所有可能的移动方向
        Vector2Int[] directions = {
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(0, 1), new Vector2Int(0, -1)
        };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int move = position + direction;
            if (!IsPositionOccupied(move) && IsValidPosition(move))
            {
                float distance = Vector2Int.Distance(move, targetPos);
                if (distance > bestDistance)
                {
                    bestMove = move;
                    bestDistance = distance;
                }
            }
        }

        // 只有找到更远的位置才移动
        if (bestMove != position)
        {
            position = bestMove;
            lastRelativePosition = position - player.position;
            UpdatePosition();
            Debug.Log($"B07 moved away from player to {position}");
        }
        else
        {
            Debug.Log($"B07 cannot move further away, staying at {position}");
        }
    }

    private bool IsBandit(string monsterName)
    {
        return monsterName == "B01" || monsterName == "B02" || monsterName == "B03" || 
               monsterName == "B04" || monsterName == "B05" || monsterName == "B06" || 
               monsterName == "B07";
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/B07");
    }

    public override List<Vector2Int> CalculatePossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>
        {
            position + new Vector2Int(1, 0),
            position + new Vector2Int(-1, 0),
            position + new Vector2Int(0, 1),
            position + new Vector2Int(0, -1)
        };

        possibleMoves.RemoveAll(pos => !IsValidPosition(pos) || IsPositionOccupied(pos));
        return possibleMoves;
    }

    public override List<string> GetSkills()
    {
        return new List<string> { "军旗" };
    }
}