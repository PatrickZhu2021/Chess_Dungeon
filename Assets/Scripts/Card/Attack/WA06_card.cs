using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;

public class WA06_card : CardButtonBase
{
    Vector2Int[] allDirections = { 
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
        Vector2Int.up + Vector2Int.left, Vector2Int.up + Vector2Int.right,
        Vector2Int.down + Vector2Int.left, Vector2Int.down + Vector2Int.right
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
                player.ShowAttackOptions(allDirections, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in WA06_card.OnClick");
        }
    }
}

public class WA06 : Card
{
    public WA06() : base(CardType.Attack, "WA06", 0) 
    { 
        isQuick = true;
        isGrace = true;
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/WA06");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/WA06");
    }

    public override string GetDescription()
    {
        return "全方向I级，快速，恩赐，造成1点伤害";
    }

    public override int GetDamageAmount()
    {
        return 1;
    }
    
    public override void OnCardExecuted(Vector2Int attackPos)
    {
        base.OnCardExecuted(); // 触发恩赐效果
    }
}