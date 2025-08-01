using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Effects;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    public Vector2Int position; // 棋子在棋盘上的位置
    public int boardSize = 8;   // 棋盘大小
    public GameObject moveHighlightPrefab; // 用于显示可移动位置的预制件
    public GameObject attackHighlightPrefab; // 用于显示可攻击位置的预制件
    public Vector3 cellSize = new Vector3(1, 1, 0); // 每个Tile的大小
    public Vector3 cellGap = new Vector3(0, 0, 0); // Cell Gap
    public int gold = 1000; // 金币数量
    public int actions = 3; //行动点
    public int maxActions = 3; //最大行动点
    public int health = 3; // 玩家初始血量
    public int armor = 3;
    public List<string> deck;
    public List<Relic> relics;
    public int damage = 1; //默认伤害
    public int damageModifierThisTurn = 0;
    public int furyStacks = 0; // 愤怒层数
    public int ferventStacks = 0; // 炙烈层数
    public int torrentStacks = 0; // 涌潮层数
    public Vector2Int lastAttackDirection { get; set; }

    public int cardsUsedThisTurn = 0; //本回合使用的卡牌数量
    public int movesThisTurn = 0; //本回合移动次数
    public Text healthText; 
    public Text armorText; 
    public bool isShieldActive = false;
    public bool isCharged = false; // 是否处于充能状态
    public Text energyStatusText;

    public int CurrentLevel;

    // 存储 ActivatePoint 和 DeactivatePoint 的位置信息
    public List<Vector2Int> activatePointPositions = new List<Vector2Int>();
    public List<Vector2Int> deactivatePointPositions = new List<Vector2Int>();

    //棋盘偏移量
    public float xshift = -1;
    public float yshift = -1;

    public List<GameObject> moveHighlights = new List<GameObject>(); // 初始化 moveHighlights 列表
    public Card currentCard;
    public DeckManager deckManager; // 引入DeckManager以更新卡牌状态
    public MonsterManager monsterManager;
    public LocationManager locationManager;
    public Text goldText;

    public event System.Action OnMoveComplete;

    private GameObject currentHighlight; // 用于存储当前的高亮对象

    public delegate void CardPlayed(Card card);
    public event CardPlayed OnCardPlayed;
    
    public delegate void MoveCardUsed(Card card);
    public event MoveCardUsed OnMoveCardUsed;


    public bool vineEffectActive = false; 
    public LayerMask attackHighlightLayer;
    public LayerMask moveHighlightLayer;
    public bool nextWeaponCardDoubleUse = false; // BS06狮鹫势效果
    public bool nextCardReturnToDeckTop = false; // BS07卷轴匣效果
    public bool saltBagEffectActive = false; // BS08盐袋效果
    public bool waveRiderEffectActive = false; // WS05逐浪效果
    public bool ws01EchoActive = false; // WS01回响效果

    private Animator animator;
    public GameObject attackEffectPrefab;
    public GameObject damageTextPrefab;
    public Vector2Int targetAttackPosition { get; set; }
    public Vector2Int lastAttackSnapshot;


    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        monsterManager = FindObjectOfType<MonsterManager>(); // 初始化deckManager引用
        position = new Vector2Int(boardSize / 2, boardSize / 2); // 初始化棋子位置到棋盘中央
        //Debug.Log($"Current Location: {position}");
        deckManager = FindObjectOfType<DeckManager>(); // 初始化deckManager引用
        LocationManager locationManager = FindObjectOfType<LocationManager>(); // 初始化locationManager引用
        animator = GetComponent<Animator>(); //初始化animator
        UpdatePosition();
        UpdateGoldText();
        currentCard = null;
    }

    void Start()
    {
        UpdateHealthText();
        UpdateArmorText();
        //position = monsterManager.GetEmptyPosition();
        //UpdatePosition();

        if (RelicManager.Instance != null)
        {
            RelicManager.Instance.OnGameStart(this);
        }
    }

    void Update()
    {
        //HandleMouseMovement(); // 处理鼠标移动
        //HandleMouseClick(); // 处理鼠标点击
        HandleMoveHighlightClick(); 
        HandleAttackHighlightClick(); 
        CheckForMonsterCollision();
    }

    public void HandleAttackHighlightClick()
    {
        if (Input.GetMouseButtonDown(0)) // 左键点击
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);

            // 使用 Raycast 仅检测 AttackHighlight Layer
            RaycastHit2D hit = Physics2D.Raycast(mousePosition2D, Vector2.zero, Mathf.Infinity, attackHighlightLayer);
            if (hit.collider != null)
            {
                MoveHighlight highlight = hit.collider.GetComponent<MoveHighlight>();
                if (highlight != null && !highlight.isMove)
                {
                    Attack(highlight.position); // 触发攻击逻辑
                }
            }
        }
    }

    public void HandleMoveHighlightClick()
    {
        if (Input.GetMouseButtonDown(0)) // 左键点击
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);

            // 使用 Raycast 仅检测 MoveHighlight Layer
            RaycastHit2D hit = Physics2D.Raycast(mousePosition2D, Vector2.zero, Mathf.Infinity, moveHighlightLayer);
            if (hit.collider != null)
            {
                MoveHighlight highlight = hit.collider.GetComponent<MoveHighlight>();
                if (highlight != null && highlight.isMove)  // 确保是一个移动高亮
                {
                    Debug.Log($"[MoveClick] Moving player to {highlight.position}");
                    Move(highlight.position);  // 调用玩家的 Move 方法
                }
                else
                {
                    Debug.LogWarning("[MoveClick] Detected highlight, but it's not a move highlight.");
                }
            }
            else
            {
                Debug.LogWarning("[MoveClick] No move highlight detected at click position.");
            }
        }
    }




    
    private void CheckForMonsterCollision()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monsterObject in monsters)
        {
            Monster monster = monsterObject.GetComponent<Monster>();
            if (monster != null && monster.IsPartOfMonster(position))
            {
                int monsterDamage = monster.GetAttackDamage();
                if (isShieldActive)
                {
                    // 架盾状态下：玩家受到伤害，不改变玩家位置，而是将攻击的敌人重定位到玩家周围
                    TakeDamage(monsterDamage); // 使用怪物的攻击伤害
                    Vector2Int newEnemyPos = FindSurroundingPosition(position, true); // skipCenter 为 true，不允许返回玩家所在的位置
                    monster.position = newEnemyPos;
                    monster.UpdatePosition();
                }   
                else
                {
                    // 非架盾状态：玩家受到伤害，并被击退
                    TakeDamage(monsterDamage); // 使用怪物的攻击伤害
                    Vector2Int knockbackDirection = -monster.lastRelativePosition;
                    Vector2Int desiredPos = position + knockbackDirection;
                    // 使用 FindSurroundingPosition 方法，skipCenter 为 false 表示允许返回理想击退位置
                    Vector2Int newPosition = FindSurroundingPosition(desiredPos, false);
                    position = newPosition;
                    UpdatePosition();
                }
                break;
            }
        }
    }

    private Vector2Int FindSurroundingPosition(Vector2Int basePosition, bool skipCenter = false)
    {
        // 当允许使用中心位置时，先检测 basePosition 本身
        if (!skipCenter && IsValidPosition(basePosition) && !IsBlockedBySomething(basePosition))
        {
            return basePosition;
        }

        int radius = 1;
        while (true)
        {
            List<Vector2Int> candidatePositions = new List<Vector2Int>();
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    // 如果需要跳过中心点，则忽略 dx == 0 且 dy == 0 的情况
                    if (skipCenter && dx == 0 && dy == 0)
                        continue;

                    Vector2Int pos = new Vector2Int(basePosition.x + dx, basePosition.y + dy);
                    if (IsValidPosition(pos) && !IsBlockedBySomething(pos))
                    {
                        candidatePositions.Add(pos);
                    }
                }
            }

            if (candidatePositions.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, candidatePositions.Count);
                return candidatePositions[index];
            }
            radius++;
        }
    }

    public void TakeDamage(int damage)
    {
        // 记录受到伤害指标
        if (GameMetrics.Instance != null)
        {
            GameMetrics.Instance.RecordDamageTaken(damage);
        }
        
        int remainingDamage = damage;

        // 如果有护甲，先用护甲抵消一部分伤害
        if (armor > 0)
        {
            // 若当前护甲大于等于伤害，伤害全部被护甲吸收
            if (armor >= damage)
            {
                armor -= damage;
                remainingDamage = 0;
            }
            // 否则，护甲被打空，剩余伤害继续扣血
            else
            {
                remainingDamage = damage - armor;
                armor = 0;
            }
        }

        // 护甲吸收完后，若仍有剩余伤害，则扣血
        if (remainingDamage > 0)
        {
            health -= remainingDamage;
        }
        UpdateHealthText();
        UpdateArmorText();
        
        // 显示伤害数字
        ShowDamageText(damage, false);
        
        if (health <= 0)
        {
            //("Player has died.");
            // 可在此实现游戏结束逻辑
        }
    }

    public void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + health.ToString();
        }
    }

    public void UpdateArmorText()
    {
        if (armorText != null)
        {
            armorText.text = "Armor: " + armor.ToString();
        }
    }

    void HandleMouseMovement()
    {
        // 如果当前有选中的卡牌，不执行高亮显示逻辑
        if (currentCard != null)
        {
            ClearCurrentHighlight();
            return;
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int gridPosition = CalculateGridPosition(mousePosition);

        // 检查目标位置是否有效，并且是上下左右或斜方向的位置
        if (actions > 0)
        {
            bool isAdjacentToMonster = IsAdjacentToMonster(position);

            if (IsValidPosition(gridPosition) && IsAdjacentOrDiagonal(gridPosition) && !IsBlockedByMonster(gridPosition) && gridPosition != position)
            {
                if (currentHighlight == null || CalculateGridPosition(currentHighlight.transform.position) != gridPosition)
                {
                    // 销毁旧的高亮对象
                    ClearCurrentHighlight();

                    // 调用 HighlightPosition 方法生成新的高亮对象
                    currentHighlight = HighlightPosition(gridPosition, true);
                }
            }
            else if (isAdjacentToMonster && IsValidPosition(gridPosition) && IsAdjacent(gridPosition))
            {
                ClearCurrentHighlight();
                currentHighlight = HighlightPosition(gridPosition, false);
            }
            else
            {
            // 如果鼠标移出了有效位置，移除高亮对象
                ClearCurrentHighlight();
            }
        }
    }

    void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0) && actions > 0) // 检查鼠标左键点击并且有可用的行动点
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int targetPosition = CalculateGridPosition(mousePosition);

            // 检查目标位置是否有效，并且是上下左右或斜方向的位置
            if (IsValidPosition(targetPosition) && IsAdjacentOrDiagonal(targetPosition))
            {
                
                ClearCurrentHighlight();
            }
        }   
    }

    private bool IsAdjacentToMonster(Vector2Int position)
    {
        Vector2Int[] adjacentPositions = {
            position + Vector2Int.up,
            position + Vector2Int.down,
            position + Vector2Int.left,
            position + Vector2Int.right
        };

        foreach (var adjacentPosition in adjacentPositions)
        {
            if (IsBlockedByMonster(adjacentPosition))
            {
                return true;
            }
        }

        return false;
    }   


    void ClearCurrentHighlight()
    {
        if (currentHighlight != null)
        {
            Destroy(currentHighlight);
            currentHighlight = null;
        }
    }


    void UpdatePosition()
    {
        transform.position = CalculateWorldPosition(position); // 更新棋子在场景中的位置
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldText();
    }

    public void AddArmor(int amount)
    {
        armor += amount;
        UpdateArmorText();
    }

    public void UpdateGoldText()
    {
        if (goldText != null)
        {
            goldText.text = "Gold: " + gold.ToString();
        }
    }

    public void ShowMoveOptions(Vector2Int[] directions, Card card)
    {
        ClearMoveHighlights();
        currentCard = card;

        foreach (var direction in directions)
        {
            Vector2Int newPosition = position + direction;
            if (IsValidPosition(newPosition))
            {
                //Debug.Log($"player valid Position: {newPosition}");
                HighlightPosition(newPosition, true);
            }
        }
    }

    public void ShowAttackOptions(Vector2Int[] directions, Card card)
    {
        ClearMoveHighlights();
        currentCard = card;

        foreach (var direction in directions)
        {
            Vector2Int newPosition = position + direction;
            if (IsValidPosition(newPosition))
            {
                HighlightPosition(newPosition, false);
            }
        }
    }

    public GameObject HighlightPosition(Vector2Int newPosition, bool isMove)
    {
        Vector3 highlightPosition = CalculateWorldPosition(newPosition);
        GameObject highlightPrefab = isMove ? moveHighlightPrefab : attackHighlightPrefab;
        GameObject highlight = Instantiate(highlightPrefab, highlightPosition, Quaternion.identity);

        if (!isMove)
        {
            highlight.layer = LayerMask.NameToLayer("AttackHighlight"); // 设置专属 Layer
        }
        else
        {
            highlight.layer = LayerMask.NameToLayer("MoveHighlight");
        }
        

        highlight.GetComponent<MoveHighlight>().Initialize(this, newPosition, isMove);
        moveHighlights.Add(highlight);
        return highlight;
    }

    public bool IsValidPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < boardSize && position.y >= 0 && position.y < boardSize;
    }

    private static bool IsBlockedByMonster(Vector2Int position)
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monsterObject in monsters)
        {
            Monster monster = monsterObject.GetComponent<Monster>();
            if (monster != null && monster.IsPartOfMonster(position))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsBlockedByLocation(Vector2Int position)
    {
        // 检查该位置是否被不可进入的位置占据
        return locationManager.IsNonEnterablePosition(position);
    }
    public bool IsBlockedBySomething(Vector2Int position)
    {
        return IsBlockedByMonster(position) || IsBlockedByLocation(position);
    }


    public void Move(Vector2Int newPosition)
    {
        Vector2Int oldPosition = position;
        position = newPosition;
        UpdatePosition();
        ClearMoveHighlights();
        
        // 增加本回合移动次数
        movesThisTurn++;
        
        // 记录移动指标
        if (GameMetrics.Instance != null)
        {
            int distance = Mathf.Abs(newPosition.x - oldPosition.x) + Mathf.Abs(newPosition.y - oldPosition.y);
            GameMetrics.Instance.RecordMovement(distance);
        }

        // 调用新方法来检查并处理 ActivatePoint 和 DeactivatePoint
        CheckAndHandlePoints(newPosition);

        ExecuteCurrentCard();
        // 移动完成后触发事件
        OnMoveComplete?.Invoke();
    }

    public void UpdateEnergyStatus()
    {
        if (energyStatusText != null)
        {
            // Update the text based on the isCharged status
            energyStatusText.text = isCharged ? "ON" : "OFF";
        }
        else
        {
            //Debug.LogError("Energy status text component is not assigned.");
        }
    }

    public void CheckAndHandlePoints(Vector2Int newPosition)
    {

        // 检查玩家是否移动到了ActivatePoint
        if (activatePointPositions.Contains(newPosition))
        {
            Charge();
            //Debug.Log("Player is now charged.");
        }

        // 检查玩家是否移动到了DeactivatePoint
        if (deactivatePointPositions.Contains(newPosition))
        {
            if (isCharged)
            {
                //Debug.Log("Player is at DeactivatePoint, triggering Exhaust.");
                Decharge();
            }
        }
        
        // 检查玩家是否接触了RitualPoint
        RitualPoint[] ritualPoints = FindObjectsOfType<RitualPoint>();
        foreach (RitualPoint ritualPoint in ritualPoints)
        {
            if (ritualPoint.position == newPosition)
            {
                ritualPoint.Interact();
                break;
            }
        }
    }

    public void Charge() {
        isCharged = true;
        UpdateEnergyStatus();
    }

    public void Decharge() {
        deckManager.Exhaust();
        isCharged = false;
        UpdateEnergyStatus();
    }




    public void Attack(Vector2Int attackPosition)
    {
        // 记录攻击方向：玩家当前坐标到目标格的向量（假设棋盘坐标以 Vector2Int 表示）
        lastAttackDirection = attackPosition - position;
        lastAttackSnapshot = attackPosition;
        targetAttackPosition = attackPosition;
        
        // 执行攻击（可能执行两次）
        int attackTimes = (currentCard != null && currentCard.cardType == CardType.Attack && nextWeaponCardDoubleUse) ? 2 : 1;
        
        for (int i = 0; i < attackTimes; i++)
        {
            Vector3 worldPosition = CalculateWorldPosition(attackPosition);
            GameObject effectInstance = Instantiate(attackEffectPrefab, worldPosition, Quaternion.identity);
            Destroy(effectInstance, 0.1f);
            
            // 基于坐标检测 Monster 的存在
            GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
            foreach (GameObject monsterObject in monsters)
            {
                Monster monster = monsterObject.GetComponent<Monster>();
                if (monster != null && monster.IsPartOfMonster(attackPosition))
                {
                    monster.TakeDamage(damage + damageModifierThisTurn);
                }
            }
            
            // 检查是否攻击Location
            Location[] locations = FindObjectsOfType<Location>();
            foreach (Location location in locations)
            {
                if (location.position == attackPosition)
                {
                    location.TakeDamage(damage + damageModifierThisTurn);
                }
            }
            
            if (i == 0 && attackTimes > 1)
            {
                Debug.Log($"BS06 effect: Attack {i + 1}/{attackTimes}");
            }
        }
        
        // BS06标记将在ExecuteCurrentCard中清除
        
        damage = 1;
        ClearMoveHighlights();
        ExecuteCurrentCard();
    }


    public void MultipleAttack(Vector2Int[] attackPositions)
    {
        // 基于坐标检测 Monster 的存在
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (Vector2Int attackPosition in attackPositions)
        {
            // 在每个攻击坐标生成攻击特效
            Vector3 worldPosition = CalculateWorldPosition(attackPosition);
            GameObject effectInstance = Instantiate(attackEffectPrefab, worldPosition, Quaternion.identity);
            Destroy(effectInstance, 0.1f);  // 根据动画时长调整销毁时间
            
            foreach (GameObject monsterObject in monsters)
            {
                Monster monster = monsterObject.GetComponent<Monster>();
                if (monster != null && monster.IsPartOfMonster(attackPosition))
                {
                    monster.TakeDamage(damage + damageModifierThisTurn);
                }
            }
        }
        ClearMoveHighlights();
        if (currentCard.cardType == CardType.Attack)
        {
            ExecuteCurrentCard();
        }
    }

    // 对于bookofpawn等等牌
    public void MultipleSpecial(Vector2Int[] attackPositions)
    {
        // 基于坐标检测 Monster 的存在
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (Vector2Int attackPosition in attackPositions)
        {
            // 在每个攻击坐标生成攻击特效
            Vector3 worldPosition = CalculateWorldPosition(attackPosition);
            GameObject effectInstance = Instantiate(attackEffectPrefab, worldPosition, Quaternion.identity);
            Destroy(effectInstance, 0.1f);  // 根据动画时长调整销毁时间
            
            foreach (GameObject monsterObject in monsters)
            {
                Monster monster = monsterObject.GetComponent<Monster>();
                if (monster != null && monster.IsPartOfMonster(attackPosition))
                {
                    monster.TakeDamage(damage + damageModifierThisTurn);
                }
            }
        }
        ClearMoveHighlights();
    }


    public void ClearMoveHighlights()
    {
        if (moveHighlights != null)
        {
            foreach (var highlight in moveHighlights)
            {
                Destroy(highlight);
            }
            moveHighlights.Clear();  // 清空列表
        }
    }

    public Vector3 CalculateWorldPosition(Vector2Int gridPosition)
    {
        // 计算世界坐标，仅考虑每个Tile的大小
        float x = (gridPosition.x + xshift) * cellSize.x + (cellSize.x / 2);
        float y = (gridPosition.y + yshift) * cellSize.y + (cellSize.y / 2);
        return new Vector3(x, y, 0);
    }

    public Vector2Int CalculateGridPosition(Vector3 worldPosition)
    {
        // 通过减去偏移量来修正网格坐标的计算
        int x = Mathf.FloorToInt((worldPosition.x) / cellSize.x - xshift );
        int y = Mathf.FloorToInt((worldPosition.y) / cellSize.y - yshift );

        return new Vector2Int(x, y);
    }


    public bool IsAdjacent(Vector2Int targetPosition)
    {
        Vector2Int delta = targetPosition - position;
        // 判断是否仅在水平方向或垂直方向上相邻
        return (Mathf.Abs(delta.x) == 1 && delta.y == 0) || (Mathf.Abs(delta.y) == 1 && delta.x == 0);
    }

    public bool IsAdjacentOrDiagonal(Vector2Int targetPosition)
    {
        Vector2Int delta = targetPosition - position;
        return Mathf.Abs(delta.x) <= 1 && Mathf.Abs(delta.y) <= 1;
    }

    public void ExecuteCurrentCard()
    {
        if (currentCard == null)
        {
            actions -= 1;
            if (actions > 0)
            {
                FindObjectOfType<TurnManager>().MoveCursor();
                FindObjectOfType<TurnManager>().UpdateActionText();
            } 
            // 推进回合
            if (actions == 0) 
            {
                FindObjectOfType<TurnManager>().UpdateActionText();   
                DisableNonQuickCardButtons();
                //添加回合条变成红色特效
            }  
        }
        if (currentCard != null)
        {
            // 记录卡牌使用指标
            if (GameMetrics.Instance != null)
            {
                GameMetrics.Instance.RecordCardPlayed(currentCard.Id, currentCard.cost);
            }
            
            deckManager.UseCard(currentCard);
            
            // 如果是攻击卡，执行 OnAttackExecuted 方法
            if (currentCard.cardType == CardType.Attack)
            {
                //Debug.Log("Executing OnAttackExecuted for: ");
                Vector2Int snapshot = targetAttackPosition;
                
                // 第一次执行OnCardExecuted
                currentCard.OnCardExecuted(lastAttackSnapshot);
                
                // 如果有BS06效果，再次执行OnCardExecuted
                if (nextWeaponCardDoubleUse)
                {
                    Debug.Log("BS06 effect: Executing OnCardExecuted twice");
                    currentCard.OnCardExecuted(lastAttackSnapshot);
                    nextWeaponCardDoubleUse = false; // 清除标记
                }
                
                targetAttackPosition = new Vector2Int(-1, -1);
            }

            if (currentCard.cardType == CardType.Move) // Assuming MovementCard is a class for movement cards
            {
                currentCard.OnCardExecuted();
                OnMoveCardUsed?.Invoke(currentCard); // 触发移动牌使用事件
                
                // 触发涌潮效果
                if (torrentStacks > 0)
                {
                    KeywordEffects.TriggerTorrentEffect(this, position);
                }
                
                // 触发逐浪效果
                if (waveRiderEffectActive)
                {
                    KeywordEffects.TriggerWaveRiderEffect(this, position);
                }
                
                // 触发 WS01 回响效果
                if (ws01EchoActive)
                {
                    KeywordEffects.TriggerWS01EchoEffect(this);
                }
                
                if (vineEffectActive)
                {
                    TriggerVineEffect();
                }
            }
            
            if (currentCard.cardType == CardType.Special)
            {
                currentCard.OnCardExecuted();
            }
            

            if (!currentCard.isQuick)
            {
                actions -= 1;
                if (actions > 0)
                {
                    FindObjectOfType<TurnManager>().MoveCursor();
                    FindObjectOfType<TurnManager>().UpdateActionText();
                }
            }
            if (currentCard.isPartner)
            {
                for (int i = 0; i < deckManager.deck.Count; i++)
                {
                    Card c = deckManager.deck[i];
                    if (c.isPartner)
                    {
                        deckManager.DrawCardAt(i); // 直接调用 DrawCardAt 方法来抽取特定位置的牌
                        break;
                    }
                }
            }

            // Notify listeners that a card has been played
            OnCardUsed(currentCard);
            currentCard = null;
            // 推进回合
            if (actions == 0) 
            {
                //ResetEffectsAtEndOfTurn();
                FindObjectOfType<TurnManager>().UpdateActionText();   
                DisableNonQuickCardButtons();
                //添加回合条变成红色特效
                
                // BS09某种草药效果：当失去所有行动点时触发
                KeywordEffects.TriggerHerbOnNoActions(this);
            }
        }
    }

    public void ResetEffectsAtEndOfTurn()
    {
        vineEffectActive = false; // Reset the vine effect after the turn
        cardsUsedThisTurn = 0;
        movesThisTurn = 0; // 重置移动次数
        damageModifierThisTurn = 0;
        KeywordEffects.ResetBSEffects(this); // 重置BS系列效果
        
        // 重置 WS01 效果
        ws01EchoActive = false;
    }
    
    public void IncreaseMaxActions(int amount)
    {
        maxActions += amount;
        Debug.Log($"Max actions increased by {amount}. New max: {maxActions}");
    }
    
    public void ApplyFuryDamage()
    {
        damageModifierThisTurn = furyStacks; // 将愤怒层数应用为伤害修正
    }
    
    public void AddFuryAndApply(int stacks)
    {
        furyStacks += stacks;
        damageModifierThisTurn = furyStacks; // 立即应用愤怒伤害加成
        Debug.Log($"Added {stacks} fury stacks. Total: {furyStacks}, damage modifier: {damageModifierThisTurn}");
    }
    
    public void ReduceFury(int stacks)
    {
        furyStacks = Mathf.Max(0, furyStacks - stacks);
        Debug.Log($"Reduced {stacks} fury stacks. Remaining: {furyStacks}");
    }
    
    public void AddFervent(int stacks)
    {
        ferventStacks += stacks;
        Debug.Log($"Added {stacks} fervent stacks. Total: {ferventStacks}");
    }

    public void OnCardUsed(Card currentCard)
    {
        cardsUsedThisTurn++;
        OnCardPlayed?.Invoke(currentCard); // Trigger the global event when a card is used
    }

    private void TriggerVineEffect()
    {
        Monster nearestMonster = monsterManager.FindNearestMonster(position, true);

        if (nearestMonster != null)
        {
            nearestMonster.TakeDamage(damage + damageModifierThisTurn);
            //Debug.Log("Vine effect triggered: Dealt 1 damage to " + nearestMonster.name);
        }
        else
        {
            //Debug.Log("No adjacent monsters to damage.");
        }
    }


    public void DisableNonQuickCardButtons()
    {
        // 获取所有 MonoBehaviour 并筛选出实现了 CardButton 接口的对象
        MonoBehaviour[] monoBehaviours = FindObjectsOfType<MonoBehaviour>();
    
        foreach (MonoBehaviour monoBehaviour in monoBehaviours)
        {
            CardButton cardButton = monoBehaviour as CardButton;
            if (cardButton != null)
            {
                // 获取按钮对应的卡牌
                Card card = cardButton.GetCard();
                if (card != null && !card.isQuick)
                {
                    // 禁用按钮
                    Button button = monoBehaviour.GetComponent<Button>();
                    if (button != null)
                    {
                        button.interactable = false;
                    }
                    // 禁用拖拽
                    CardButtonBase cardButtonBase = monoBehaviour as CardButtonBase;
                    if (cardButtonBase != null)
                    {
                        cardButtonBase.SetDraggable(false);
                    }
                    }
            }
        }

    }

    public void SetDeck(List<Card> deck)
    {
        this.deck = new List<string>();
        foreach (Card card in deck)
        {
            this.deck.Add(card.Id); // Assuming each Card has a `cardName` property
        }
    }

    public void DeselectCurrentCard()
    {
        currentCard = null;
        ClearMoveHighlights();
    }

    // 示例方法：读取卡组
    public List<string> GetDeckNames()
    {
        return new List<string>(deck);
    }

    // 示例方法：加载卡组
    public void LoadDeck(List<string> newDeck)
    {
        deck = new List<string>(newDeck);
        deckManager.UpdateDeckPanel(); 
        deckManager.RefreshCardReferences(this, monsterManager);
    }

    // 示例方法：设置玩家位置
    public void SetPosition(Vector2Int newPosition)
    {
        position = newPosition;
        UpdatePosition(); // 更新玩家prefab的世界位置
    }

    // 示例方法：设置玩家生命值
    public void SetHealth(int newHealth)
    {
        health = newHealth;
        UpdateHealthText();
    }

    public void SetArmor(int newArmor)
    {
        armor = newArmor;
        UpdateArmorText();
    }

    // 示例方法：设置金币
    public void SetGold(int newGold)
    {
        gold = newGold;
    }
    
    public void ShowDamageText(int damage, bool isHeal = false)
    {
        Vector3 worldPos = CalculateWorldPosition(position) + Vector3.up * 0.5f;
        DamageTextManager.ShowDamageText(this, damage, worldPos, damageTextPrefab, isHeal);
    }
}
