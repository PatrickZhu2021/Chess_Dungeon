using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class BS07_card: CardButtonBase
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
                Debug.Log("BS07 card used: next card will return to deck top");
            }
        }
        else
        {
            Debug.LogError("Card is null in BS07_card.OnClick");
        }
    }
}

public class BS07: Card
{
    public BS07() : base(CardType.Special, "BS07", 0, isQuick: true) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/BS07");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/BS07");
    }

    public override string GetDescription()
    {
        return "卷轴匣  0 / 快速，下张被打出的卡牌回到抽牌区顶部";
    }

    public override void OnCardExecuted()
    {
        Debug.Log("BS07 OnCardExecuted called");
        
        // 设置下一张卡牌回到牌库顶部的标记
        if (player != null)
        {
            player.nextCardReturnToDeckTop = true;
            Debug.Log("BS07: Next card will return to deck top");
        }
    }
}