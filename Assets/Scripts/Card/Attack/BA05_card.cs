using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class BA05_card: CardButtonBase
{
    // 十字无限攻击位置（直到棋盘边界）
    Vector2Int[] spearDirections = 
    { 
        // 上方向
        new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(0, 3), new Vector2Int(0, 4), 
        new Vector2Int(0, 5), new Vector2Int(0, 6), new Vector2Int(0, 7),
        // 下方向
        new Vector2Int(0, -1), new Vector2Int(0, -2), new Vector2Int(0, -3), new Vector2Int(0, -4),
        new Vector2Int(0, -5), new Vector2Int(0, -6), new Vector2Int(0, -7),
        // 右方向
        new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0), new Vector2Int(4, 0),
        new Vector2Int(5, 0), new Vector2Int(6, 0), new Vector2Int(7, 0),
        // 左方向
        new Vector2Int(-1, 0), new Vector2Int(-2, 0), new Vector2Int(-3, 0), new Vector2Int(-4, 0),
        new Vector2Int(-5, 0), new Vector2Int(-6, 0), new Vector2Int(-7, 0)
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
                player.ShowAttackOptions(spearDirections, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in BA05_card.OnClick");
        }
    }
}

public class BA05: Card
{
    public BA05() : base(CardType.Attack, "BA05", 0, isQuick: true, isExhaust: true, isForethought: true) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/BA05");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/BA05");
    }

    public override string GetDescription()
    {
        return "十字无限攻击，造成2点伤害。谋定，消耗，快速。";
    }

    public override int GetDamageAmount()
    {
        return 2;
    }

    public override void OnCardExecuted(Vector2Int attackPos)
    {   
        // // 消耗效果：将卡牌移到消耗堆
        // DeckManager deckManager = GameObject.FindObjectOfType<DeckManager>();
        // if (deckManager != null)
        // {
        //     deckManager.ExhaustCard(this);
        // }
    }
}