using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;

public class BA04_card: CardButtonBase
{
    Vector2Int[] swordDirections = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

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
                player.ShowAttackOptions(swordDirections, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in BA04_card.OnClick");
        }
    }
}

public class BA04: Card
{
    public BA04() : base(CardType.Attack, "BA04", 10, isLingering: true) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/BA04");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/BA04");
    }

    public override string GetDescription()
    {
        return "上下左右攻击，造成1点伤害，并抽取等同于造成伤害量的卡牌。长驻。";
    }

    public override int GetDamageAmount()
    {
        return 1;
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
                int theoreticalDamage = 1 + player.damageModifierThisTurn;
                actualDamageDealt = theoreticalDamage; // 假设怪物血量足够，实际伤害等于理论伤害
                Debug.Log($"BA04 hit monster at {attackPos}, theoretical damage: {theoreticalDamage}");
                break;
            }
        }
        
        // 只有在实际造成伤害时才抽卡
        if (actualDamageDealt > 0)
        {
            DeckManager deckManager = GameObject.FindObjectOfType<DeckManager>();
            if (deckManager != null)
            {
                deckManager.DrawCards(actualDamageDealt);
                Debug.Log($"BA04: Drew {actualDamageDealt} cards based on actual damage dealt");
            }
        }
        else
        {
            Debug.Log("BA04: No monster hit, no cards drawn");
        }
    }
}