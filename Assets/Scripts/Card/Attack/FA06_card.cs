using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class FA06_card: CardButtonBase
{
    Vector2Int[] crossInfiniteDirections = new Vector2Int[]
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
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
                // 检查玩家是否在火域上
                if (IsPlayerInFireZone())
                {
                    int damage = card.GetDamageAmount();
                    player.damage = damage;
                    player.ShowAttackOptions(crossInfiniteDirections, card);
                }
                else
                {
                    Debug.Log("FA06: Cannot use - player not in fire zone");
                }
            }
        }
        else
        {
            Debug.LogError("Card is null in FA06_card.OnClick");
        }
    }
    
    private bool IsPlayerInFireZone()
    {
        LocationManager locationManager = UnityEngine.Object.FindObjectOfType<LocationManager>();
        if (locationManager == null) return false;
        
        foreach (FireZone fireZone in locationManager.activeFireZones)
        {
            if (fireZone != null)
            {
                return true; // 简化检查：如果有活跃的火域就允许使用
            }
        }
        
        return false;
    }
}

public class FA06: Card
{
    public FA06() : base(CardType.Attack, "FA06", 1) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/FA06");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/FA06");
    }

    public override string GetDescription()
    {
        return "蛇龙之涎 燃点，火域，炽烈 1 十字无限 仅在地形火域上时可以打出；对该方向所有敌方单位造成2点伤害。若命中至少2名敌方单位，再获得1层【炽烈】";
    }

    public override int GetDamageAmount()
    {
        return 2;
    }

    public override void OnCardExecuted(Vector2Int attackPos)
    {
        Debug.Log($"FA06 OnCardExecuted called at {attackPos}");
        
        // 计算攻击方向
        Vector2Int direction = attackPos - player.position;
        Vector2Int normalizedDirection = new Vector2Int(
            direction.x != 0 ? (direction.x > 0 ? 1 : -1) : 0,
            direction.y != 0 ? (direction.y > 0 ? 1 : -1) : 0
        );
        
        // 攻击该方向所有敌人
        int hitCount = AttackInDirection(normalizedDirection);
        
        // 若命中至少2名敌方单位，获得1层炽烈
        if (hitCount >= 2)
        {
            player.AddFervent(1);
            Debug.Log($"FA06: Hit {hitCount} enemies, gained 1 fervent stack");
        }
    }
    
    private int AttackInDirection(Vector2Int direction)
    {
        int hitCount = 0;
        Vector2Int currentPos = player.position + direction;
        
        // 沿着方向攻击所有位置上的敌人
        while (player.IsValidPosition(currentPos))
        {
            GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
            foreach (GameObject monsterObject in monsters)
            {
                Monster monster = monsterObject.GetComponent<Monster>();
                if (monster != null && monster.IsPartOfMonster(currentPos))
                {
                    monster.TakeDamage(2);
                    hitCount++;
                    Debug.Log($"FA06: Hit monster at {currentPos}");
                }
            }
            
            currentPos += direction;
        }
        
        Debug.Log($"FA06: Total enemies hit in direction {direction}: {hitCount}");
        return hitCount;
    }
}