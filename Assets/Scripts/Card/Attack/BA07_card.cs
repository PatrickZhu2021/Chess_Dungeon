using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;

public class BA07_card: CardButtonBase
{
    Vector2Int[] diagonalDirections = { 
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
                player.ShowAttackOptions(diagonalDirections, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in BA07_card.OnClick");
        }
    }
}

public class BA07: Card
{
    public bool wasDrawnExtra = false; // 记录是否为额外抽取

    public BA07() : base(CardType.Attack, "BA07", 0, isQuick: true, isPartner: true) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/BA07");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/BA07");
    }

    public override string GetDescription()
    {
        return "斜向攻击，造成1点伤害，若本卡牌是被额外抽出的，则额外造成1点伤害。快速，羁绊。";
    }

    public override int GetDamageAmount()
    {
        return wasDrawnExtra ? 2 : 1; // 如果是额外抽取则造成2点伤害
    }

    public override void OnCardExecuted(Vector2Int attackPos)
    {
        // 攻击完成后重置标记
        wasDrawnExtra = false;
    }
}