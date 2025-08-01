using UnityEngine;
using System.Collections.Generic;

public class T01 : Monster
{
    private static int sharedHealth = 8;
    private static List<T01> allT01s = new List<T01>();
    private bool hasBeenDamagedThisTurn = false;
    private bool isDisappeared = false;
    private Vector2Int disappearedPosition;

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
        if (player == null || isDisappeared) return;
        
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
        if (hasBeenDamagedThisTurn || isDisappeared) return;
        
        // 一回合最多受到一点伤害
        int actualDamage = Mathf.Min(damageAmount, 1);
        
        sharedHealth -= actualDamage;
        if (sharedHealth < 0) sharedHealth = 0;
        
        hasBeenDamagedThisTurn = true;
        
        // 显示伤害数字
        ShowDamageText(actualDamage, false);
        
        UpdateAllT01Health();
        
        if (sharedHealth <= 0)
        {
            DestroyAllT01s();
        }
        else
        {
            // 受到伤害后消失
            DisappearAfterDamage();
        }
    }

    private void UpdateAllT01Health()
    {
        foreach (T01 t01 in allT01s)
        {
            if (t01 != null)
            {
                t01.health = sharedHealth;
                t01.UpdateHealthBar();
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

    private void UpdateHealthBar()
    {
        if (healthFillRenderer != null)
        {
            float healthRatio = (float)health / maxHealth;
            healthFillRenderer.transform.localScale = new Vector3(healthRatio, 1, 1);
        }
    }

    private void DisappearAfterDamage()
    {
        if (isDisappeared) return;
        
        isDisappeared = true;
        disappearedPosition = position;
        
        // 隐藏怪物
        gameObject.SetActive(false);
        
        Debug.Log($"{monsterName} disappeared after taking damage");
    }
    
    public void ReappearAtTurnEnd()
    {
        if (!isDisappeared) return;
        
        isDisappeared = false;
        hasBeenDamagedThisTurn = false;
        
        // 重新出现在原位置
        position = disappearedPosition;
        gameObject.SetActive(true);
        UpdatePosition();
        
        Debug.Log($"{monsterName} reappeared at turn end");
    }
    
    public override void OnTurnEnd()
    {
        ReappearAtTurnEnd();
        hasBeenDamagedThisTurn = false;
    }
    

    
    void OnDestroy()
    {
        allT01s.Remove(this);
    }
}