using UnityEngine;
using System.Collections.Generic;

public class A02 : Monster
{
    public override void Initialize(Vector2Int startPos)
    {
        health = 2;
        base.Initialize(startPos);
        type = MonsterType.Beast;
        monsterName = "A02";
        displayName = "鳄鱼";
    }

    public override void PerformMovement()
    {
        if (player == null) return;
        lastRelativePosition = position - player.position;
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        // 斜向移动：四个对角方向
        possibleMoves.Add(new Vector2Int(position.x + 1, position.y + 1));
        possibleMoves.Add(new Vector2Int(position.x - 1, position.y + 1));
        possibleMoves.Add(new Vector2Int(position.x + 1, position.y - 1));
        possibleMoves.Add(new Vector2Int(position.x - 1, position.y - 1));

        Vector2Int targetPos = GetTargetPosition();
        possibleMoves.Sort((a, b) => Vector2Int.Distance(a, targetPos).CompareTo(Vector2Int.Distance(b, targetPos)));

        foreach (Vector2Int move in possibleMoves)
        {
            if (!IsPositionOccupied(move) && IsValidPositionForA02(move))
            {
                position = move;
                UpdatePosition();
                break;
            }
        }
    }

    private bool IsValidPositionForA02(Vector2Int pos)
    {
        // 基础位置检查（边界检查）
        if (pos.x < 0 || pos.x >= player.boardSize || pos.y < 0 || pos.y >= player.boardSize)
        {
            return false;
        }
        
        LocationManager locManager = FindObjectOfType<LocationManager>();
        if (locManager == null) return true;
        
        // A02可以跨越水单元格
        if (locManager.IsWaterPosition(pos))
        {
            return true; // 水对A02不是障碍
        }
        
        // 检查其他不可进入位置
        return !locManager.IsNonEnterablePosition(pos);
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/A02");
    }

    public override List<Vector2Int> CalculatePossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>
        {
            position + new Vector2Int(1, 1),
            position + new Vector2Int(-1, 1),
            position + new Vector2Int(1, -1),
            position + new Vector2Int(-1, -1)
        };

        possibleMoves.RemoveAll(pos => !IsValidPositionForA02(pos) || IsPositionOccupied(pos));
        return possibleMoves;
    }

    public override List<string> GetSkills()
    {
        return new List<string> { "水珠" };
    }
}