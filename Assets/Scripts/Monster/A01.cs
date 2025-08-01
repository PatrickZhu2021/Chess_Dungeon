using UnityEngine;
using System.Collections.Generic;

public class A01 : Monster
{
    public override void Initialize(Vector2Int startPos)
    {
        health = 2;
        base.Initialize(startPos);
        type = MonsterType.Beast;
        monsterName = "A01";
        displayName = "豺犬";
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
        possibleMoves.Sort((a, b) => Vector2Int.Distance(a, targetPos).CompareTo(Vector2Int.Distance(b, targetPos)));

        foreach (Vector2Int move in possibleMoves)
        {
            if (!IsPositionOccupied(move) && IsValidPosition(move))
            {
                position = move;
                UpdatePosition();
                break;
            }
        }
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/A01");
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

        possibleMoves.RemoveAll(pos => !IsValidPosition(pos) || IsPositionOccupied(pos));
        return possibleMoves;
    }
}