using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    public MonsterManager monsterManager;
    public DeckManager deckManager; // 引用DeckManager
    public Button endTurnButton; // 引用EndTurn按钮
    public List<Button> allButtons; // 引用所有的按钮
    private EndTurnButtonRotation endTurnRotation; // 引用旋转脚本

    public Text actionText;

    public int turnCount = 0;
    public GameObject turnSlotPrefab;
    public Transform turnPanel;
    public int actions = 3;

    public Player player;
    public RewardManager rewardManager;
    private List<GameObject> turnSlots = new List<GameObject>();
    private int currentActionIndex = 0;

    void Start()
    {
        player = FindObjectOfType<Player>();
        InitializeTurnPanel();
        deckManager = FindObjectOfType<DeckManager>();
        rewardManager = FindObjectOfType<RewardManager>();
        UpdateActionText();

        if (monsterManager == null)
        {
            monsterManager = FindObjectOfType<MonsterManager>();
        }

        //monsterManager.SpawnMonster(new Slime());
        //monsterManager.SpawnMonster(new SlimeKing());

        // 添加EndTurn按钮点击事件监听
        if (endTurnButton != null)
        {
            endTurnButton.onClick.AddListener(AdvanceTurn);
            endTurnRotation = endTurnButton.GetComponent<EndTurnButtonRotation>();
        }

        // 初始化所有按钮列表
        allButtons = new List<Button>(FindObjectsOfType<Button>());
    }

    void InitializeTurnPanel()
    {
        for (int i = 0; i < actions; i++)
        {
            GameObject turnSlot = Instantiate(turnSlotPrefab, turnPanel);
            turnSlots.Add(turnSlot);
        }

        UpdateCursor();
    }

    public void AddAction()
    {
        player.actions += 1;
        GameObject turnSlot = Instantiate(turnSlotPrefab, turnPanel);
        turnSlots.Add(turnSlot);
        UpdateCursor();
        UpdateActionText();
        EnableAllButtons();
    }
    


    public void AdvanceTurn()
    {
        StartCoroutine(HandleTurnEnd());
    }

    private IEnumerator HandleTurnEnd()
    {
        DisableAllButtons(); // 禁用所有按钮
        
        // 确保 GameMetrics 被初始化
        if (GameMetrics.Instance == null)
        {
            GameObject metricsObject = new GameObject("GameMetrics");
            metricsObject.AddComponent<GameMetrics>();
            DontDestroyOnLoad(metricsObject);
        }
        
        // 在弃牌前结束当前回合的指标记录
        if (GameMetrics.Instance != null)
        {
            GameMetrics.Instance.EndTurn();
        }
        
        // 回合结束弃牌 
        deckManager.DiscardHand();
        player.ResetEffectsAtEndOfTurn();
        yield return new WaitForSeconds(0.3f);
        player.ClearMoveHighlights();
        player.actions = player.maxActions;
        // 回合开始时先减少愤怒层数，再应用伤害加成
        if (player.furyStacks > 0)
        {
            player.ReduceFury(1);
        }
        player.ApplyFuryDamage();
        
        // 回合开始时减少涌潮层数
        if (player.torrentStacks > 0)
        {
            player.torrentStacks--;
            Debug.Log($"Torrent stacks reduced. Remaining: {player.torrentStacks}");
        }
        turnCount++;
        
        // 开始新回合的指标记录
        if (GameMetrics.Instance != null)
        {
            GameMetrics.Instance.StartTurn(turnCount);
        }
        
        // 处理锚点效果（在怪物移动之前）
        LocationManager locationManager = FindObjectOfType<LocationManager>();
        if (locationManager != null)
        {
            locationManager.OnTurnEnd(); // 先处理锚点效果
        }
        
        monsterManager.OnTurnEnd(turnCount);
        Debug.Log("Turn end");

        // 等待所有史莱姆移动完成
        if (monsterManager.isLevelCompleted != true)
        {
            int monsterCount = monsterManager.GetMonsterCount();
            float delay = monsterCount * 0.3f + 0.1f; // 每个史莱姆移动0.5秒，再额外等待1. (每回合生成两只)
            yield return new WaitForSeconds(delay);
        }

        // 检查是否打开了RewardPanel，如果打开了则暂停执行，直到RewardPanel关闭
        while (rewardManager.isRewardPanelOpen)
        {
            yield return null; // 每帧检查一次，直到RewardPanel关闭
        }
        if (monsterManager.nextlevel == true)  {
            // 处理卡牌的回合结束效果
            deckManager.HandleEndOfTurnEffects();

            // 重置第一次抓牌标记
            deckManager.ResetFirstDrawFlag();
            
            // 回合结束抓新的手牌
            deckManager.DrawCards(deckManager.handSize);
        }
        else {
            monsterManager.nextlevel = true;
        }
        monsterManager.isLevelCompleted = false; // 标记关卡完成
        EnableAllButtons(); // 启用所有按钮
        UpdateActionText();
        
        // 玩家回合开始时旋转按钮回来
        if (endTurnRotation != null)
        {
            endTurnRotation.OnPlayerTurnStart();
        }

        ResetCursor();
        
        // 处理拒障耐久度（回合开始效果）
        if (locationManager != null)
        {
            locationManager.OnTurnStart(); // 处理回合开始效果（拒障）
            

        }
    }

    public void MoveCursor()
    {
        if (currentActionIndex < turnSlots.Count - 1)
        {
            currentActionIndex++;
        }
        else
        {
            //currentActionIndex = 0;
        }
        UpdateCursor();
    }

    void ResetCursor()
    {
        currentActionIndex = 0;
        // 如果现在turnpanel里的格子数大于3个，重新设置回3
        while (turnSlots.Count > 3)
        {
            GameObject excessSlot = turnSlots[turnSlots.Count - 1];
            turnSlots.RemoveAt(turnSlots.Count - 1);
            Destroy(excessSlot);
        }
        UpdateCursor();
    }

    void UpdateCursor()
    {
        for (int i = 0; i < turnSlots.Count; i++)
        {
            Transform cursorTransform = turnSlots[i].transform.Find("Cursor");
            if (cursorTransform != null)
            {
                Image cursorImage = cursorTransform.GetComponent<Image>();
                if (cursorImage != null)
                {
                    cursorImage.enabled = (i == currentActionIndex);
                    RectTransform cursorRect = cursorTransform.GetComponent<RectTransform>();
                    cursorRect.sizeDelta = new Vector2(40, 40); // Ensure the cursor is 40x40
                }
            }
        }
    }

    void DisableAllButtons()
    {
        foreach (Button button in allButtons)
        {
            if (button != null)
            {
                button.interactable = false;
            }
        }

        // Disable buttons in the cardPanel
        foreach (Transform card in deckManager.cardPanel)
        {
            Button cardButton = card.GetComponent<Button>();
            if (cardButton != null)
            {
                cardButton.interactable = false;
            }
        }
    }

    void EnableAllButtons()
    {
        foreach (Button button in allButtons)
        {
            if (button != null)
            {
                button.interactable = true;
            }
        }

        // 启用 cardPanel 中的所有按钮，并恢复拖拽
        foreach (Transform card in deckManager.cardPanel)
        {
            Button cardButton = card.GetComponent<Button>();
            CardButtonBase cardButtonBase = card.GetComponent<CardButtonBase>();

            if (cardButton != null)
            {
                cardButton.interactable = true;  // 恢复按钮点击
            }

            if (cardButtonBase != null)
            {
                cardButtonBase.SetDraggable(true);  // 恢复拖拽
            }
        }
    }

    public void UpdateActionText()
    {
        if (actionText != null)
        {
            actionText.text = player.maxActions.ToString() + "/" + player.actions.ToString();
        }
    }
}
