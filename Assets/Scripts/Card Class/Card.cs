using UnityEngine;
using System;
using System.Collections.Generic;

public enum CardType { Move, Attack, Special}

[System.Serializable]
public class Card
{
    public Player player;
    public MonsterManager monsterManager;
    
    public CardType cardType;
    public string Id;
    public string cardName;
    public int cost; // 添加花费属性
    public virtual List<CardUpgrade> UpgradeOptions { get; protected set; } = new List<CardUpgrade>();

    public bool isQuick; // 新增 quick 变量
    public bool isEnergy; // 新增 energy 变量
    public bool isMadness;
    public bool isExhaust; // 消耗属性
    public bool isLingering; // 长驻属性
    public bool isForethought; // 谋定属性
    public string upgradeFrom; // 升级来源
    public int hoardingValue; // 囤积值
    public bool isPartner;
    public bool isTemporary;
    public bool isTriumph; // 凯旋属性
    public bool isGrace; // 恩赐属性

    public Card(CardType type, string Id = "tbd", int cost = 10, string upgradeFrom = null, bool isQuick = false, int hoardingValue = 0, bool isPartner = false, bool isEnergy = false,
    bool isMadness = false, bool isTemporary = false, bool isExhaust = false, bool isLingering = false, bool isForethought = false, bool isTriumph = false, bool isGrace = false)
    {
        cardType = type;
        this.Id = Id;
        this.cost = cost;
        this.upgradeFrom = upgradeFrom;
        this.isQuick = isQuick;
        this.isMadness = isMadness;
        this.isEnergy = isEnergy;
        this.hoardingValue = hoardingValue;
        this.isPartner = isPartner;
        this.isTemporary = isTemporary;
        this.isExhaust = isExhaust;
        this.isLingering = isLingering;
        this.isForethought = isForethought;
        this.isTriumph = isTriumph;
        this.isGrace = isGrace;

        this.player = GameObject.FindObjectOfType<Player>();
        this.monsterManager = GameObject.FindObjectOfType<MonsterManager>();
    }

    public virtual GameObject GetPrefab()
    {
        return null;
    }

    public virtual Sprite GetSprite()
    {
        return null;
    }
    public virtual string GetDescription()
    {
        return null;
    }

    public virtual Type GetScriptType()
    {
        return typeof(CardButtonBase); // 默认返回基础类型
    }

    public virtual void ExhaustEffect()
    {
        // 这里可以加入每张卡牌独特的 Exhaust 效果
    }

    public virtual void DiscardEffect()
    {
        // 这里可以加入每张卡牌独特的 Discard 效果
    }

    public virtual void OnCardExecuted()
    {
        // 记录卡牌使用指标
        if (GameMetrics.Instance != null)
        {
            string cardId = !string.IsNullOrEmpty(cardName) ? cardName : Id;
            GameMetrics.Instance.RecordCardPlayed(cardId, cost);
        }
    }

    public virtual void OnCardExecuted(Vector2Int gridPosition)
    {
        OnCardExecuted();
    }

    public virtual int GetDamageAmount()
    {
        return 1;
    }

    public virtual bool IsUpgraded()
    {
        return false;
    }
    public virtual bool HasUpgrade(CardUpgrade upgrade) => false;


    public virtual List<CardUpgrade> GetUpgradeOptions()
    {
        return UpgradeOptions;
    }

    public virtual void AddUpgrade(CardUpgrade upgrade)
    {

    }


    public virtual Card Clone()
    {
        Card copy = (Card)MemberwiseClone();          // ① 先做浅拷贝

        // ② 如果是 PawnCard（或含 upgrades 的其它子类），把列表深拷贝
        if (copy is PawnCard pawn)
        {
            // 新建一个 List，把原来的元素复制进去，避免引用同一 List
            pawn.upgrades = new List<CardUpgrade>(pawn.upgrades);
        }

        // 如有其它引用字段也需要深拷，可在这里补充

        return copy;
    }


    public string GetName()
    {
        return cardName;  // 假设每张卡牌有一个唯一名称
    }

}

public class BladeCard : Card
{
    public BladeCard() : base(CardType.Attack, "A02", 10) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/blade_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/blade_card");
    }
    public override string GetDescription()
    {
        return "斜向攻击";
    }
}

public class SpearCard : Card
{
    public SpearCard() : base(CardType.Attack, "A03", 20) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/spear_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/spear_card");
    }
    public override string GetDescription()
    {
        return "上下左右两格攻击";
    }
}

public class BowCard : Card
{
    public BowCard() : base(CardType.Attack, "A04", 50) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/bow_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/bow_card");
    }
    public override string GetDescription()
    {
        return "任意位置攻击";
    }
}

//FlailCard在独立文件里

public class PotionCard : Card
{
    public PotionCard() : base(CardType.Special, "S01", 40)
    {
        isQuick = true;
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/potion_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/potion_card");
    }
    public override string GetDescription()
    {
        return "快速，增加一点行动点";
    }
}

