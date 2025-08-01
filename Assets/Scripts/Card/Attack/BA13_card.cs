using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class BA13_card: CardButtonBase
{
    Vector2Int[] allDirections = { 
        new Vector2Int(1, 0), new Vector2Int(-1, 0), 
        new Vector2Int(0, 1), new Vector2Int(0, -1),
        new Vector2Int(1, 1), new Vector2Int(1, -1), 
        new Vector2Int(-1, 1), new Vector2Int(-1, -1) 
    };

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
                int damage = card.GetDamageAmount(); 
                player.damage = damage;
                
                // 显示全方向II级攻击选项（2格距离）
                List<Vector2Int> extendedDirections = new List<Vector2Int>();
                foreach (Vector2Int direction in allDirections)
                {
                    extendedDirections.Add(direction);
                    extendedDirections.Add(direction * 2); // II级，2格距离
                }
                
                player.ShowAttackOptions(extendedDirections.ToArray(), card);
            }
        }
        else
        {
            Debug.LogError("Card is null in BA13_card.OnClick");
        }
    }
}

public class BA13: Card
{
    public BA13() : base(CardType.Attack, "BA13", 1) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/BA13");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/BA13");
    }

    public override string GetDescription()
    {
        return "战镰 / 1 全方向II级 造成2点伤害。若击杀目标，再获得1点行动点，抽取1张移动牌并将此牌置入手牌区";
    }

    public override int GetDamageAmount()
    {
        return 2;
    }

    public override void OnCardExecuted(Vector2Int attackPos)
    {
        // 检查是否击杀了目标（伤害已经由Player.Attack造成）
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monsterObject in monsters)
        {
            Monster monster = monsterObject.GetComponent<Monster>();
            if (monster != null && monster.IsPartOfMonster(attackPos) && monster.health <= 0)
            {
                Debug.Log("BA13 killed target - triggering kill effects");
                
                // 获得1点行动点
                TurnManager turnManager = GameObject.FindObjectOfType<TurnManager>();
                if (turnManager != null)
                {
                    turnManager.AddAction();
                    Debug.Log("BA13: Added 1 action point");
                }
                
                // 抽取1张移动牌
                DeckManager deckManager = GameObject.FindObjectOfType<DeckManager>();
                if (deckManager != null)
                {
                    DrawMoveCard(deckManager);
                    
                    // 将此牌（BA13）从弃牌堆置入手牌区
                    // 使用MonoBehaviour来延迟调用
                    deckManager.StartCoroutine(ReturnThisCardToHandDelayed(deckManager));
                }
                
                break;
            }
        }
    }
    
    private System.Collections.IEnumerator ReturnThisCardToHandDelayed(DeckManager deckManager)
    {
        yield return new WaitForEndOfFrame(); // 等待当前帧结束，确保卡牌已进入弃牌堆
        ReturnThisCardToHand(deckManager);
    }
    
    private void DrawMoveCard(DeckManager deckManager)
    {
        // 从牌库中找到第一张移动牌
        Card moveCard = null;
        for (int i = 0; i < deckManager.deck.Count; i++)
        {
            if (deckManager.deck[i].cardType == CardType.Move)
            {
                moveCard = deckManager.deck[i];
                deckManager.deck.RemoveAt(i);
                break;
            }
        }
        
        // 如果牌库没有移动牌，从弃牌堆找
        if (moveCard == null)
        {
            for (int i = 0; i < deckManager.discardPile.Count; i++)
            {
                if (deckManager.discardPile[i].cardType == CardType.Move)
                {
                    moveCard = deckManager.discardPile[i];
                    deckManager.discardPile.RemoveAt(i);
                    break;
                }
            }
        }
        
        if (moveCard != null)
        {
            deckManager.DrawSpecificCard(moveCard);
            Debug.Log($"BA13: Drew move card {moveCard.Id}");
        }
        else
        {
            Debug.Log("BA13: No move cards available to draw");
        }
    }
    
    private void ReturnThisCardToHand(DeckManager deckManager)
    {
        // 从弃牌堆中找到这张BA13卡牌
        Card thisCard = null;
        for (int i = 0; i < deckManager.discardPile.Count; i++)
        {
            if (deckManager.discardPile[i].Id == "BA13")
            {
                thisCard = deckManager.discardPile[i];
                deckManager.discardPile.RemoveAt(i);
                break;
            }
        }
        
        if (thisCard != null)
        {
            deckManager.DrawSpecificCard(thisCard);
            Debug.Log("BA13: Returned this card to hand");
        }
    }
}