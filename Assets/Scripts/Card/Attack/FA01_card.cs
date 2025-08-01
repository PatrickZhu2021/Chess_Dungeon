using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;

public class FA01_card: CardButtonBase
{
    Vector2Int[] crossDirections = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

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
                player.ShowAttackOptions(crossDirections, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in FA01_card.OnClick");
        }
    }
}

public class FA01: Card
{
    public FA01() : base(CardType.Attack, "FA01", 1) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/FA01");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/FA01");
    }

    public override string GetDescription()
    {
        return "灼锋 燃点 1 十字I级 造成1点伤害，并在目标处创造地形燃点";
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
        Debug.Log($"FA01: Placing FirePoint at grid position: {gridPosition}");
        LocationManager locationManager = UnityEngine.Object.FindObjectOfType<LocationManager>();
        if (locationManager != null)
        {
            GameObject firePointPrefab = Resources.Load<GameObject>("Prefabs/Location/FirePoint");
            if (firePointPrefab != null)
            {
                locationManager.CreateFirePoint(firePointPrefab, gridPosition);
                Debug.Log($"FA01: FirePoint created at {gridPosition}");
            }
            else
            {
                Debug.LogError("FA01: FirePoint prefab not found");
            }
        }
        else
        {
            Debug.LogError("FA01: LocationManager not found");
        }
    }
}