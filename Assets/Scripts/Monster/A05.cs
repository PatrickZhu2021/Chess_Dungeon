using UnityEngine;
using System.Collections.Generic;

public class A05 : Monster
{
    // 定义皇后的所有可能移动方向（直线 + 对角线）
    private static readonly Vector2Int[] queenDirections = new Vector2Int[]
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
        monsterName = "A05";
        displayName = "混血龙种";
    }

    public override void PerformMovement()
    {
        if (player == null) return;
        Vector2Int targetPos = GetTargetPosition();

        Vector2Int bestMove = position;
        float closestDistance = Vector2Int.Distance(position, targetPos);
        Vector2Int chosenDirection = Vector2Int.zero;
        
        // 遍历所有可能的皇后移动方向
        foreach (Vector2Int direction in queenDirections)
        {
            Vector2Int potentialPosition = position;

            // 沿着当前方向移动直到越界，翅膀技能允许跨越障碍物
            while (true)
            {
                potentialPosition += direction;

                // 只检查边界，不检查障碍物（翅膀技能）
                if (!IsWithinBounds(potentialPosition))
                    break;

                // 但是不能停在被占据的位置上
                if (IsPositionOccupied(potentialPosition))
                    continue;

                // 不能停在不可进入的位置上（如森林、墙壁等）
                LocationManager locManager = FindObjectOfType<LocationManager>();
                if (locManager != null && locManager.IsNonEnterablePosition(potentialPosition))
                    continue;

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

        // 检测是否接触到目标
        if (position == targetPos)
        {
            lastRelativePosition = -chosenDirection;
        }
    }

    private bool IsWithinBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < player.boardSize && pos.y >= 0 && pos.y < player.boardSize;
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/A05");
    }

    public override List<Vector2Int> CalculatePossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        foreach (Vector2Int direction in queenDirections)
        {
            Vector2Int currentPos = position + direction;
            
            // 翅膀技能：可以跨越障碍物，但不能停在障碍物上
            while (IsWithinBounds(currentPos))
            {
                // 可以经过障碍物，但不能停在被占据或不可进入的位置
                LocationManager locManager = FindObjectOfType<LocationManager>();
                if (!IsPositionOccupied(currentPos) && 
                    (locManager == null || !locManager.IsNonEnterablePosition(currentPos)))
                {
                    possibleMoves.Add(currentPos);
                }
                currentPos += direction;
            }
        }

        return possibleMoves;
    }

    public override List<string> GetSkills()
    {
        return new List<string> { "翅膀" };
    }
}