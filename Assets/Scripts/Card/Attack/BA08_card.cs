using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class BA08_card: CardButtonBase
{
    Vector2Int[] diagonalDirections = { 
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
                player.ShowAttackOptions(diagonalDirections, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in BA08_card.OnClick");
        }
    }
}

public class BA08: Card
{
    public BA08() : base(CardType.Attack, "BA08", 1) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/BA08");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/BA08");
    }

    public override string GetDescription()
    {
        return "斜向攻击，造成1点伤害，随后对3x3范围内另1名随机敌方棋子造成2点伤害。";
    }

    public override int GetDamageAmount()
    {
        return 1;
    }

    public override void OnCardExecuted(Vector2Int attackPos)
    {
        // 寻找3x3范围内的随机敌人（排除主攻击目标）
        List<Monster> nearbyMonsters = FindMonstersInRange(attackPos, 1);
        
        if (nearbyMonsters.Count > 0)
        {
            // 随机选择一个敌人
            int randomIndex = Random.Range(0, nearbyMonsters.Count);
            Monster targetMonster = nearbyMonsters[randomIndex];
            
            // 直接对目标造成2点伤害（加上伤害修正）
            targetMonster.TakeDamage(2 + player.damageModifierThisTurn);
            
            // 生成攻击特效
            Vector3 worldPosition = player.CalculateWorldPosition(targetMonster.position);
            GameObject effectInstance = Object.Instantiate(player.attackEffectPrefab, worldPosition, Quaternion.identity);
            Object.Destroy(effectInstance, 0.1f);
            
            Debug.Log($"BA08 secondary attack hit {targetMonster.name} for 2 damage");
        }
    }
    
    private List<Monster> FindMonstersInRange(Vector2Int centerPos, int range)
    {
        List<Monster> monstersInRange = new List<Monster>();
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        
        foreach (GameObject monsterObject in monsters)
        {
            Monster monster = monsterObject.GetComponent<Monster>();
            if (monster != null)
            {
                // 检查是否在3x3范围内（range=1表示周围1格，即3x3）
                int deltaX = Mathf.Abs(monster.position.x - centerPos.x);
                int deltaY = Mathf.Abs(monster.position.y - centerPos.y);
                
                if (deltaX <= range && deltaY <= range && monster.position != centerPos)
                {
                    // 检查是否是多格怪物的一部分
                    if (monster.IsPartOfMonster(monster.position))
                    {
                        monstersInRange.Add(monster);
                    }
                }
            }
        }
        
        return monstersInRange;
    }
}