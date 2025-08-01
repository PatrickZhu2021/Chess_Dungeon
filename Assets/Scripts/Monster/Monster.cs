using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using Effects;
public enum MonsterType
{
    Pawn,
    Knight,
    Rook,
    Bishop,
    Queen,
    King,
    Beast,
    // 根据需要继续添加其他类型
}

public class Monster : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string monsterName = "default";
    public string displayName = ""; // 显示名称，默认为空
    private Animator animator;
    public int health;
    public int maxHealth;
    public float pieceValue;
    public MonsterType type;
    public SpriteRenderer healthFillRenderer;  // 健康条的 SpriteRenderer
    public Vector2Int position;
    public GameObject healthBarPrefab;  // Prefab 引用
    private GameObject healthBarInstance;
    private Image healthFill;  // 引用填充的红色条
    public Player player;
    private MonsterManager monsterManager;
    private LocationManager locationManager;

    private bool isDying = false;
    public Vector2Int lastRelativePosition;
    public int stunnedStacks = 0; // 眩晕层数
    public int lureStacks = 0; // 瞩目层数
    public static Monster currentLureTarget = null; // 当前瞩目目标
    public Monster karmaLinkedMonster = null; // 业力连接的怪物
    public static List<Monster> karmaLinkedPair = new List<Monster>(); // 业力连接对
    public static GameObject karmaLinkLine = null; // 业力连接线


    public MonsterInfoManager infoManager;
    private List<GameObject> highlightInstances = new List<GameObject>();
    public GameObject highlightPrefab;  // 在 Inspector 中拖入 Highlight Prefab
    public GameObject damageTextPrefab;  // 伤害数字预制体

    
    public virtual void Initialize(Vector2Int startPos)
    {
        maxHealth = health;
        position = startPos;
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogError("Player object not found! Make sure a Player is present in the scene.");
            return;
        }
        monsterManager = FindObjectOfType<MonsterManager>();
        infoManager = FindObjectOfType<MonsterInfoManager>();
        locationManager = FindObjectOfType<LocationManager>();
        animator = GetComponent<Animator>(); 

        // 实例化血量条并设置其位置
        GameObject healthBarPrefab = Resources.Load<GameObject>("Prefabs/UI/HealthBar");
        healthBarInstance = Instantiate(healthBarPrefab, transform);
        healthBarInstance.transform.localPosition = new Vector3(0, -0.0f, 0);  // 在底部稍微偏移
        healthFillRenderer = healthBarInstance.transform.Find("fill").GetComponent<SpriteRenderer>();
        UpdateHealthBar();
        
        // 加载伤害文本预制体
        if (damageTextPrefab == null)
        {
            damageTextPrefab = Resources.Load<GameObject>("Prefabs/UI/DamageText");
            Debug.Log($"Loaded damageTextPrefab from Resources: {(damageTextPrefab != null ? "Success" : "Failed")}");
        }

        UpdatePosition();
    }

    public void UpdatePosition()
    {
        transform.position = player.CalculateWorldPosition(position);
        
        // 更新业力连接线位置
        UpdateKarmaLinkLine();
    }

    public virtual List<Vector2Int> CalculatePossibleMoves()
    {
        // 默认实现：简单地计算怪物周围的 1 格可移动位置（可以根据需要定制复杂逻辑）
        List<Vector2Int> possibleMoves = new List<Vector2Int>
        {
            position + new Vector2Int(1, 0),
            position + new Vector2Int(-1, 0),
            position + new Vector2Int(0, 1),
            position + new Vector2Int(0, -1)
        };

        // 过滤掉无效位置或被占据的位置
        possibleMoves.RemoveAll(pos => !IsValidPosition(pos) || IsPositionOccupied(pos));
        return possibleMoves;
    }

    public virtual void TakeDamage(int damage)
    {
        // 记录造成伤害指标
        if (GameMetrics.Instance != null && damage > 0)
        {
            GameMetrics.Instance.RecordDamageDealt(damage, 1);
        }
        
        health -= damage;
        UpdateHealthBar();
        
        // 显示伤害数字
        ShowDamageText(damage, false);

        // 播放受伤动画
        if (animator != null)
        {
            //animator.SetTrigger("TakeDamage");
        }
        
        // 业力连接伤害共享（只有受到大于0的伤害时才解除连接）
        if (damage > 0 && karmaLinkedMonster != null && karmaLinkedMonster.health > 0)
        {
            Monster linkedMonster = karmaLinkedMonster;
            ClearKarmaLinks(); // 立即清除连接防止递归
            
            linkedMonster.TakeDamage(damage);
            Debug.Log($"Karma link: {linkedMonster.monsterName} also takes {damage} damage (link consumed)");
        }

        if (health <= 0)
        {
            Die();
        }
    }


    private void UpdateHealthBar()
    {
        if (healthFillRenderer != null)
        {
            float healthRatio = (float)health / maxHealth;

            // 调整 X 轴缩放比例以更新血条长度
            healthFillRenderer.transform.localScale = new Vector3(healthRatio, 1, 1);
        }
    }


    public virtual void Die()
    {
        // 清除业力连接（在死亡前清除以避免递归）
        if (HasKarmaLink())
        {
            ClearKarmaLinks();
            Debug.Log($"{monsterName} died, karma link cleared");
        }
        
        // 清除瞩目状态
        if (currentLureTarget == this)
        {
            currentLureTarget = null;
            Debug.Log($"{monsterName} died, lure target cleared");
        }
        
        if (player != null)
        {
            player.AddGold(10);
            
            // BS08盐袋效果：每个死亡的敌人获得1点护甲
            KeywordEffects.TriggerSaltBagOnEnemyDeath(player, monsterName);
        }
         if (monsterManager != null)
        {
            monsterManager.RemoveMonster(this);
            
            // 检查是否所有敌人都被消灭
            if (monsterManager.GetMonsterCount() == 0)
            {
                Debug.Log("All enemies defeated! Triggering reward selection.");
                monsterManager.TriggerLevelComplete();
            }
        }

        if (animator != null)
        {
            Debug.Log("DIEEEEEEEEEEEEEEE");
            animator.SetTrigger("Die");  // 触发死亡动画
        } 
        isDying = true;  // 设置死亡状态
        Destroy(healthBarInstance);
        //棋子打出不显示可移动位置死亡时间延迟bug
        Destroy(gameObject, 0.6f);
    }

    public virtual void MoveTowardsPlayer()
    {
        if (IsStunned())
        {
            Debug.Log($"{monsterName} is stunned and cannot move");
            return;
        }
        
        // 检查脚下是否有潮沼，移除所有脚下的潮沼
        bool trappedByMire = false;
        if (locationManager != null)
        {
            for (int i = locationManager.activeMires.Count - 1; i >= 0; i--)
            {
                MireLocation mire = locationManager.activeMires[i];
                if (mire != null && mire.position == position)
                {
                    Debug.Log($"Monster {monsterName} trapped by mire at {position}");
                    
                    // 移除潮沼
                    locationManager.activeMires.RemoveAt(i);
                    locationManager.RemoveLocation(mire);
                    Destroy(mire.gameObject);
                    
                    trappedByMire = true;
                }
            }
        }
        
        if (trappedByMire)
        {
            return; // 被潮沼阻止，不移动
        }
        
        PerformMovement();
    }
    
    public virtual void PerformMovement()
    {
        Vector2Int targetPos;
        
        // 检查是否有瞩目目标（不能是自己）
        if (currentLureTarget != null && currentLureTarget != this && currentLureTarget.HasLure())
        {
            targetPos = currentLureTarget.position;
            Debug.Log($"{monsterName} targeting lure at {targetPos}");
            
            // 每影响一名敌方棋子则减少1层瞩目
            currentLureTarget.ReduceLure(1);
        }
        else
        {
            targetPos = player.position;
            Debug.Log($"{monsterName} targeting player at {targetPos}");
        }
        
        PerformMovement(targetPos);
    }
    
    public virtual void PerformMovement(Vector2Int targetPosition)
    {
        // Default empty implementation, override in subclasses
        Debug.Log($"{monsterName} performing movement towards {targetPosition} (base implementation)");
    }
    


    public bool IsPositionOccupied(Vector2Int checkPosition)
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monster in monsters)
        {
            Monster otherMonster = monster.GetComponent<Monster>();
            if (otherMonster != null && otherMonster != this && otherMonster.IsPartOfMonster(checkPosition))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsValidPosition(Vector2Int position)
    {
        foreach (Vector2Int pos in GetOccupiedPositions(position)) {
            if (position.x < 0 || position.x >= player.boardSize || position.y < 0 || position.y >= player.boardSize )
            {
                return false;
            }
        }

        // 检查位置是否是不可进入的区域
        if (locationManager != null && locationManager.IsNonEnterablePosition(position))
        {
            Debug.Log($"{monsterName} cannot move to {position} - blocked by location");
            return false;
        }

        return true;
    }

    public virtual GameObject GetPrefab()
    {
        return null; // Return null or a default prefab if you have one
    }

    public virtual bool IsPartOfMonster(Vector2Int position)
    {
        return this.position == position;
    }

    public virtual List<Vector2Int> GetOccupiedPositions(Vector2Int position)
    {
        return new List<Vector2Int> { position }; // Default to single-tile monster
    }
    
    public virtual List<string> GetSkills()
    {
        return new List<string>(); // Default: no skills
    }
    
    public string GetDisplayName()
    {
        return string.IsNullOrEmpty(displayName) ? monsterName : displayName;
    }
    
    public virtual int GetAttackDamage()
    {
        return 1; // 默认伤害为1
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDying) return;
        Debug.Log($"Pointer entered monster: {monsterName}"); // 调试用日志
        if (infoManager != null)
        {
            // 调用 MonsterInfoManager 更新信息面板
            infoManager.UpdateMonsterInfo(monsterName, health, position, this);
        }
        HighlightPath();
    }

    // 鼠标移出事件
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"Pointer exited monster: {monsterName}"); // 调试用日志
        if (infoManager != null)
        {
            // 调用 MonsterInfoManager 隐藏信息面板
            infoManager.HideMonsterInfo();
        }
        ClearHighlight();
    }

    public virtual void HighlightPath()
    {
        // 清除之前的高亮
        ClearHighlight();

        // 检查highlightPrefab是否存在
        if (highlightPrefab == null)
        {
            highlightPrefab = Resources.Load<GameObject>("Prefabs/UI/warning");
            if (highlightPrefab == null)
            {
                Debug.LogWarning($"{monsterName}: highlightPrefab not found, skipping highlight");
                return;
            }
        }

        // 获取合法的移动路径
        List<Vector2Int> possibleMoves = CalculatePossibleMoves();

        // 在每个合法位置生成高亮对象
        foreach (Vector2Int move in possibleMoves)
        {
            Vector3 worldPos = player.CalculateWorldPosition(move);
            GameObject highlightInstance = Instantiate(highlightPrefab, worldPos, Quaternion.identity);
            highlightInstances.Add(highlightInstance);
        }
    }

    public void ClearHighlight()
    {
        foreach (GameObject highlight in highlightInstances)
        {
            Destroy(highlight);
        }
        highlightInstances.Clear();
    }

    public void OnDeathAnimationComplete()
    {
        // 动画结束后执行的逻辑
        Destroy(gameObject);  // 销毁怪物
    }
    
    public void AddStun(int stacks)
    {
        stunnedStacks += stacks;
        Debug.Log($"{monsterName} stunned for {stacks} stacks. Total: {stunnedStacks}");
    }
    
    public void ReduceStun(int stacks)
    {
        stunnedStacks = Mathf.Max(0, stunnedStacks - stacks);
        Debug.Log($"{monsterName} stun reduced by {stacks}. Remaining: {stunnedStacks}");
    }
    
    public bool IsStunned()
    {
        return stunnedStacks > 0;
    }
    
    public void AddLure(int stacks)
    {
        // 同时仅能有1名敌方棋子持有瞩目
        if (currentLureTarget != null && currentLureTarget != this)
        {
            currentLureTarget.lureStacks = 0;
        }
        
        lureStacks = stacks;
        currentLureTarget = this;
        Debug.Log($"{monsterName} gained {stacks} lure stacks");
    }
    
    public void ReduceLure(int stacks)
    {
        lureStacks = Mathf.Max(0, lureStacks - stacks);
        Debug.Log($"{monsterName} lure reduced by {stacks}. Remaining: {lureStacks}");
        
        if (lureStacks <= 0 && currentLureTarget == this)
        {
            currentLureTarget = null;
        }
    }
    
    public bool HasLure()
    {
        return lureStacks > 0;
    }
    
    protected Vector2Int GetTargetPosition()
    {
        // 检查是否有瞩目目标（不能是自己）
        if (currentLureTarget != null && currentLureTarget != this && currentLureTarget.HasLure())
        {
            Vector2Int lurePos = currentLureTarget.position;
            // 每影响一名敌方棋子则减少1层瞩目
            currentLureTarget.ReduceLure(1);
            Debug.Log($"{monsterName} targeting lure at {lurePos}");
            return lurePos;
        }
        else
        {
            Debug.Log($"{monsterName} targeting player at {player.position}");
            return player.position;
        }
    }
    
    private void MoveTowardsLureTarget()
    {
        if (currentLureTarget == null || !currentLureTarget.HasLure())
        {
            PerformMovement(); // 回退到正常移动
            return;
        }
        
        Vector2Int targetPos = currentLureTarget.position;
        Vector2Int direction = new Vector2Int(
            targetPos.x > position.x ? 1 : (targetPos.x < position.x ? -1 : 0),
            targetPos.y > position.y ? 1 : (targetPos.y < position.y ? -1 : 0)
        );
        
        Vector2Int newPosition = position + direction;
        
        if (IsValidPosition(newPosition) && !IsPositionOccupied(newPosition))
        {
            position = newPosition;
            UpdatePosition();
            
            // 每影响一名敌方棋子则减少1层瞩目
            currentLureTarget.ReduceLure(1);
            Debug.Log($"{monsterName} moved towards lure target at {targetPos}");
        }
        else
        {
            Debug.Log($"{monsterName} cannot move towards lure target - path blocked");
        }
    }
    
    public static void CreateKarmaLink(Monster monster1, Monster monster2)
    {
        // 清除之前的连接
        ClearKarmaLinks();
        
        // 建立新连接
        monster1.karmaLinkedMonster = monster2;
        monster2.karmaLinkedMonster = monster1;
        
        karmaLinkedPair.Clear();
        karmaLinkedPair.Add(monster1);
        karmaLinkedPair.Add(monster2);
        
        // 创建蓝色连接线
        CreateKarmaLinkLine(monster1, monster2);
        
        Debug.Log($"Karma link created between {monster1.monsterName} and {monster2.monsterName}");
    }
    
    public static void ClearKarmaLinks()
    {
        foreach (Monster monster in karmaLinkedPair)
        {
            if (monster != null)
            {
                monster.karmaLinkedMonster = null;
            }
        }
        karmaLinkedPair.Clear();
        
        // 销毁连接线
        if (karmaLinkLine != null)
        {
            Object.Destroy(karmaLinkLine);
            karmaLinkLine = null;
        }
        
        Debug.Log("Karma links cleared");
    }
    
    public bool HasKarmaLink()
    {
        return karmaLinkedMonster != null;
    }
    
    private static void CreateKarmaLinkLine(Monster monster1, Monster monster2)
    {
        // 创建连接线对象
        karmaLinkLine = new GameObject("KarmaLinkLine");
        
        // 添加LineRenderer组件
        LineRenderer lineRenderer = karmaLinkLine.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, monster1.transform.position);
        lineRenderer.SetPosition(1, monster2.transform.position);
        lineRenderer.startColor = lineRenderer.endColor = Color.blue;
        lineRenderer.widthMultiplier = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.useWorldSpace = true;
        lineRenderer.sortingLayerName = "UI";
        lineRenderer.sortingOrder = 10;
        
        Debug.Log($"Karma link line created between {monster1.monsterName} and {monster2.monsterName}");
    }
    
    public void ShowDamageText(int damage, bool isHeal = false)
    {
        if (player == null) return;
        
        GameObject prefabToUse = damageTextPrefab;
        if (prefabToUse == null && player != null)
        {
            prefabToUse = player.damageTextPrefab;
        }
        
        Vector3 worldPos = player.CalculateWorldPosition(position) + Vector3.up * 0.5f;
        DamageTextManager.ShowDamageText(this, damage, worldPos, prefabToUse, isHeal);
    }
    
    private static void UpdateKarmaLinkLine()
    {
        if (karmaLinkLine != null && karmaLinkedPair.Count == 2)
        {
            Monster monster1 = karmaLinkedPair[0];
            Monster monster2 = karmaLinkedPair[1];
            
            if (monster1 != null && monster2 != null)
            {
                LineRenderer lineRenderer = karmaLinkLine.GetComponent<LineRenderer>();
                if (lineRenderer != null)
                {
                    lineRenderer.SetPosition(0, monster1.transform.position);
                    lineRenderer.SetPosition(1, monster2.transform.position);
                }
            }
        }
    }


}
