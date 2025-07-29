using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class FS03_card: CardButtonBase
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
                player.currentCard = card;
                player.ExecuteCurrentCard();
                Debug.Log("FS03 card used: triggering fire zone damage");
            }
        }
        else
        {
            Debug.LogError("Card is null in FS03_card.OnClick");
        }
    }
}

public class FS03: Card
{
    public FS03() : base(CardType.Special, "FS03", 1, isTriumph: true) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/FS03");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/FS03");
    }

    public override string GetDescription()
    {
        return "龙龟之息 凯旋 1 凯旋，触发1次火域伤害";
    }

    public override void OnCardExecuted()
    {
        Debug.Log("FS03 OnCardExecuted called");
        
        // 触发1次火域伤害
        TriggerFireZoneDamage();
    }
    
    private void TriggerFireZoneDamage()
    {
        LocationManager locationManager = UnityEngine.Object.FindObjectOfType<LocationManager>();
        if (locationManager == null)
        {
            Debug.LogError("FS03: LocationManager not found");
            return;
        }
        
        int triggeredCount = 0;
        foreach (FireZone fireZone in locationManager.activeFireZones)
        {
            if (fireZone != null)
            {
                fireZone.TriggerDamageOnly();
                Debug.Log("FS03: Triggered fire zone damage");
                triggeredCount++;
            }
        }
        
        if (triggeredCount > 0)
        {
            Debug.Log($"FS03: Triggered damage for {triggeredCount} fire zones");
        }
        else
        {
            Debug.Log("FS03: No active fire zones to trigger");
        }
    }
}