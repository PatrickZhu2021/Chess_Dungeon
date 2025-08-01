using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public static CardDatabase Instance { get; private set; }

    private Dictionary<string, Card> cardLibrary = new Dictionary<string, Card>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeCardDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeCardDatabase()
    {
        // 在这里手动添加所有卡牌对象
        AddCard(new PawnCard());
        AddCard(new KnightCard());
        AddCard(new BishopCard());
        AddCard(new RookCard());
        AddCard(new SwordCard());
        AddCard(new UpgradedSwordCard());
        AddCard(new BladeCard());
        AddCard(new SpearCard());
        AddCard(new BowCard());
        AddCard(new PotionCard());
        AddCard(new EnergyCore());
        AddCard(new SickleCard());
        AddCard(new RitualSpear());
        AddCard(new Assassin());
        AddCard(new TwoBladeCard());
        AddCard(new FloatSword());
        AddCard(new FlailCard());
        AddCard(new DarkEnergy());
        AddCard(new MadnessEcho());
        AddCard(new Vine());
        AddCard(new Coffin());
        AddCard(new Book());
        AddCard(new Belt());
        AddCard(new BookOfPawn());
        AddCard(new BookOfKnight());
        AddCard(new BookOfBishop());
        AddCard(new BookOfRook());
        AddCard(new BookOfQueen());
        AddCard(new Fan());
        AddCard(new Horn());
        AddCard(new WarFire());
        AddCard(new UpgradedPawnCard());
        AddCard(new FlameSword());
        AddCard(new FlameBow());
        AddCard(new RitualDagger());
        AddCard(new BA01());
        AddCard(new BA02());
        AddCard(new BA03());
        AddCard(new BA04());
        AddCard(new BA05());
        AddCard(new BA07());
        AddCard(new BA08());
        AddCard(new BA09());
        AddCard(new BA10());
        AddCard(new BA11());
        AddCard(new BA12());
        AddCard(new BA13());
        AddCard(new BA14());
        AddCard(new BS01());
        AddCard(new BS02());
        AddCard(new BS05());
        AddCard(new BS06());
        AddCard(new BS07());
        AddCard(new BS08());
        AddCard(new BS09());
        AddCard(new FA01());
        AddCard(new FA02());
        AddCard(new FA03());
        AddCard(new FA04());
        AddCard(new FA05());
        AddCard(new FA05a());
        AddCard(new FA06());
        AddCard(new FA07());
        AddCard(new FA08());
        AddCard(new FS01());
        AddCard(new FS02());
        AddCard(new FS03());
        AddCard(new FS04());
        AddCard(new FS05());
        AddCard(new WA01());
        AddCard(new WA02());
        AddCard(new WA03());
        AddCard(new WA04());
        AddCard(new WA05());
        AddCard(new WA06());
        AddCard(new WA07());
        AddCard(new WS01());
        AddCard(new WS02());
        AddCard(new WS03());
        AddCard(new WS04());
        AddCard(new WS05());
    }

    private void AddCard(Card card)
    {
        if (!cardLibrary.ContainsKey(card.Id))
        {
            cardLibrary.Add(card.Id, card);
        }
    }

    public Card GetCardById(string id)
    {
        // ① 拆分 Id：M01+Quick+Draw1 → ["M01","Quick","Draw1"]
        string[] parts  = id.Split('+');
        string   baseId = parts[0];

        // ② 先拿到“基础卡牌”原型
        if (!cardLibrary.TryGetValue(baseId, out Card proto))
            return null;

        // ③ 克隆一份（确保互不干扰）
        Card card = proto.Clone();

        // ④ 把后缀解析回升级
        for (int i = 1; i < parts.Length; i++)
        {
            if (System.Enum.TryParse(parts[i], out CardUpgrade up))
            
                card.AddUpgrade(up);
        }
        return card;
    }

}
