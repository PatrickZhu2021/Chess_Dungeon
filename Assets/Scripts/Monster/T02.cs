using UnityEngine;
using System.Collections.Generic;

public class T02 : Monster
{
    private static List<T02> allT02s = new List<T02>();
    private bool isMovementBlocked = false;

    public override void Initialize(Vector2Int startPos)
    {
        health = 6;
        base.Initialize(startPos);
        type = MonsterType.King;
        monsterName = "T02";
        displayName = "反应体";
        
        allT02s.Add(this);
        
        // 订阅卡牌使用事件
        if (player != null && player.deckManager != null)
        {
            player.deckManager.OnCardPlayed += OnCardPlayed;
        }
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/T02");
    }

    private void OnCardPlayed()
    {
        if (!isMovementBlocked)
        {
            base.MoveTowardsPlayer();
        }
    }

    public override void PerformMovement()
    {
        if (player == null) return;
        
        Vector2Int targetPos = GetTargetPosition();
        Vector2Int direction = new Vector2Int(
            targetPos.x > position.x ? 1 : (targetPos.x < position.x ? -1 : 0),
            targetPos.y > position.y ? 1 : (targetPos.y < position.y ? -1 : 0)
        );
        
        Vector2Int newPosition = position + direction;
        
        if (IsValidPosition(newPosition) && !IsPositionOccupied(newPosition))
        {
            position = newPosition;
            UpdatePosition();
        }
    }

    public void SetMovementBlocked(bool blocked)
    {
        isMovementBlocked = blocked;
        Debug.Log($"T02 {monsterName} movement blocked: {blocked}");
    }
    
    public override void OnTurnEnd()
    {
        // 回合结束时重置移动阻止状态
        isMovementBlocked = false;
    }

    public override List<Vector2Int> CalculatePossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>
        {
            position + new Vector2Int(1, 0),   // 右
            position + new Vector2Int(-1, 0),  // 左
            position + new Vector2Int(0, 1),   // 上
            position + new Vector2Int(0, -1),  // 下
            position + new Vector2Int(1, 1),   // 右上
            position + new Vector2Int(-1, 1),  // 左上
            position + new Vector2Int(1, -1),  // 右下
            position + new Vector2Int(-1, -1)  // 左下
        };

        // 过滤掉无效位置或被占据的位置
        possibleMoves.RemoveAll(pos => !IsValidPosition(pos) || IsPositionOccupied(pos));
        return possibleMoves;
    }

    void OnDestroy()
    {
        allT02s.Remove(this);
        
        // 取消订阅事件
        if (player != null && player.deckManager != null)
        {
            player.deckManager.OnCardPlayed -= OnCardPlayed;
        }
    }
}