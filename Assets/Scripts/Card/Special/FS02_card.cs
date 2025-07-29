using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class FS02_card: CardButtonBase
{
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
                // 检查是否存在火域
                if (HasActiveFireZone())
                {
                    player.currentCard = card;
                    player.ExecuteCurrentCard();
                    Debug.Log("FS02 card used: creating corner fire points");
                }
                else
                {
                    Debug.Log("FS02: Cannot use - no active fire zones");
                }
            }
        }
        else
        {
            Debug.LogError("Card is null in FS02_card.OnClick");
        }
    }
    
    private bool HasActiveFireZone()
    {
        LocationManager locationManager = UnityEngine.Object.FindObjectOfType<LocationManager>();
        if (locationManager == null) return false;
        
        return locationManager.activeFireZones.Count > 0;
    }
}

public class FS02: Card
{
    public FS02() : base(CardType.Special, "FS02", 1) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/FS02");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/FS02");
    }

    public override string GetDescription()
    {
        return "焚界降诞 火域 1 仅在存在地形火域时可以打出；在棋盘四角处创造地形燃点，当前火域的持续回合数减少1";
    }

    public override void OnCardExecuted()
    {
        Debug.Log("FS02 OnCardExecuted called");
        
        // 在棋盘四角创造燃点
        CreateCornerFirePoints();
        
        // 减少所有火域的持续回合数
        ReduceFireZoneDuration();
    }
    
    private void CreateCornerFirePoints()
    {
        LocationManager locationManager = UnityEngine.Object.FindObjectOfType<LocationManager>();
        GameObject firePointPrefab = Resources.Load<GameObject>("Prefabs/Location/FirePoint");
        
        if (locationManager == null || firePointPrefab == null)
        {
            Debug.LogError("FS02: LocationManager or FirePoint prefab not found");
            return;
        }
        
        // 棋盘四角位置（假设8x8棋盘）
        Vector2Int[] cornerPositions = new Vector2Int[]
        {
            new Vector2Int(0, 0),     // 左下角
            new Vector2Int(7, 0),     // 右下角
            new Vector2Int(0, 7),     // 左上角
            new Vector2Int(7, 7)      // 右上角
        };
        
        int createdCount = 0;
        foreach (Vector2Int cornerPos in cornerPositions)
        {
            if (player.IsValidPosition(cornerPos))
            {
                locationManager.CreateFirePoint(firePointPrefab, cornerPos);
                Debug.Log($"FS02: Created FirePoint at corner {cornerPos}");
                createdCount++;
            }
        }
        
        Debug.Log($"FS02: Created {createdCount} corner fire points");
    }
    
    private void ReduceFireZoneDuration()
    {
        LocationManager locationManager = UnityEngine.Object.FindObjectOfType<LocationManager>();
        if (locationManager == null)
        {
            Debug.LogError("FS02: LocationManager not found");
            return;
        }
        
        int reducedCount = 0;
        foreach (FireZone fireZone in locationManager.activeFireZones)
        {
            if (fireZone != null)
            {
                fireZone.remainingEnemyTurns = Mathf.Max(0, fireZone.remainingEnemyTurns - 1);
                Debug.Log($"FS02: Reduced fire zone duration to {fireZone.remainingEnemyTurns}");
                reducedCount++;
                
                // 如果持续时间降到0，销毁火域
                if (fireZone.remainingEnemyTurns <= 0)
                {
                    fireZone.DestroySelf();
                    Debug.Log("FS02: Fire zone destroyed due to duration reaching 0");
                }
            }
        }
        
        Debug.Log($"FS02: Reduced duration for {reducedCount} fire zones");
    }
}