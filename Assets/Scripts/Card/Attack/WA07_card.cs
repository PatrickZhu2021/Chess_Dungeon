using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;

public class WA07_card : CardButtonBase
{
    Vector2Int[] allDirections = { 
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
        Vector2Int.up + Vector2Int.left, Vector2Int.up + Vector2Int.right,
        Vector2Int.down + Vector2Int.left, Vector2Int.down + Vector2Int.right
    };

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
                int damage = card.GetDamageAmount();
                player.damage = damage;
                player.ShowAttackOptions(allDirections, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in WA07_card.OnClick");
        }
    }
}

public class WA07 : Card
{
    private bool isListening = false;
    
    public WA07() : base(CardType.Attack, "WA07", 0) 
    { 
        isQuick = true;
        // 在构造函数中开始监听
        StartListening();
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/WA07");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/WA07");
    }

    public override string GetDescription()
    {
        return "全方向I级，快速，造成1点伤害。使用移动牌时将本牌置入手牌区";
    }

    public override void OnCardExecuted(Vector2Int attackPos)
    {
        base.OnCardExecuted(); // 触发通用效果
    }
    
    private void StartListening()
    {
        if (!isListening && player != null)
        {
            player.OnMoveCardUsed += OnMoveCardUsed;
            isListening = true;
            Debug.Log("WA07: Started listening for move card usage");
        }
    }
    
    private void StopListening()
    {
        if (isListening && player != null)
        {
            player.OnMoveCardUsed -= OnMoveCardUsed;
            isListening = false;
            Debug.Log("WA07: Stopped listening for move card usage");
        }
    }
    
    private void OnMoveCardUsed(Card moveCard)
    {
        // 将本牌从弃牌堆或牌库置入手牌区
        DeckManager deckManager = GameObject.FindObjectOfType<DeckManager>();
        if (deckManager != null)
        {
            // 先从弃牌堆寻找
            for (int i = 0; i < deckManager.discardPile.Count; i++)
            {
                if (deckManager.discardPile[i].Id == this.Id)
                {
                    Card foundCard = deckManager.discardPile[i];
                    deckManager.discardPile.RemoveAt(i);
                    deckManager.DrawSpecificCard(foundCard);
                    Debug.Log("WA07: Returned to hand from discard pile");
                    return;
                }
            }
            
            // 如果弃牌堆没有，从牌库寻找
            for (int i = 0; i < deckManager.deck.Count; i++)
            {
                if (deckManager.deck[i].Id == this.Id)
                {
                    Card foundCard = deckManager.deck[i];
                    deckManager.deck.RemoveAt(i);
                    deckManager.DrawSpecificCard(foundCard);
                    Debug.Log("WA07: Returned to hand from deck");
                    return;
                }
            }
        }
    }

    public override int GetDamageAmount()
    {
        return 1;
    }
}