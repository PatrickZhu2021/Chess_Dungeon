using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;

public class WA03_card : CardButtonBase
{
    Vector2Int[] crossDirections = { 
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
                player.ShowAttackOptions(crossDirections, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in WA03_card.OnClick");
        }
    }
}

public class WA03 : Card
{
    public WA03() : base(CardType.Attack, "WA03", 10) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/WA03");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/WA03");
    }

    public override string GetDescription()
    {
        return "十字II级，造成2点伤害，并在目标处创造地形锚点";
    }

    public override void OnCardExecuted(Vector2Int attackPos)
    {
        // 在目标位置创建锚点
        LocationManager locationManager = GameObject.FindObjectOfType<LocationManager>();
        if (locationManager != null)
        {
            locationManager.CreateAnchor(attackPos);
        }
    }

    public override int GetDamageAmount()
    {
        return 2;
    }
}