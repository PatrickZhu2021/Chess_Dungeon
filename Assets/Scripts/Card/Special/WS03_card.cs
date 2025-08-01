using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;

public class WS03_card : CardButtonBase
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
                Debug.Log("WS03 card used: gaining 2 torrent stacks");
            }
        }
        else
        {
            Debug.LogError("Card is null in WS03_card.OnClick");
        }
    }
}

public class WS03 : Card
{
    public WS03() : base(CardType.Special, "WS03", 0, isQuick: true) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/WS03");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/WS03");
    }

    public override string GetDescription()
    {
        return "快速，获得2层【涌潮】";
    }

    public override void OnCardExecuted()
    {
        base.OnCardExecuted(); // 触发通用效果
        
        Debug.Log("WS03 OnCardExecuted called");
        
        // 获得2层涌潮
        if (player != null)
        {
            player.torrentStacks += 2;
            Debug.Log($"WS03: Added 2 torrent stacks. Total: {player.torrentStacks}");
        }
    }
}