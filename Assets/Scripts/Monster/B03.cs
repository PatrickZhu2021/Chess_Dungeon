using UnityEngine;
using System.Collections.Generic;

public class B03 : Monster
{
    public override void Initialize(Vector2Int startPos)
    {
        health = 2; // 设置初始血量为2
        base.Initialize(startPos);
        type = MonsterType.Pawn;
        monsterName = "B03";
        displayName = "盗贼";
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }

    public override void Die()
    {
        // 在死亡时触发金币效果：抓一张牌并获得一点行动点
        if (player != null)
        {
            // 抓一张牌
            DeckManager deckManager = FindObjectOfType<DeckManager>();
            if (deckManager != null)
            {
                deckManager.DrawCards(1);
                Debug.Log($"{displayName} death effect: Drew 1 card");
            }
            
            // 获得一点行动点
            player.actions += 1;
            // if (player.actions > player.maxActions)
            // {
            //     player.actions = player.maxActions; // 不超过最大行动点
            // }
            
            // 更新行动点显示
            TurnManager turnManager = FindObjectOfType<TurnManager>();
            if (turnManager != null)
            {
                turnManager.UpdateActionText();
            }
            
            Debug.Log($"{displayName} death effect: Gained 1 action point, current actions: {player.actions}");
        }
        
        base.Die();
    }

    public override void PerformMovement()
    {
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
            Debug.Log("Target reached by B03.");
        }
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/B03");
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
        return new List<string> { "金币" };
    }
}