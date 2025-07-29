using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class FA08_card: CardButtonBase
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
            Debug.LogError("Card is null in FA08_card.OnClick");
        }
    }
}

public class FA08: Card
{
    public FA08() : base(CardType.Attack, "FA08", 1) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/FA08");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/FA08");
    }

    public override string GetDescription()
    {
        return "焰形剑 火域 1 十字I级 造成2点伤害；丢弃手牌区内所有卡牌，并触发火域伤害X次，X为丢弃数";
    }

    public override int GetDamageAmount()
    {
        return 2;
    }

    public override void OnCardExecuted(Vector2Int attackPos)
    {
        Debug.Log($"FA08 OnCardExecuted called at {attackPos}");
        
        // 丢弃手牌区内所有卡牌
        DeckManager deckManager = UnityEngine.Object.FindObjectOfType<DeckManager>();
        if (deckManager != null)
        {
            int discardedCount = deckManager.DiscardAllCards();
            Debug.Log($"FA08: Discarded {discardedCount} cards");
            
            // 触发火域伤害X次
            if (discardedCount > 0)
            {
                TriggerFireZoneDamage(discardedCount);
            }
        }
    }
    
    private void TriggerFireZoneDamage(int times)
    {
        LocationManager locationManager = UnityEngine.Object.FindObjectOfType<LocationManager>();
        if (locationManager == null)
        {
            Debug.LogError("FA08: LocationManager not found");
            return;
        }
        
        for (int i = 0; i < times; i++)
        {
            foreach (FireZone fireZone in locationManager.activeFireZones)
            {
                if (fireZone != null)
                {
                    fireZone.TriggerDamageOnly();
                    Debug.Log($"FA08: Triggered fire zone damage {i + 1}/{times}");
                }
            }
        }
        
        Debug.Log($"FA08: Triggered fire zone damage {times} times");
    }
}