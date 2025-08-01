using UnityEngine;
using System.Collections.Generic;

public class B06 : Monster
{
    private static readonly Vector2Int[] rookDirections = new Vector2Int[]
    {
        new Vector2Int(1, 0), new Vector2Int(-1, 0),
        new Vector2Int(0, 1), new Vector2Int(0, -1)
    };

    public override void Initialize(Vector2Int startPos)
    {
        health = 4;
        base.Initialize(startPos);
        type = MonsterType.Rook;
        monsterName = "B06";
        displayName = "车垒";
    }

    public override void PerformMovement()
    {
        if (player == null) return;

        Vector2Int targetPos = GetTargetPosition();
        Vector2Int bestMove = position;
        float closestDistance = Vector2Int.Distance(position, targetPos);
        Vector2Int chosenDirection = Vector2Int.zero;
        bool foundBetterMove = false;

        foreach (Vector2Int direction in rookDirections)
        {
            Vector2Int potentialPosition = position;

            while (true)
            {
                potentialPosition += direction;

                if (!IsValidPosition(potentialPosition) || IsPositionOccupied(potentialPosition))
                    break;

                float distanceToTarget = Vector2Int.Distance(potentialPosition, targetPos);
                if (distanceToTarget < closestDistance)
                {
                    bestMove = potentialPosition;
                    closestDistance = distanceToTarget;
                    chosenDirection = direction;
                    foundBetterMove = true;
                }
            }
        }

        // 如果没有更好的移动，就移动到第一个可用位置
        if (!foundBetterMove)
        {
            foreach (Vector2Int direction in rookDirections)
            {
                Vector2Int potentialPosition = position + direction;
                if (IsValidPosition(potentialPosition) && !IsPositionOccupied(potentialPosition))
                {
                    bestMove = potentialPosition;
                    chosenDirection = direction;
                    break;
                }
            }
        }

        position = bestMove;
        UpdatePosition();

        if (position == targetPos)
        {
            lastRelativePosition = -chosenDirection;
        }
        else
        {
            lastRelativePosition = position - player.position;
        }
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/B06");
    }

    public override List<Vector2Int> CalculatePossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        foreach (Vector2Int direction in rookDirections)
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
}