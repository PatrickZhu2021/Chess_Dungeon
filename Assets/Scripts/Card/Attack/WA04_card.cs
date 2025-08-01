using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;

public class WA04_card : CardButtonBase
{
    Vector2Int[] crossDirections = { 
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
        Vector2Int.up + Vector2Int.left, Vector2Int.up + Vector2Int.right,
        Vector2Int.down + Vector2Int.left, Vector2Int.down + Vector2Int.right,
        Vector2Int.up * 2, Vector2Int.down * 2, Vector2Int.left * 2, Vector2Int.right * 2
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
                player.ShowAttackOptions(crossDirections, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in WA04_card.OnClick");
        }
    }
}

public class WA04 : Card
{
    public WA04() : base(CardType.Attack, "WA04", 10) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/WA04");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/WA04");
    }

    public override string GetDescription()
    {
        int currentDamage = GetDamageAmount();
        return $"十字III级，造成{currentDamage}点伤害，X为本回合移动次数+1";
    }

    public override int GetDamageAmount()
    {
        if (player != null)
        {
            return player.movesThisTurn + 1;
        }
        return 1; // 默认伤害
    }
}