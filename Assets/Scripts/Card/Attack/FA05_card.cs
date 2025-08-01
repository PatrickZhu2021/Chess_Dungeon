using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class FA05_card: CardButtonBase
{
    Vector2Int[] crossII_Directions = new Vector2Int[]
    {
        new Vector2Int(-2, 0), new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(2, 0),
        new Vector2Int(0, -2), new Vector2Int(0, -1), new Vector2Int(0, 1), new Vector2Int(0, 2)
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
                int damage = card.GetDamageAmount(); 
                player.damage = damage;
                player.ShowAttackOptions(crossII_Directions, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in FA05_card.OnClick");
        }
    }
}

public class FA05: Card
{
    public FA05() : base(CardType.Attack, "FA05", 1) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/FA05");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/FA05");
    }

    public override string GetDescription()
    {
        return "炽炬-熄灭 燃点，火域 1 十字II级 造成1点伤害；若目标位于地形燃点或火域上，再获得1张【炽炬-点燃】";
    }

    public override int GetDamageAmount()
    {
        return 1;
    }

    public override void OnCardExecuted(Vector2Int attackPos)
    {
        Debug.Log($"FA05 OnCardExecuted called at {attackPos}");
        
        // 检查目标是否位于燃点或火域上
        if (IsTargetOnFirePointOrZone(attackPos))
        {
            // 获得1张炽炬-点燃
            FA05a igniteCard = new FA05a();
            igniteCard.isTemporary = true; // 设置为临时卡牌
            
            DeckManager deckManager = UnityEngine.Object.FindObjectOfType<DeckManager>();
            if (deckManager != null)
            {
                deckManager.DrawSpecificCard(igniteCard);
                Debug.Log("FA05: Added FA05a (炽炬-点燃) to hand");
            }
        }
    }
    
    private bool IsTargetOnFirePointOrZone(Vector2Int targetPos)
    {
        LocationManager locationManager = UnityEngine.Object.FindObjectOfType<LocationManager>();
        if (locationManager == null) return false;
        
        Vector3 worldPos = player.CalculateWorldPosition(targetPos);
        
        // 检查是否在燃点上
        foreach (FirePoint firePoint in locationManager.activeFirePoints)
        {
            if (firePoint != null && firePoint.gridPosition == targetPos)
            {
                Debug.Log($"FA05: Target {targetPos} is on FirePoint");
                return true;
            }
        }
        
        // 检查是否在火域上
        foreach (FireZone fireZone in locationManager.activeFireZones)
        {
            if (fireZone != null)
            {
                // 简化检查：如果有活跃火域就认为可能在火域上
                Debug.Log($"FA05: Target {targetPos} might be in FireZone");
                return true;
            }
        }
        
        return false;
    }
}