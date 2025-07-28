using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class BA09_card: CardButtonBase
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
            Debug.LogError("Card is null in BA09_card.OnClick");
        }
    }
}

public class BA09: Card
{
    public BA09() : base(CardType.Attack, "BA09", 1) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/BA09");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/BA09");
    }

    public override string GetDescription()
    {
        return "战斧 愤怒 1 斜向I级 造成1点伤害，获得1层【愤怒】";
    }

    public override int GetDamageAmount()
    {
        return 1;
    }

    public override void OnCardExecuted(Vector2Int attackPos)
    {
        // 获得1层愤怒
        player.AddFury(1);
        // 立即应用愤怒伤害加成
        player.ApplyFuryDamage();
        Debug.Log($"BA09 executed: fury stacks = {player.furyStacks}, damage modifier = {player.damageModifierThisTurn}");
    }
}