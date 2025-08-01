using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class BA10_card: CardButtonBase
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
                player.ShowAttackOptions(allDirections, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in BA10_card.OnClick");
        }
    }
}

public class BA10: Card
{
    public BA10() : base(CardType.Attack, "BA10", 0) 
    { 
        isQuick = true;
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/BA10");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/BA10");
    }

    public override string GetDescription()
    {
        return "钉头锤 快速，眩晕 0 全方向I级 快速，造成1点伤害，施加1层【眩晕】。每回合手牌区内首次不存在移动牌时，将此牌置入手牌区(未完成)";
    }

    public override int GetDamageAmount()
    {
        return 1;
    }

    public override void OnCardExecuted(Vector2Int attackPos)
    {
        // 对攻击位置的怪物施加眩晕
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monsterObject in monsters)
        {
            Monster monster = monsterObject.GetComponent<Monster>();
            if (monster != null && monster.IsPartOfMonster(attackPos))
            {
                monster.AddStun(1);
                Debug.Log($"BA10 stunned {monster.monsterName} for 1 turn");
            }
        }
    }
}