using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class BA12_card: CardButtonBase
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
            Debug.LogError("Card is null in BA12_card.OnClick");
        }
    }
}

public class BA12: Card
{
    public BA12() : base(CardType.Attack, "BA12", 1) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/BA12");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/BA12");
    }

    public override string GetDescription()
    {
        return "双刃弯刀 / 1 全方向II级 造成1伤害，重复4次";
    }

    public override int GetDamageAmount()
    {
        return 1;
    }

    public override void OnCardExecuted(Vector2Int attackPos)
    {
        // 重复4次攻击
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monsterObject in monsters)
        {
            Monster monster = monsterObject.GetComponent<Monster>();
            if (monster != null && monster.IsPartOfMonster(attackPos))
            {
                int finalDamage = 1 + player.damageModifierThisTurn;
                for (int i = 0; i < 4; i++)
                {
                    monster.TakeDamage(finalDamage);
                    
                    // 生成攻击特效
                    Vector3 worldPosition = player.CalculateWorldPosition(attackPos);
                    GameObject effectInstance = Object.Instantiate(player.attackEffectPrefab, worldPosition, Quaternion.identity);
                    Object.Destroy(effectInstance, 0.1f);
                    
                    Debug.Log($"BA12 attack {i + 1}/4: dealt {finalDamage} damage to {monster.monsterName} at {attackPos}");
                }
                break;
            }
        }
    }
}