using UnityEngine;
using System.Collections.Generic;

public class T02 : Monster
{
    private static List<T02> allT02s = new List<T02>();

    public override void Initialize(Vector2Int startPos)
    {
        health = 3;
        base.Initialize(startPos);
        type = MonsterType.Beast;
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
        base.MoveTowardsPlayer();
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