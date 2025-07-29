using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class BS09_card: CardButtonBase
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
                Debug.Log("BS09 card used: gained 1 action point");
            }
        }
        else
        {
            Debug.LogError("Card is null in BS09_card.OnClick");
        }
    }
}

public class BS09: Card
{
    public BS09() : base(CardType.Special, "BS09", 0, isQuick: true, isExhaust: true) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/BS09");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/BS09");
    }

    public override string GetDescription()
    {
        return "某种草药  0 / 快速，消耗，获得1点行动点，当你失去所有行动点时，将这张牌从牌库或弃牌堆置入你的手牌区";
    }

    public override void OnCardExecuted()
    {
        Debug.Log("BS09 OnCardExecuted called");
        
        // 获得1点行动点
        TurnManager turnManager = GameObject.FindObjectOfType<TurnManager>();
        if (turnManager != null)
        {
            turnManager.AddAction();
            Debug.Log("BS09: Added 1 action point");
        }
    }
}