using UnityEngine;
using System.Collections.Generic;

public class WhiteBishop : Monster
{
    // 定义四个对角线方向
    private static readonly Vector2Int[] bishopDirections = new Vector2Int[]
    {
        new Vector2Int(1, 1), new Vector2Int(1, -1),
        new Vector2Int(-1, 1), new Vector2Int(-1, -1)
    };

    public override void Initialize(Vector2Int startPos)
    {
        base.Initialize(startPos);
        monsterName = "WhiteBishop";
        type = MonsterType.Bishop;
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

        Vector2Int bestMove = position;
        float closestDistance = Vector2Int.Distance(position, targetPos);
        Vector2Int chosenDirection = Vector2Int.zero;
        // 遍历所有可能的对角线方向
        foreach (Vector2Int direction in bishopDirections)
        {
            Vector2Int potentialPosition = position;

            // 沿着当前方向移动直到遇到障碍或越界
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

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/white_bishop");
    }

    public override List<Vector2Int> CalculatePossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 1),   // 右上
            new Vector2Int(-1, 1),  // 左上
            new Vector2Int(1, -1),  // 右下
            new Vector2Int(-1, -1)  // 左下
        };

        foreach (Vector2Int direction in directions)
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
