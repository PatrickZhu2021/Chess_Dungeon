using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;

public class BA01_card: CardButtonBase
{
    Vector2Int[] swordDirections = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

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
                player.ShowAttackOptions(swordDirections, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in BA01_card.OnClick");
        }
    }
}

public class BA01: Card
{
    public BA01() : base(CardType.Attack, "BA01", 10) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/BA01");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/BA01");
    }

    public override string GetDescription()
    {
        return "上下左右攻击，造成2点伤害";
    }

    public override int GetDamageAmount()
    {
        return 2;
    }

    public override void OnCardExecuted(Vector2Int attackPos)
    {
        KeywordEffects.AttackWithKnockback(player, attackPos);
    }
}