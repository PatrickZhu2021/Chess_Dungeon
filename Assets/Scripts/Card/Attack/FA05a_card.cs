using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class FA05a_card: CardButtonBase
{
    Vector2Int[] crossII_Directions = new Vector2Int[]
    {
        new Vector2Int(-2, 0), new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(2, 0),
        new Vector2Int(0, -2), new Vector2Int(0, -1), new Vector2Int(0, 1), new Vector2Int(0, 2)
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
                player.ShowAttackOptions(crossII_Directions, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in FA05a_card.OnClick");
        }
    }
}

public class FA05a: Card
{
    public FA05a() : base(CardType.Attack, "FA05a", 0, isQuick: true, isTemporary: true) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/FA05a");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/FA05a");
    }

    public override string GetDescription()
    {
        return "炽炬-点燃 快速，衍生，燃点 0 十字II级 快速，衍生，造成X点伤害，X为场上燃点数量";
    }

    public override int GetDamageAmount()
    {
        // X为场上燃点数量
        LocationManager locationManager = UnityEngine.Object.FindObjectOfType<LocationManager>();
        if (locationManager != null)
        {
            int firePointCount = locationManager.activeFirePoints.Count;
            Debug.Log($"FA05a: Damage = {firePointCount} (number of active fire points)");
            return firePointCount;
        }
        return 0;
    }

    public override void OnCardExecuted(Vector2Int attackPos)
    {
        Debug.Log($"FA05a OnCardExecuted called at {attackPos}");
        // 衍生卡牌通常没有额外效果，伤害已经在GetDamageAmount中计算
    }
}