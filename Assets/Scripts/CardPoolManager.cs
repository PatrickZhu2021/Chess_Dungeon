using System.Collections.Generic;
using UnityEngine;

public static class CardPoolManager
{
    // 内部使用的稀有度卡池字典
    private static Dictionary<string, List<Card>> rarityPools;

    /// <summary>
    /// 初始化各稀有度卡池
    /// </summary>
    public static void InitializeRarityPools()
    {
        rarityPools = new Dictionary<string, List<Card>>();

        // Common 卡池
        rarityPools["Common"] = new List<Card>
        {
            // new UpgradedPawnCard(),
            new KnightCard(),
            new BishopCard(),
            // new SwordCard(),
            // new RitualDagger(),
            // new FlameSword(),
            new BA01(),
            new BA02(),
            new BA03(),
            new BA04(),
            new BA05(),
            new BA07(),
            new BA08(),
            new BS02(),
            new BS05(),
            new BS07(),
            new BS09(),
            new FA04(),
            new FS03(),
            new FS04(),
            new WA01(),
            new WA02(),
            new WA03(),
            new WA06()
        };

        // Uncommon 卡池
        rarityPools["Uncommon"] = new List<Card>
        {
            // new UpgradedSwordCard(),
            new RookCard(),
            // new SpearCard(),
            // new BowCard(),
            // new PotionCard(),
            // new EnergyCore(),
            // new SickleCard(),
            // new RitualSpear(),
            // new Assassin(),
            // new TwoBladeCard(),
            // new FloatSword(),
            // new Book(),
            // new Fan(),
            new BA09(),
            new BA10(),
            new BA11(),
            new BA12(),
            new BA14(),
            new BS01(),
            new BS06(),
            new BS08(),
            new FA01(),
            new FA03(),
            new WA04(),
            new WA05()
        };

        // Epic 卡池
        rarityPools["Epic"] = new List<Card>
        {
            // new FlailCard(),
            // new FloatSword(),
            // new DarkEnergy(),
            // new MadnessEcho(),
            // new Vine(),
            // new BookOfPawn(),
            // new FlameBow(),
            new BA13(),
            new FA02(),
            new FA05(),
            new FA08(),
            new FS01(),
            new FS02(),
            new FS05(),
            new WA07()
        };

        // Legendary 卡池
        rarityPools["Legendary"] = new List<Card>
        {
            new BookOfKnight(),
            new BookOfBishop(),
            new BookOfRook(),
            new BookOfQueen(),
            new FA06(),
            new FA07()
        };
    }

    /// <summary>
    /// 根据随机数返回卡牌稀有度
    /// </summary>
    /// <returns>卡牌稀有度字符串</returns>
    public static string GetRandomRarity()
    {
        float randomValue = Random.Range(0f, 100f);
        if (randomValue < 80f)
            return "Common";
        else if (randomValue < 95f)
            return "Uncommon";
        else if (randomValue < 99f)
            return "Epic";
        else
            return "Legendary";
    }

    /// <summary>
    /// 生成三张奖励卡牌
    /// </summary>
    /// <returns>奖励卡牌列表</returns>
    public static List<Card> GenerateRewardCards()
    {
        // 如果卡池未初始化，则先初始化
        if (rarityPools == null)
            InitializeRarityPools();

        List<Card> rewardCards = new List<Card>();
        HashSet<Card> selectedCards = new HashSet<Card>();

        // 生成三张不重复的卡牌
        while (rewardCards.Count < 3)
        {
            string rarity = GetRandomRarity();
            if (!rarityPools.ContainsKey(rarity))
            {
                Debug.LogWarning($"No card pool defined for rarity {rarity}");
                continue;
            }
            List<Card> cardPool = rarityPools[rarity];
            if (cardPool.Count > 0)
            {
                int randomIndex = Random.Range(0, cardPool.Count);
                Card selectedCard = cardPool[randomIndex];

                if (selectedCards.Contains(selectedCard))
                    continue;

                rewardCards.Add(selectedCard);
                selectedCards.Add(selectedCard);
            }
            else
            {
                Debug.LogWarning($"No cards available in the {rarity} rarity pool.");
            }
        }

        return rewardCards;
    }
}
