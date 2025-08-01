using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;

public class WS05_card : CardButtonBase
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
                Debug.Log("WS05 card used: activating wave rider effect");
            }
        }
        else
        {
            Debug.LogError("Card is null in WS05_card.OnClick");
        }
    }
}

public class WS05 : Card
{
    public WS05() : base(CardType.Special, "WS05", 1, isExhaust: true) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/WS05");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/WS05");
    }

    public override string GetDescription()
    {
        return "消耗，本次战斗中，打出移动牌后击退位移目标点3x3范围内随机1名敌方单位1次";
    }

    public override void OnCardExecuted()
    {
        base.OnCardExecuted(); // 触发通用效果
        
        Debug.Log("WS05 OnCardExecuted called");
        
        // 激活逐浪效果
        if (player != null)
        {
            player.waveRiderEffectActive = true;
            Debug.Log("WS05: Wave Rider effect activated for the rest of the battle");
        }
    }
}