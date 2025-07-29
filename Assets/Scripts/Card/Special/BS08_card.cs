using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class BS08_card: CardButtonBase
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
                Debug.Log("BS08 card used: will gain armor for each enemy death");
            }
        }
        else
        {
            Debug.LogError("Card is null in BS08_card.OnClick");
        }
    }
}

public class BS08: Card
{
    public BS08() : base(CardType.Special, "BS08", 1, isExhaust: true) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/BS08");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/BS08");
    }

    public override string GetDescription()
    {
        return "盐袋  1 / 消耗，本次战斗内，每个死亡的敌方棋子使你获得1点虚血。";
    }

    public override void OnCardExecuted()
    {
        Debug.Log("BS08 OnCardExecuted called");
        KeywordEffects.ActivateSaltBag(player);
    }
}