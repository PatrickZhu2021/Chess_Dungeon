using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;

public class WS01_card : CardButtonBase
{
    private bool isListening = false;
    
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
    }

    protected override void Start()
    {
        base.Start();
        
        // 在 Start 中开始监听，确保 player 已初始化
        StartListening();

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
                Debug.Log("WS01 card used");
            }
        }
        else
        {
            Debug.LogError("Card is null in WS01_card.OnClick");
        }
    }
    
    private void StartListening()
    {
        if (!isListening && player != null)
        {
            player.OnMoveCardUsed += OnMoveCardUsed;
            isListening = true;
            Debug.Log("WS01: Started listening for move card usage");
        }
    }
    
    private void OnMoveCardUsed(Card moveCard)
    {
        // 回响效果：在自身3x3范围内空格创造地形潮沼
        LocationManager locationManager = GameObject.FindObjectOfType<LocationManager>();
        if (locationManager != null && player != null)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    Vector2Int checkPos = player.position + new Vector2Int(dx, dy);
                    
                    // 检查位置是否有效且为空格
                    if (player.IsValidPosition(checkPos) && 
                        !locationManager.IsNonEnterablePosition(checkPos) &&
                        KeywordEffects.GetMonsterAtPosition(checkPos) == null &&
                        checkPos != player.position)
                    {
                        locationManager.CreateMire(checkPos);
                    }
                }
            }
            Debug.Log("WS01: Echo effect - created mires in 3x3 area");
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