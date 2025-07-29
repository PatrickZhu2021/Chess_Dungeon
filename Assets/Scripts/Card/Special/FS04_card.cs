using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class FS04_card: CardButtonBase
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
                Debug.Log("FS04 card used: creating fire point and applying lure");
            }
        }
        else
        {
            Debug.LogError("Card is null in FS04_card.OnClick");
        }
    }
}

public class FS04: Card
{
    public FS04() : base(CardType.Special, "FS04", 1) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/FS04");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/FS04");
    }

    public override string GetDescription()
    {
        return "耀火引信 燃点，火域，瞩目 1 在1名随机敌方棋子处创造地形燃点；若火域内存在敌方棋子，再对1名火域内的随机敌方棋子施加5层【瞩目】";
    }

    public override void OnCardExecuted()
    {
        Debug.Log("FS04 OnCardExecuted called");
        
        // 在1名随机敌方棋子处创造地形燃点
        CreateFirePointAtRandomEnemy();
        
        // 若火域内存在敌方棋子，施加瞩目
        ApplyLureToEnemyInFireZone();
    }
    
    private void CreateFirePointAtRandomEnemy()
    {
        Monster randomEnemy = GetRandomEnemy();
        if (randomEnemy == null)
        {
            Debug.Log("FS04: No enemies available for fire point creation");
            return;
        }
        
        LocationManager locationManager = UnityEngine.Object.FindObjectOfType<LocationManager>();
        GameObject firePointPrefab = Resources.Load<GameObject>("Prefabs/Location/FirePoint");
        
        if (locationManager == null || firePointPrefab == null)
        {
            Debug.LogError("FS04: LocationManager or FirePoint prefab not found");
            return;
        }
        
        locationManager.CreateFirePoint(firePointPrefab, randomEnemy.position);
        Debug.Log($"FS04: Created FirePoint at {randomEnemy.position} ({randomEnemy.monsterName})");
    }
    
    private void ApplyLureToEnemyInFireZone()
    {
        LocationManager locationManager = UnityEngine.Object.FindObjectOfType<LocationManager>();
        if (locationManager == null || locationManager.activeFireZones.Count == 0)
        {
            Debug.Log("FS04: No active fire zones");
            return;
        }
        
        // 获取火域内的所有敌人
        List<Monster> enemiesInFireZone = GetEnemiesInFireZone();
        if (enemiesInFireZone.Count == 0)
        {
            Debug.Log("FS04: No enemies in fire zone");
            return;
        }
        
        // 随机选择一个火域内的敌人施加瞩目
        int randomIndex = Random.Range(0, enemiesInFireZone.Count);
        Monster targetEnemy = enemiesInFireZone[randomIndex];
        
        targetEnemy.AddLure(5);
        Debug.Log($"FS04: Applied 5 lure stacks to {targetEnemy.monsterName} at {targetEnemy.position}");
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
    
    private List<Monster> GetEnemiesInFireZone()
    {
        List<Monster> enemiesInFireZone = new List<Monster>();
        LocationManager locationManager = UnityEngine.Object.FindObjectOfType<LocationManager>();
        
        if (locationManager == null) return enemiesInFireZone;
        
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monsterObject in monsters)
        {
            Monster monster = monsterObject.GetComponent<Monster>();
            if (monster != null && monster.health > 0)
            {
                Vector3 monsterWorldPos = monster.transform.position;
                
                // 检查是否在任何火域内
                foreach (FireZone fireZone in locationManager.activeFireZones)
                {
                    if (fireZone != null && IsMonsterInFireZone(monsterWorldPos, fireZone))
                    {
                        enemiesInFireZone.Add(monster);
                        break; // 找到一个火域就够了
                    }
                }
            }
        }
        
        return enemiesInFireZone;
    }
    
    private bool IsMonsterInFireZone(Vector3 monsterPos, FireZone fireZone)
    {
        // 简化检查：假设FireZone有公共方法检查点是否在内部
        // 这里需要访问FireZone的私有方法，暂时返回true作为占位
        return true; // 简化实现
    }
}