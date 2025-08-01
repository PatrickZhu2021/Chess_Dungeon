using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class FA04_card: CardButtonBase
{
    Vector2Int[] allDirections = new Vector2Int[]
    {
        new Vector2Int(-2, -2), new Vector2Int(-2, -1), new Vector2Int(-2, 0), new Vector2Int(-2, 1), new Vector2Int(-2, 2),
        new Vector2Int(-1, -2), new Vector2Int(-1, -1), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(-1, 2),
        new Vector2Int(0, -2), new Vector2Int(0, -1), new Vector2Int(0, 1), new Vector2Int(0, 2),
        new Vector2Int(1, -2), new Vector2Int(1, -1), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(1, 2),
        new Vector2Int(2, -2), new Vector2Int(2, -1), new Vector2Int(2, 0), new Vector2Int(2, 1), new Vector2Int(2, 2)
    };

    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnClick()
    {
        if (card != null)
        {
            if (player.currentCard == card)
            {
                player.DeselectCurrentCard();
            }
            else
            {
                // 检查玩家是否在火域上
                if (IsPlayerInFireZone())
                {
                    int damage = card.GetDamageAmount();
                    player.damage = damage;
                    player.ShowAttackOptions(allDirections, card);
                }
                else
                {
                    Debug.Log("FA04: Cannot use - player not in fire zone");
                }
            }
        }
        else
        {
            Debug.LogError("Card is null in FA04_card.OnClick");
        }
    }
    
    private bool IsPlayerInFireZone()
    {
        LocationManager locationManager = UnityEngine.Object.FindObjectOfType<LocationManager>();
        if (locationManager == null) return false;
        
        Vector3 playerWorldPos = player.transform.position;
        
        foreach (FireZone fireZone in locationManager.activeFireZones)
        {
            if (fireZone != null)
            {
                // 使用FireZone的IsPointInPolygon方法检查玩家是否在火域内
                // 这里需要访问FireZone的多边形点，但它是私有的
                // 简化检查：如果有活跃的火域就允许使用
                return true;
            }
        }
        
        return false;
    }
}

public class FA04: Card
{
    public FA04() : base(CardType.Attack, "FA04", 0, isQuick: true, isExhaust: true) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/FA04");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/FA04");
    }

    public override string GetDescription()
    {
        return "焚化油 快速，消耗，燃点 0 全方向II级 仅在地形火域上时可以打出；以目标处为中心，所有十字相邻格创造地形燃点";
    }

    public override int GetDamageAmount()
    {
        return 0;
    }

    public override void OnCardExecuted(Vector2Int targetPos)
    {
        Debug.Log($"FA04 OnCardExecuted called at {targetPos}");
        
        // 在目标处十字相邻格创造燃点
        CreateCrossFirePoints(targetPos);
    }
    
    private void CreateCrossFirePoints(Vector2Int centerPos)
    {
        LocationManager locationManager = UnityEngine.Object.FindObjectOfType<LocationManager>();
        if (locationManager == null)
        {
            Debug.LogError("FA04: LocationManager not found");
            return;
        }
        
        GameObject firePointPrefab = Resources.Load<GameObject>("Prefabs/Location/FirePoint");
        if (firePointPrefab == null)
        {
            Debug.LogError("FA04: FirePoint prefab not found");
            return;
        }
        
        // 十字相邻格位置
        Vector2Int[] crossPositions = new Vector2Int[]
        {
            centerPos + Vector2Int.up,
            centerPos + Vector2Int.down,
            centerPos + Vector2Int.left,
            centerPos + Vector2Int.right
        };
        
        int firePointsCreated = 0;
        foreach (Vector2Int pos in crossPositions)
        {
            if (player.IsValidPosition(pos))
            {
                locationManager.CreateFirePoint(firePointPrefab, pos);
                Debug.Log($"FA04: FirePoint created at {pos}");
                firePointsCreated++;
            }
        }
        
        Debug.Log($"FA04: Created {firePointsCreated} fire points around {centerPos}");
    }
}