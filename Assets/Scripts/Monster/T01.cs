using UnityEngine;
using System.Collections.Generic;

public class T01 : Monster
{
    private static int sharedHealth = 8;
    private static List<T01> allT01s = new List<T01>();

    public override void Initialize(Vector2Int startPos)
    {
        health = sharedHealth;
        base.Initialize(startPos);
        type = MonsterType.Beast;
        monsterName = "T01";
        displayName = "共生体";
        
        allT01s.Add(this);
        UpdateAllT01Health();
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/T01");
    }

    public override void PerformMovement()
    {
        if (player == null) return;
        
        Vector2Int playerPosition = player.position;
        LocationManager locationManager = FindObjectOfType<LocationManager>();
        
        // 尝试水平移动（最多2格）
        if (playerPosition.x != position.x)
        {
            int direction = playerPosition.x > position.x ? 1 : -1;
            Vector2Int bestPos = position;
            for (int i = 1; i <= 2; i++)
            {
                Vector2Int targetPos = new Vector2Int(position.x + direction * i, position.y);
                if (IsValidPositionForT01(targetPos) && !IsPositionOccupied(targetPos))
                {
                    bestPos = targetPos;
                }
                else
                {
                    break;
                }
            }
            if (bestPos != position)
            {
                position = bestPos;
                UpdatePosition();
                return;
            }
        }
        
        // 尝试垂直移动（最多2格）
        if (playerPosition.y != position.y)
        {
            int direction = playerPosition.y > position.y ? 1 : -1;
            Vector2Int bestPos = position;
            for (int i = 1; i <= 2; i++)
            {
                Vector2Int targetPos = new Vector2Int(position.x, position.y + direction * i);
                if (IsValidPositionForT01(targetPos) && !IsPositionOccupied(targetPos))
                {
                    bestPos = targetPos;
                }
                else
                {
                    break;
                }
            }
            if (bestPos != position)
            {
                position = bestPos;
                UpdatePosition();
                return;
            }
        }
    }

    public override void TakeDamage(int damageAmount)
    {
        sharedHealth -= damageAmount;
        if (sharedHealth < 0) sharedHealth = 0;
        
        UpdateAllT01Health();
        
        if (sharedHealth <= 0)
        {
            DestroyAllT01s();
        }
    }

    private void UpdateAllT01Health()
    {
        foreach (T01 t01 in allT01s)
        {
            if (t01 != null)
            {
                t01.health = sharedHealth;
            }
        }
    }

    private void DestroyAllT01s()
    {
        for (int i = allT01s.Count - 1; i >= 0; i--)
        {
            if (allT01s[i] != null)
            {
                allT01s[i].Die();
            }
        }
        allT01s.Clear();
        sharedHealth = 8;
    }

    private bool IsValidPositionForT01(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= player.boardSize || pos.y < 0 || pos.y >= player.boardSize)
        {
            return false;
        }
        
        LocationManager locManager = FindObjectOfType<LocationManager>();
        if (locManager == null) return true;
        
        if (locManager.IsWaterPosition(pos))
        {
            return true;
        }
        
        return !locManager.IsNonEnterablePosition(pos);
    }

    public override List<Vector2Int> CalculatePossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        
        // 水平移动（最多2格）
        for (int i = 1; i <= 2; i++)
        {
            possibleMoves.Add(new Vector2Int(position.x + i, position.y));
            possibleMoves.Add(new Vector2Int(position.x - i, position.y));
        }
        
        // 垂直移动（最多2格）
        for (int i = 1; i <= 2; i++)
        {
            possibleMoves.Add(new Vector2Int(position.x, position.y + i));
            possibleMoves.Add(new Vector2Int(position.x, position.y - i));
        }
        
        possibleMoves.RemoveAll(pos => !IsValidPositionForT01(pos) || IsPositionOccupied(pos));
        return possibleMoves;
    }

    void OnDestroy()
    {
        allT01s.Remove(this);
    }
}