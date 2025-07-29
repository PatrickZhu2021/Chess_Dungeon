using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;
using System.Linq;

public class FS05_card: CardButtonBase
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
                Debug.Log("FS05 card used: creating karma link");
            }
        }
        else
        {
            Debug.LogError("Card is null in FS05_card.OnClick");
        }
    }
}

public class FS05: Card
{
    public FS05() : base(CardType.Special, "FS05", 1) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/FS05");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/FS05");
    }

    public override string GetDescription()
    {
        return "业力之锁 1 连接离自己最近和最远的的2名敌方棋子，双方共享受到的伤害";
    }

    public override void OnCardExecuted()
    {
        Debug.Log("FS05 OnCardExecuted called");
        
        // 连接最近和最远的2名敌方棋子
        CreateKarmaLink();
    }
    
    private void CreateKarmaLink()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        List<Monster> validMonsters = new List<Monster>();
        
        // 获取所有有效的敌人
        foreach (GameObject monsterObject in monsters)
        {
            Monster monster = monsterObject.GetComponent<Monster>();
            if (monster != null && monster.health > 0)
            {
                validMonsters.Add(monster);
            }
        }
        
        if (validMonsters.Count < 2)
        {
            Debug.Log("FS05: Not enough enemies to create karma link");
            return;
        }
        
        // 计算每个敌人到玩家的距离
        Vector2Int playerPos = player.position;
        var monstersWithDistance = validMonsters.Select(monster => new
        {
            Monster = monster,
            Distance = Mathf.Abs(monster.position.x - playerPos.x) + Mathf.Abs(monster.position.y - playerPos.y)
        }).OrderBy(x => x.Distance).ToList();
        
        // 获取最近和最远的敌人
        Monster closestMonster = monstersWithDistance.First().Monster;
        Monster farthestMonster = monstersWithDistance.Last().Monster;
        
        // 如果最近和最远是同一个敌人（只有一个敌人），选择第二近的
        if (closestMonster == farthestMonster && monstersWithDistance.Count > 1)
        {
            farthestMonster = monstersWithDistance[monstersWithDistance.Count - 2].Monster;
        }
        
        // 创建业力连接
        Monster.CreateKarmaLink(closestMonster, farthestMonster);
        
        Debug.Log($"FS05: Created karma link between {closestMonster.monsterName} (closest) and {farthestMonster.monsterName} (farthest)");
    }
}