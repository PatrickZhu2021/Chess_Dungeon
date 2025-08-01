using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;

public class BA03_card: CardButtonBase
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
            Debug.LogError("Card is null in BA03_card.OnClick");
        }
    }
}

public class BA03: Card
{
    public BA03() : base(CardType.Attack, "BA03", 10, isExhaust: true) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/BA03");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/BA03");
    }

    public override string GetDescription()
    {
        return "上下左右攻击，造成4点伤害。消耗。";
    }

    public override int GetDamageAmount()
    {
        return 4;
    }

    public override void OnCardExecuted(Vector2Int attackPos)
    {        
    }
}