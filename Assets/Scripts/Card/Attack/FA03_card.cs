using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class FA03_card: CardButtonBase
{
    Vector2Int[] crossDirections = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

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
                int damage = card.GetDamageAmount(); 
                player.damage = damage;
                player.ShowAttackOptions(crossDirections, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in FA03_card.OnClick");
        }
    }
}

public class FA03: Card
{
    public FA03() : base(CardType.Attack, "FA03", 1) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/FA03");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/FA03");
    }

    public override string GetDescription()
    {
        return "蛟油蜡烛 燃点 1 十字I级 以目标点为中心，在临近位置随机创造3处地形燃点";
    }

    public override int GetDamageAmount()
    {
        return 0;
    }

    public override void OnCardExecuted(Vector2Int targetPos)
    {
        Debug.Log($"FA03 OnCardExecuted called at {targetPos}");
        
        // 在目标点周围随机创造3处燃点
        CreateRandomFirePoints(targetPos, 3);
    }
    
    private void CreateRandomFirePoints(Vector2Int centerPos, int count)
    {
        LocationManager locationManager = UnityEngine.Object.FindObjectOfType<LocationManager>();
        if (locationManager == null)
        {
            Debug.LogError("FA03: LocationManager not found");
            return;
        }
        
        GameObject firePointPrefab = Resources.Load<GameObject>("Prefabs/Location/FirePoint");
        if (firePointPrefab == null)
        {
            Debug.LogError("FA03: FirePoint prefab not found");
            return;
        }
        
        // 获取目标点周围的所有相邻位置
        List<Vector2Int> adjacentPositions = new List<Vector2Int>
        {
            centerPos + Vector2Int.up,
            centerPos + Vector2Int.down,
            centerPos + Vector2Int.left,
            centerPos + Vector2Int.right,
            centerPos + new Vector2Int(1, 1),   // 右上
            centerPos + new Vector2Int(1, -1),  // 右下
            centerPos + new Vector2Int(-1, 1),  // 左上
            centerPos + new Vector2Int(-1, -1), // 左下
            centerPos // 中心点也可以放置
        };
        
        // 过滤掉无效位置
        List<Vector2Int> validPositions = new List<Vector2Int>();
        foreach (Vector2Int pos in adjacentPositions)
        {
            if (player.IsValidPosition(pos))
            {
                validPositions.Add(pos);
            }
        }
        
        // 随机选择3个位置创造燃点
        int firePointsCreated = 0;
        while (firePointsCreated < count && validPositions.Count > 0)
        {
            int randomIndex = Random.Range(0, validPositions.Count);
            Vector2Int selectedPos = validPositions[randomIndex];
            
            locationManager.CreateFirePoint(firePointPrefab, selectedPos);
            Debug.Log($"FA03: FirePoint created at {selectedPos}");
            
            validPositions.RemoveAt(randomIndex);
            firePointsCreated++;
        }
        
        Debug.Log($"FA03: Created {firePointsCreated} fire points around {centerPos}");
    }
}