using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;

public class WS01_card : CardButtonBase
{

    
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
    }

    protected override void Start()
    {
        base.Start();

        if (card != null && card.IsUpgraded())
        {
            Transform glow = transform.Find("UpgradeEffect");
            if (glow != null)
                glow.gameObject.SetActive(true);
        }
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
                
                // 激活 WS01 回响效果
                player.ws01EchoActive = true;
                
                Debug.Log("WS01 card used");
            }
        }
        else
        {
            Debug.LogError("Card is null in WS01_card.OnClick");
        }
    }


}

public class WS01 : Card
{
    public WS01() : base(CardType.Special, "WS01", 1) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/WS01");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/WS01");
    }

    public override string GetDescription()
    {
        return "回响，潮沼，使用移动牌，回响：在自身3x3范围内空格创造地形潮沼";
    }

    public override void OnCardExecuted()
    {
        base.OnCardExecuted(); // 触发通用效果
        Debug.Log("WS01 OnCardExecuted called");
    }
}