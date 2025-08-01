using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;

public class WA02_card : CardButtonBase
{
    Vector2Int[] crossDirections = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

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
                player.ShowAttackOptions(crossDirections, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in WA02_card.OnClick");
        }
    }
}

public class WA02 : Card
{
    public WA02() : base(CardType.Attack, "WA02", 10) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/WA02");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/WA02");
    }

    public override string GetDescription()
    {
        return "十字I级，造成1点伤害，并击退目标3x3范围内所有敌方棋子1次";
    }

    public override void OnCardExecuted(Vector2Int attackPos)
    {
        // 获取目标3x3范围内的所有敌方棋子并击退
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                Vector2Int checkPos = attackPos + new Vector2Int(dx, dy);
                Monster monster = KeywordEffects.GetMonsterAtPosition(checkPos);
                if (monster != null)
                {
                    // 计算击退方向：从攻击位置到怪物位置
                    Vector2 direction = (monster.transform.position - player.CalculateWorldPosition(attackPos)).normalized;
                    Vector2 cardinalDirection = KeywordEffects.RoundToCardinal(direction);
                    KeywordEffects.ApplyKnockback(monster, cardinalDirection, player);
                }
            }
        }
    }

    public override int GetDamageAmount()
    {
        return 1;
    }
}