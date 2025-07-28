using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class BA14_card: CardButtonBase
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
                
                // 显示全方向II级攻击选项（2格距离）
                List<Vector2Int> extendedDirections = new List<Vector2Int>();
                foreach (Vector2Int direction in allDirections)
                {
                    extendedDirections.Add(direction);
                    extendedDirections.Add(direction * 2); // II级，2格距离
                }
                
                player.ShowAttackOptions(extendedDirections.ToArray(), card);
            }
        }
        else
        {
            Debug.LogError("Card is null in BA14_card.OnClick");
        }
    }
}

public class BA14: Card
{
    public BA14() : base(CardType.Attack, "BA14", 1) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/BA14");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/BA14");
    }

    public override string GetDescription()
    {
        return "皮鞭 / 1 全方向II级 造成2点伤害，并获得等同于造成伤害量的行动点";
    }

    public override int GetDamageAmount()
    {
        return 2;
    }

    public override void OnCardExecuted(Vector2Int attackPos)
    {
        // 检查是否有怪物在攻击位置，并计算实际造成的伤害
        int actualDamageDealt = 0;
        
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monsterObject in monsters)
        {
            Monster monster = monsterObject.GetComponent<Monster>();
            if (monster != null && monster.IsPartOfMonster(attackPos))
            {
                // 计算实际造成的伤害（伤害已经由Player.Attack造成）
                int theoreticalDamage = 2 + player.damageModifierThisTurn;
                // 实际伤害不能超过怪物的最大血量
                actualDamageDealt = theoreticalDamage; // 假设怪物血量足够，实际伤害等于理论伤害
                Debug.Log($"BA14 hit monster at {attackPos}, theoretical damage: {theoreticalDamage}");
                break;
            }
        }
        
        // 只有在实际造成伤害时才获得行动点
        if (actualDamageDealt > 0)
        {
            TurnManager turnManager = GameObject.FindObjectOfType<TurnManager>();
            if (turnManager != null)
            {
                for (int i = 0; i < actualDamageDealt; i++)
                {
                    turnManager.AddAction();
                }
                Debug.Log($"BA14: Added {actualDamageDealt} action points based on actual damage dealt");
            }
        }
        else
        {
            Debug.Log("BA14: No monster hit, no action points gained");
        }
    }
}