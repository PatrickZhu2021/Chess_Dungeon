using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;

public class WA01_card : CardButtonBase
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
            Debug.LogError("Card is null in wa01_card.OnClick");
        }
    }
}

public class WA01 : Card
{
    public WA01() : base(CardType.Attack, "WA01", 10) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/WA01");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/WA01");
    }

    public override string GetDescription()
    {
        return "十字I级，造成1点伤害，并抽取1张移动牌";
    }

    public override void OnCardExecuted(Vector2Int attackPos)
    {
        // 抽取1张移动牌
        if (player != null && player.deckManager != null)
        {
            player.deckManager.DrawCardOfType(CardType.Move, 1);
        }
    }

    public override int GetDamageAmount()
    {
        return 1;
    }
}