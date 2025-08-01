using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class BS05_card: CardButtonBase
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
                Debug.Log("BS05 card used: gaining 1 action point");
            }
        }
        else
        {
            Debug.LogError("Card is null in BS05_card.OnClick");
        }
    }
}

public class BS05: Card
{
    public BS05() : base(CardType.Special, "BS05", 0, isQuick: true, isExhaust: true, isGrace: true) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/BS05");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/BS05");
    }

    public override string GetDescription()
    {
        return "战舞  0 / 快速，消耗，恩赐，获得1点行动点";
    }

    public override void OnCardExecuted()
    {
        base.OnCardExecuted(); // 触发恩赐效果
        
        Debug.Log("BS05 OnCardExecuted called");
        
        // 获得1点行动点
        TurnManager turnManager = GameObject.FindObjectOfType<TurnManager>();
        if (turnManager != null)
        {
            turnManager.AddAction();
            Debug.Log("BS05: Added 1 action point");
        }
    }
}