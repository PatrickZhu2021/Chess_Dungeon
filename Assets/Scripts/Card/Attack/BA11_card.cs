using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class BA11_card: CardButtonBase
{
    Vector2Int[] allDirections = { 
        new Vector2Int(1, 0), new Vector2Int(-1, 0), 
        new Vector2Int(0, 1), new Vector2Int(0, -1),
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
                
                // 过滤掉有怪物的位置
                List<Vector2Int> validDirections = new List<Vector2Int>();
                foreach (Vector2Int direction in allDirections)
                {
                    Vector2Int targetPos = player.position + direction;
                    if (player.IsValidPosition(targetPos) && !IsPositionOccupiedByMonster(targetPos))
                    {
                        validDirections.Add(direction);
                    }
                }
                
                player.ShowAttackOptions(validDirections.ToArray(), card);
            }
        }
        else
        {
            Debug.LogError("Card is null in BA11_card.OnClick");
        }
    }
    
    private bool IsPositionOccupiedByMonster(Vector2Int position)
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monsterObject in monsters)
        {
            Monster monster = monsterObject.GetComponent<Monster>();
            if (monster != null && monster.IsPartOfMonster(position))
            {
                return true;
            }
        }
        return false;
    }
}

public class BA11: Card
{
    public BA11() : base(CardType.Attack, "BA11", 0, isQuick: true, isTriumph: true) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/BA11");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/BA11");
    }

    public override string GetDescription()
    {
        return "铲子 快速，凯旋，拒障 0 全方向I级 快速，凯旋；创造耐久为4的地形拒障";
    }

    public override int GetDamageAmount()
    {
        return 0;
    }

    public override void OnCardExecuted(Vector2Int attackPos)
    {
        // 在攻击位置创建拒障
        LocationManager locationManager = GameObject.FindObjectOfType<LocationManager>();
        if (locationManager != null)
        {
            locationManager.CreateBarrier(attackPos, 4);
            Debug.Log($"BA11 created barrier at {attackPos} with durability 4");
        }
    }
}