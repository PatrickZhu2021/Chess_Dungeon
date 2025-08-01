using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;

public class FA02_card: CardButtonBase
{
    Vector2Int[] allBoardDirections = new Vector2Int[]
    {
        new Vector2Int(-7, -7), new Vector2Int(-7, -6), new Vector2Int(-7, -5), new Vector2Int(-7, -4), new Vector2Int(-7, -3), new Vector2Int(-7, -2), new Vector2Int(-7, -1), new Vector2Int(-7, 0), new Vector2Int(-7, 1), new Vector2Int(-7, 2), new Vector2Int(-7, 3), new Vector2Int(-7, 4), new Vector2Int(-7, 5), new Vector2Int(-7, 6), new Vector2Int(-7, 7),
        new Vector2Int(-6, -7), new Vector2Int(-6, -6), new Vector2Int(-6, -5), new Vector2Int(-6, -4), new Vector2Int(-6, -3), new Vector2Int(-6, -2), new Vector2Int(-6, -1), new Vector2Int(-6, 0), new Vector2Int(-6, 1), new Vector2Int(-6, 2), new Vector2Int(-6, 3), new Vector2Int(-6, 4), new Vector2Int(-6, 5), new Vector2Int(-6, 6), new Vector2Int(-6, 7),
        new Vector2Int(-5, -7), new Vector2Int(-5, -6), new Vector2Int(-5, -5), new Vector2Int(-5, -4), new Vector2Int(-5, -3), new Vector2Int(-5, -2), new Vector2Int(-5, -1), new Vector2Int(-5, 0), new Vector2Int(-5, 1), new Vector2Int(-5, 2), new Vector2Int(-5, 3), new Vector2Int(-5, 4), new Vector2Int(-5, 5), new Vector2Int(-5, 6), new Vector2Int(-5, 7),
        new Vector2Int(-4, -7), new Vector2Int(-4, -6), new Vector2Int(-4, -5), new Vector2Int(-4, -4), new Vector2Int(-4, -3), new Vector2Int(-4, -2), new Vector2Int(-4, -1), new Vector2Int(-4, 0), new Vector2Int(-4, 1), new Vector2Int(-4, 2), new Vector2Int(-4, 3), new Vector2Int(-4, 4), new Vector2Int(-4, 5), new Vector2Int(-4, 6), new Vector2Int(-4, 7),
        new Vector2Int(-3, -7), new Vector2Int(-3, -6), new Vector2Int(-3, -5), new Vector2Int(-3, -4), new Vector2Int(-3, -3), new Vector2Int(-3, -2), new Vector2Int(-3, -1), new Vector2Int(-3, 0), new Vector2Int(-3, 1), new Vector2Int(-3, 2), new Vector2Int(-3, 3), new Vector2Int(-3, 4), new Vector2Int(-3, 5), new Vector2Int(-3, 6), new Vector2Int(-3, 7),
        new Vector2Int(-2, -7), new Vector2Int(-2, -6), new Vector2Int(-2, -5), new Vector2Int(-2, -4), new Vector2Int(-2, -3), new Vector2Int(-2, -2), new Vector2Int(-2, -1), new Vector2Int(-2, 0), new Vector2Int(-2, 1), new Vector2Int(-2, 2), new Vector2Int(-2, 3), new Vector2Int(-2, 4), new Vector2Int(-2, 5), new Vector2Int(-2, 6), new Vector2Int(-2, 7),
        new Vector2Int(-1, -7), new Vector2Int(-1, -6), new Vector2Int(-1, -5), new Vector2Int(-1, -4), new Vector2Int(-1, -3), new Vector2Int(-1, -2), new Vector2Int(-1, -1), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(-1, 2), new Vector2Int(-1, 3), new Vector2Int(-1, 4), new Vector2Int(-1, 5), new Vector2Int(-1, 6), new Vector2Int(-1, 7),
        new Vector2Int(0, -7), new Vector2Int(0, -6), new Vector2Int(0, -5), new Vector2Int(0, -4), new Vector2Int(0, -3), new Vector2Int(0, -2), new Vector2Int(0, -1), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(0, 3), new Vector2Int(0, 4), new Vector2Int(0, 5), new Vector2Int(0, 6), new Vector2Int(0, 7),
        new Vector2Int(1, -7), new Vector2Int(1, -6), new Vector2Int(1, -5), new Vector2Int(1, -4), new Vector2Int(1, -3), new Vector2Int(1, -2), new Vector2Int(1, -1), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(1, 2), new Vector2Int(1, 3), new Vector2Int(1, 4), new Vector2Int(1, 5), new Vector2Int(1, 6), new Vector2Int(1, 7),
        new Vector2Int(2, -7), new Vector2Int(2, -6), new Vector2Int(2, -5), new Vector2Int(2, -4), new Vector2Int(2, -3), new Vector2Int(2, -2), new Vector2Int(2, -1), new Vector2Int(2, 0), new Vector2Int(2, 1), new Vector2Int(2, 2), new Vector2Int(2, 3), new Vector2Int(2, 4), new Vector2Int(2, 5), new Vector2Int(2, 6), new Vector2Int(2, 7),
        new Vector2Int(3, -7), new Vector2Int(3, -6), new Vector2Int(3, -5), new Vector2Int(3, -4), new Vector2Int(3, -3), new Vector2Int(3, -2), new Vector2Int(3, -1), new Vector2Int(3, 0), new Vector2Int(3, 1), new Vector2Int(3, 2), new Vector2Int(3, 3), new Vector2Int(3, 4), new Vector2Int(3, 5), new Vector2Int(3, 6), new Vector2Int(3, 7),
        new Vector2Int(4, -7), new Vector2Int(4, -6), new Vector2Int(4, -5), new Vector2Int(4, -4), new Vector2Int(4, -3), new Vector2Int(4, -2), new Vector2Int(4, -1), new Vector2Int(4, 0), new Vector2Int(4, 1), new Vector2Int(4, 2), new Vector2Int(4, 3), new Vector2Int(4, 4), new Vector2Int(4, 5), new Vector2Int(4, 6), new Vector2Int(4, 7),
        new Vector2Int(5, -7), new Vector2Int(5, -6), new Vector2Int(5, -5), new Vector2Int(5, -4), new Vector2Int(5, -3), new Vector2Int(5, -2), new Vector2Int(5, -1), new Vector2Int(5, 0), new Vector2Int(5, 1), new Vector2Int(5, 2), new Vector2Int(5, 3), new Vector2Int(5, 4), new Vector2Int(5, 5), new Vector2Int(5, 6), new Vector2Int(5, 7),
        new Vector2Int(6, -7), new Vector2Int(6, -6), new Vector2Int(6, -5), new Vector2Int(6, -4), new Vector2Int(6, -3), new Vector2Int(6, -2), new Vector2Int(6, -1), new Vector2Int(6, 0), new Vector2Int(6, 1), new Vector2Int(6, 2), new Vector2Int(6, 3), new Vector2Int(6, 4), new Vector2Int(6, 5), new Vector2Int(6, 6), new Vector2Int(6, 7),
        new Vector2Int(7, -7), new Vector2Int(7, -6), new Vector2Int(7, -5), new Vector2Int(7, -4), new Vector2Int(7, -3), new Vector2Int(7, -2), new Vector2Int(7, -1), new Vector2Int(7, 0), new Vector2Int(7, 1), new Vector2Int(7, 2), new Vector2Int(7, 3), new Vector2Int(7, 4), new Vector2Int(7, 5), new Vector2Int(7, 6), new Vector2Int(7, 7)
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
                player.ShowAttackOptions(allBoardDirections, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in FA02_card.OnClick");
        }
    }
}

public class FA02: Card
{
    public FA02() : base(CardType.Attack, "FA02", 1) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/FA02");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/FA02");
    }

    public override string GetDescription()
    {
        return "烈矢 燃点 1 全场 造成1点伤害，并在目标处创造地形燃点";
    }

    public override int GetDamageAmount()
    {
        return 1;
    }

    public override void OnCardExecuted(Vector2Int attackPos)
    {
        // 在攻击位置创造燃点地形
        PlaceFirePointAt(attackPos);
    }
    
    private void PlaceFirePointAt(Vector2Int gridPosition)
    {
        Debug.Log($"FA02: Placing FirePoint at grid position: {gridPosition}");
        LocationManager locationManager = UnityEngine.Object.FindObjectOfType<LocationManager>();
        if (locationManager != null)
        {
            GameObject firePointPrefab = Resources.Load<GameObject>("Prefabs/Location/FirePoint");
            if (firePointPrefab != null)
            {
                locationManager.CreateFirePoint(firePointPrefab, gridPosition);
                Debug.Log($"FA02: FirePoint created at {gridPosition}");
            }
            else
            {
                Debug.LogError("FA02: FirePoint prefab not found");
            }
        }
        else
        {
            Debug.LogError("FA02: LocationManager not found");
        }
    }
}