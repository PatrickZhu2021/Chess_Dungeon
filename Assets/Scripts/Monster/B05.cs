using UnityEngine;
using System.Collections.Generic;

public class B05 : Monster
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
        health = 3;
        base.Initialize(startPos);
        type = MonsterType.Knight;
        monsterName = "B05";
        displayName = "逃兵";
    }

    public override void PerformMovement()
    {
        if (player == null) return;
        Vector2Int targetPos = GetTargetPosition();
        Vector2Int oldPos = position;
        List<Vector2Int> validMoves = new List<Vector2Int>();

        foreach (Vector2Int move in knightMoves)
        {
            Vector2Int potentialPosition = position + move;
            if (IsValidPosition(potentialPosition) && !IsPositionOccupied(potentialPosition))
            {
                validMoves.Add(potentialPosition);
            }
        }

        Vector2Int bestMove = position;
        if (validMoves.Count > 0)
        {
            // 找到最接近目标的位置
            float closestDistance = float.MaxValue;
            foreach (Vector2Int move in validMoves)
            {
                float distanceToTarget = Vector2Int.Distance(move, targetPos);
                if (distanceToTarget < closestDistance)
                {
                    bestMove = move;
                    closestDistance = distanceToTarget;
                }
            }
        }

        // 强制移动：只要有有效移动就一定要移动
        if (bestMove != position)
        {
            Vector2Int knightMove = bestMove - oldPos;
            position = bestMove;
            UpdatePosition();

            if (position == targetPos)
            {
                lastRelativePosition = ComputeKnightPushDirection(knightMove);
            }
            else
            {
                lastRelativePosition = position - player.position;
            }
        }
    }

    private Vector2Int ComputeKnightPushDirection(Vector2Int knightMove)
    {
        if (Mathf.Abs(knightMove.y) > Mathf.Abs(knightMove.x))
        {
            return new Vector2Int(0, (int)Mathf.Sign(-knightMove.y));
        }
        else
        {
            return new Vector2Int((int)Mathf.Sign(-knightMove.x), 0);
        }
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/B05");
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
}