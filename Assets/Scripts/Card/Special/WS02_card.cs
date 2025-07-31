using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;

public class WS02_card : CardButtonBase
{
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
    }

    protected override void Start()
    {
        base.Start();

        if (card != null && card.IsUpgraded())
        {
            Transform glow = transform.Find("UpgradeEffect");
            if (glow != null)
                glow.gameObject.SetActive(true);
        }
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
                Debug.Log("WS02 card used: increasing max action points");
            }
        }
        else
        {
            Debug.LogError("Card is null in WS02_card.OnClick");
        }
    }
}

public class WS02 : Card
{
    public WS02() : base(CardType.Special, "WS02", 1, isExhaust: true) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/WS02");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/WS02");
    }

    public override string GetDescription()
    {
        return "消耗，本次战斗中，行动点上限+1";
    }

    public override void OnCardExecuted()
    {
        base.OnCardExecuted(); // 触发通用效果
        
        Debug.Log("WS02 OnCardExecuted called");
        
        // 永久增加行动点上限
        if (player != null)
        {
            player.IncreaseMaxActions(1);
            Debug.Log("WS02: Increased max action points by 1");
        }
    }
}