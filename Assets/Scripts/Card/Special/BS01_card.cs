using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
using System.Collections.Generic;

public class BS01_card: CardButtonBase
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
                Debug.Log("BS01 card used: targeting highest health enemy");
            }
        }
        else
        {
            Debug.LogError("Card is null in BS01_card.OnClick");
        }
    }
}

public class BS01: Card
{
    public BS01() : base(CardType.Special, "BS01", 1) 
    { 
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/BS01");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/BS01");
    }

    public override string GetDescription()
    {
        return "刃祝 / 1 / 对场上血量最高的敌方棋子造成2点伤害，此效果重复X次，X为打出时手牌区的武器牌总数";
    }

    public override int GetDamageAmount()
    {
        return 2;
    }

    public override void OnCardExecuted()
    {
        Debug.Log("BS01 OnCardExecuted called");
        
        // 计算手牌中武器牌的数量
        int weaponCount = CountWeaponCardsInHand();
        Debug.Log($"BS01: Found {weaponCount} weapon cards in hand");
        
        if (weaponCount == 0)
        {
            Debug.Log("BS01: No weapon cards in hand, no damage dealt");
            return;
        }
        
        // 重复X次，对血量最高的敌人造成2点伤害
        for (int i = 0; i < weaponCount; i++)
        {
            Monster highestHealthMonster = FindHighestHealthMonster();
            if (highestHealthMonster != null)
            {
                int finalDamage = 2 + player.damageModifierThisTurn;
                highestHealthMonster.TakeDamage(finalDamage);
                
                // 生成攻击特效
                Vector3 worldPosition = player.CalculateWorldPosition(highestHealthMonster.position);
                GameObject effectInstance = Object.Instantiate(player.attackEffectPrefab, worldPosition, Quaternion.identity);
                Object.Destroy(effectInstance, 0.1f);
                
                Debug.Log($"BS01 attack {i + 1}/{weaponCount}: dealt {finalDamage} damage to {highestHealthMonster.monsterName}");
            }
            else
            {
                Debug.Log($"BS01 attack {i + 1}/{weaponCount}: no monsters remaining");
                break;
            }
        }
    }
    
    private int CountWeaponCardsInHand()
    {
        int count = 0;
        DeckManager deckManager = GameObject.FindObjectOfType<DeckManager>();
        if (deckManager != null)
        {
            foreach (Card card in deckManager.hand)
            {
                // 检查是否是武器牌（攻击类型的卡牌）
                if (card.cardType == CardType.Attack)
                {
                    count++;
                }
            }
        }
        return count;
    }
    
    private Monster FindHighestHealthMonster()
    {
        Monster highestHealthMonster = null;
        int highestHealth = -1;
        
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monsterObject in monsters)
        {
            Monster monster = monsterObject.GetComponent<Monster>();
            if (monster != null && monster.health > highestHealth)
            {
                highestHealth = monster.health;
                highestHealthMonster = monster;
            }
        }
        
        return highestHealthMonster;
    }
}