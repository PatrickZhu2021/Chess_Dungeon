using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class WS04_card : CardButtonBase
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
                Debug.Log("WS04 card used: removing non-move cards");
            }
        }
        else
        {
            Debug.LogError("Card is null in WS04_card.OnClick");
        }
    }
}

public class WS04 : Card
{
    public WS04() : base(CardType.Special, "WS04", 1) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/WS04");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/WS04");
    }

    public override string GetDescription()
    {
        return "移除手牌区中所有非移动牌，获得X层【涌潮】并抽取X张移动牌，X为移除数";
    }

    public override void OnCardExecuted()
    {
        base.OnCardExecuted(); // 触发通用效果
        
        Debug.Log("WS04 OnCardExecuted called");
        
        DeckManager deckManager = UnityEngine.Object.FindObjectOfType<DeckManager>();
        if (deckManager != null && player != null)
        {
            int removedCount = DiscardNonMoveCards(deckManager);
            
            // 获得X层涌潮
            player.torrentStacks += removedCount;
            
            // 抽取X张移动牌
            DrawMoveCards(deckManager, removedCount);
            
            Debug.Log($"WS04: Removed {removedCount} non-move cards, gained {removedCount} torrent stacks, drew {removedCount} move cards");
        }
    }
    
    private int DiscardNonMoveCards(DeckManager deckManager)
    {
        List<Card> nonMoveCards = new List<Card>();
        
        // 找出手牌中所有非移动牌
        for (int i = deckManager.hand.Count - 1; i >= 0; i--)
        {
            if (deckManager.hand[i].cardType != CardType.Move)
            {
                nonMoveCards.Add(deckManager.hand[i]);
                deckManager.discardPile.Add(deckManager.hand[i]);
                deckManager.hand.RemoveAt(i);
            }
        }
        
        // 更新手牌显示
        deckManager.UpdateHandDisplay();
        
        return nonMoveCards.Count;
    }
    
    private void DrawMoveCards(DeckManager deckManager, int count)
    {
        for (int i = 0; i < count; i++)
        {
            // 从牌库中寻找移动牌
            Card moveCard = null;
            for (int j = 0; j < deckManager.deck.Count; j++)
            {
                if (deckManager.deck[j].cardType == CardType.Move)
                {
                    moveCard = deckManager.deck[j];
                    deckManager.deck.RemoveAt(j);
                    break;
                }
            }
            
            if (moveCard != null)
            {
                deckManager.DrawSpecificCard(moveCard);
            }
            else
            {
                // 如果牌库没有移动牌，从弃牌堆寻找
                for (int j = 0; j < deckManager.discardPile.Count; j++)
                {
                    if (deckManager.discardPile[j].cardType == CardType.Move)
                    {
                        moveCard = deckManager.discardPile[j];
                        deckManager.discardPile.RemoveAt(j);
                        deckManager.DrawSpecificCard(moveCard);
                        break;
                    }
                }
            }
        }
    }
}