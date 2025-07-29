using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class BS06_card: CardButtonBase
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
                Debug.Log("BS06 card used: next weapon card will be used twice");
            }
        }
        else
        {
            Debug.LogError("Card is null in BS06_card.OnClick");
        }
    }
}

public class BS06: Card
{
    public BS06() : base(CardType.Special, "BS06", 1) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/BS06");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/BS06");
    }

    public override string GetDescription()
    {
        return "狮鹫势  1 / 使本回合下1张使用的武器牌在目标处使用2次";
    }

    public override void OnCardExecuted()
    {
        Debug.Log("BS06 OnCardExecuted called");
        
        // 设置下一张武器牌双重使用标记
        if (player != null)
        {
            player.nextWeaponCardDoubleUse = true;
            Debug.Log("BS06: Next weapon card will be used twice");
        }
    }
}