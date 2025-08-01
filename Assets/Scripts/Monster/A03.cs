using UnityEngine;
using System.Collections.Generic;

public class A03 : Monster
{
    public override void Initialize(Vector2Int startPos)
    {
        health = 2;
        base.Initialize(startPos);
        type = MonsterType.Beast;
        monsterName = "A03";
        displayName = "蜥蜴";
    }

    public override void PerformMovement()
    {
        if (player == null) return;
        
        Vector2Int targetPos = GetTargetPosition();
        List<Vector2Int> possibleMoves = CalculatePossibleMoves();
        
        // 按照接近目标的优先级排序
        possibleMoves.Sort((a, b) => Vector2Int.Distance(a, targetPos).CompareTo(Vector2Int.Distance(b, targetPos)));
        
        // 选择最接近目标的有效位置
        foreach (Vector2Int move in possibleMoves)
        {
            Vector2Int oldPos = position;
            position = move;
            lastRelativePosition = oldPos - player.position;
            UpdatePosition();
            Debug.Log($"A03 moved from {oldPos} to {position}");
            break;
        }
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/A03");
    }

    public override List<Vector2Int> CalculatePossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        Vector2Int currentPos = position;

        // 第一次移动的可能位置（斜向移动）
        List<Vector2Int> firstMoves = new List<Vector2Int>
        {
            currentPos + new Vector2Int(1, 1),   // 右上
            currentPos + new Vector2Int(-1, 1),  // 左上
            currentPos + new Vector2Int(1, -1),  // 右下
            currentPos + new Vector2Int(-1, -1)  // 左下
        };

        // 过滤第一次移动的有效位置
        firstMoves.RemoveAll(pos => !IsValidPosition(pos) || IsPositionOccupied(pos));

        // 对于每个第一次移动的位置，计算第二次移动的可能位置
        foreach (Vector2Int firstMove in firstMoves)
        {
            possibleMoves.Add(firstMove); // 添加第一次移动的位置

            // 从第一次移动位置计算第二次移动（继续斜向移动）
            List<Vector2Int> secondMoves = new List<Vector2Int>
            {
                firstMove + new Vector2Int(1, 1),   // 右上
                firstMove + new Vector2Int(-1, 1),  // 左上
                firstMove + new Vector2Int(1, -1),  // 右下
                firstMove + new Vector2Int(-1, -1)  // 左下
            };

            // 过滤第二次移动的有效位置
            foreach (Vector2Int secondMove in secondMoves)
            {
                if (IsValidPosition(secondMove) && !IsPositionOccupied(secondMove) && secondMove != position)
                {
                    possibleMoves.Add(secondMove);
                }
            }
        }

        // 去除重复位置
        possibleMoves = new List<Vector2Int>(new HashSet<Vector2Int>(possibleMoves));
        return possibleMoves;
    }

    public override List<string> GetSkills()
    {
        return new List<string> { "大心脏" };
    }
}