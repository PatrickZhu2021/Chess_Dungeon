using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;

public class BA04_card: CardButtonBase
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
            Debug.LogError("Card is null in BA04_card.OnClick");
        }
    }
}

public class BA04: Card
{
    public BA04() : base(CardType.Attack, "BA04", 10, isLingering: true) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/BA04");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/BA04");
    }

    public override string GetDescription()
    {
        return "上下左右攻击，造成1点伤害，并抽取等同于造成伤害量的卡牌。长驻。";
    }

    public override int GetDamageAmount()
    {
        return 1;
    }

    public override void OnCardExecuted(Vector2Int attackPos)
    {   
        // 抽卡效果：抽取等同于造成伤害量的卡牌
        int damageDealt = GetDamageAmount();
        DeckManager deckManager = GameObject.FindObjectOfType<DeckManager>();
        if (deckManager != null)
        {
            deckManager.DrawCards(damageDealt);
        }
    }
}