using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class FA07_card: CardButtonBase
{
    Vector2Int[] diagonalDirections = new Vector2Int[]
    {
        new Vector2Int(1, 1),   // 右上
        new Vector2Int(1, -1),  // 右下
        new Vector2Int(-1, 1),  // 左上
        new Vector2Int(-1, -1)  // 左下
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
            Debug.LogError("Card is null in FA07_card.OnClick");
        }
    }
}

public class FA07: Card
{
    public FA07() : base(CardType.Attack, "FA07", 1) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/FA07");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/FA07");
    }

    public override string GetDescription()
    {
        return "灼骨之刃 炽烈 1 斜向I级 造成1点伤害；若击杀目标单位，再获得1层【炽烈】";
    }

    public override int GetDamageAmount()
    {
        return 1;
    }

    public override void OnCardExecuted(Vector2Int attackPos)
    {
        Debug.Log($"FA07 OnCardExecuted called at {attackPos}");
        
        // 检查是否击杀了目标（伤害已经由Player.Attack造成）
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monsterObject in monsters)
        {
            Monster monster = monsterObject.GetComponent<Monster>();
            if (monster != null && monster.IsPartOfMonster(attackPos) && monster.health <= 0)
            {
                Debug.Log("FA07 killed target - gaining fervent stack");
                player.AddFervent(1);
                break;
            }
        }
    }

}