using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class BS02_card: CardButtonBase
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
                Debug.Log("BS02 card used: drawing cards until move card found");
            }
        }
        else
        {
            Debug.LogError("Card is null in BS02_card.OnClick");
        }
    }
}

public class BS02: Card
{
    public BS02() : base(CardType.Special, "BS02", 0, isQuick: true, isExhaust: true) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/BS02");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/BS02");
    }

    public override string GetDescription()
    {
        return "罗盘 消耗，快速 0 / 消耗，快速；持续抽牌直至抽取到移动牌。";
    }

    public override void OnCardExecuted()
    {
        Debug.Log("BS02 OnCardExecuted called");
        
        DeckManager deckManager = GameObject.FindObjectOfType<DeckManager>();
        if (deckManager == null)
        {
            Debug.LogError("BS02: DeckManager not found");
            return;
        }
        
        // 使用协程逐张抽牌
        deckManager.StartCoroutine(DrawCardsUntilMoveCard(deckManager));
    }
    
    private System.Collections.IEnumerator DrawCardsUntilMoveCard(DeckManager deckManager)
    {
        int cardsDrawn = 0;
        bool foundMoveCard = false;
        
        // 持续抽牌直到抽到移动牌
        while (!foundMoveCard && (deckManager.deck.Count > 0 || deckManager.discardPile.Count > 0))
        {
            // 如果牌库空了，重新洗牌
            if (deckManager.deck.Count == 0 && deckManager.discardPile.Count > 0)
            {
                // 直接调用公开的方法或手动洗牌
                foreach (Card card in deckManager.discardPile)
                {
                    deckManager.deck.Add(card);
                }
                deckManager.discardPile.Clear();
                // 洗牌
                for (int i = 0; i < deckManager.deck.Count; i++)
                {
                    Card temp = deckManager.deck[i];
                    int randomIndex = UnityEngine.Random.Range(i, deckManager.deck.Count);
                    deckManager.deck[i] = deckManager.deck[randomIndex];
                    deckManager.deck[randomIndex] = temp;
                }
            }
            
            if (deckManager.deck.Count > 0)
            {
                Card drawnCard = deckManager.deck[0];
                deckManager.deck.RemoveAt(0);
                deckManager.DrawSpecificCard(drawnCard);
                cardsDrawn++;
                
                Debug.Log($"BS02: Drew card {drawnCard.Id} ({drawnCard.cardType})");
                
                // 检查是否是移动牌
                if (drawnCard.cardType == CardType.Move)
                {
                    foundMoveCard = true;
                    Debug.Log($"BS02: Found move card {drawnCard.Id}, stopping draw");
                }
                
                // 等待一帧，让抽牌动画播放
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                Debug.Log("BS02: No more cards available to draw");
                break;
            }
            
            // 安全检查，防止无限循环
            if (cardsDrawn >= 20)
            {
                Debug.LogWarning("BS02: Drew 20 cards without finding move card, stopping");
                break;
            }
        }
        
        Debug.Log($"BS02: Total cards drawn: {cardsDrawn}, Move card found: {foundMoveCard}");
    }
}