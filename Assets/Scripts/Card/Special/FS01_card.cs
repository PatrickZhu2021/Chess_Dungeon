using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class FS01_card: CardButtonBase
{
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
                player.currentCard = card;
                player.ExecuteCurrentCard();
                Debug.Log("FS01 card used: consuming all action points");
            }
        }
        else
        {
            Debug.LogError("Card is null in FS01_card.OnClick");
        }
    }
}

public class FS01: Card
{
    public FS01() : base(CardType.Special, "FS01", 0, isQuick: true) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/FS01");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/FS01");
    }

    public override string GetDescription()
    {
        return "焚咒 燃点 0 快速，消耗所有行动点，对随机敌方棋子造成1点伤害并在目标处创造地形燃点，重复X次且伤害逐次+1，X为消耗行动点数量";
    }

    public override void OnCardExecuted()
    {
        Debug.Log("FS01 OnCardExecuted called");
        
        // 获取当前行动点数量
        int actionPoints = player.actions;
        Debug.Log($"FS01: Consuming {actionPoints} action points");
        
        // 消耗所有行动点
        player.actions = 0;
        
        // 对随机敌人造成递增伤害并创造燃点
        ExecuteBurningCurse(actionPoints);
        
        // 更新UI
        TurnManager turnManager = UnityEngine.Object.FindObjectOfType<TurnManager>();
        if (turnManager != null)
        {
            turnManager.UpdateActionText();
        }
    }
    
    private void ExecuteBurningCurse(int times)
    {
        LocationManager locationManager = UnityEngine.Object.FindObjectOfType<LocationManager>();
        GameObject firePointPrefab = Resources.Load<GameObject>("Prefabs/Location/FirePoint");
        
        if (locationManager == null || firePointPrefab == null)
        {
            Debug.LogError("FS01: LocationManager or FirePoint prefab not found");
            return;
        }
        
        for (int i = 0; i < times; i++)
        {
            // 获取随机敌人
            Monster randomEnemy = GetRandomEnemy();
            if (randomEnemy != null)
            {
                int damage = 1 + i; // 伤害逐次+1
                randomEnemy.TakeDamage(damage);
                Debug.Log($"FS01: Hit {randomEnemy.name} for {damage} damage (iteration {i + 1})");
                
                // 在目标处创造燃点
                Vector2Int enemyGridPos = GetGridPosition(randomEnemy.transform.position);
                locationManager.CreateFirePoint(firePointPrefab, enemyGridPos);
                Debug.Log($"FS01: Created FirePoint at {enemyGridPos}");
            }
            else
            {
                Debug.Log($"FS01: No enemies available for iteration {i + 1}");
                break; // 没有敌人时停止
            }
        }
        
        Debug.Log($"FS01: Completed burning curse with {times} iterations");
    }
    
    private Monster GetRandomEnemy()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        List<Monster> validMonsters = new List<Monster>();
        
        foreach (GameObject monsterObject in monsters)
        {
            Monster monster = monsterObject.GetComponent<Monster>();
            if (monster != null && monster.health > 0)
            {
                validMonsters.Add(monster);
            }
        }
        
        if (validMonsters.Count > 0)
        {
            int randomIndex = Random.Range(0, validMonsters.Count);
            return validMonsters[randomIndex];
        }
        
        return null;
    }
    
    private Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        // 使用Player的CalculateGridPosition方法转换世界坐标到网格坐标
        return player.CalculateGridPosition(worldPosition);
    }
}