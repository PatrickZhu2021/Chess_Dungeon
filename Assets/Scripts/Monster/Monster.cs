using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 
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
    // 根据需要继续添加其他类型
}

public class Monster : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string monsterName = "default";
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


    public MonsterInfoManager infoManager;
    private List<GameObject> highlightInstances = new List<GameObject>();
    public GameObject highlightPrefab;  // 在 Inspector 中拖入 Highlight Prefab

    
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

        UpdatePosition();
    }

    public void UpdatePosition()
    {
        transform.position = player.CalculateWorldPosition(position);
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
        health -= damage;
        UpdateHealthBar();

        // 播放受伤动画
        if (animator != null)
        {
            //animator.SetTrigger("TakeDamage");
        }
        
        // 业力连接伤害共享（使用一次后消失）
        if (karmaLinkedMonster != null && karmaLinkedMonster.health > 0)
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
        
        if (player != null)
        {
            player.AddGold(10);
            
            // BS08盐袋效果：每个死亡的敌人获得1点护甲
            KeywordEffects.TriggerSaltBagOnEnemyDeath(player, monsterName);
        }
         if (monsterManager != null)
        {
            monsterManager.RemoveMonster(this);
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
        
        // 检查是否有瞩目目标
        if (currentLureTarget != null && currentLureTarget.HasLure())
        {
            MoveTowardsLureTarget();
        }
        else
        {
            PerformMovement();
        }
    }
    
    public virtual void PerformMovement()
    {
        // Default empty implementation, override in subclasses
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDying) return;
        Debug.Log($"Pointer entered monster: {monsterName}"); // 调试用日志
        if (infoManager != null)
        {
            // 调用 MonsterInfoManager 更新信息面板
            infoManager.UpdateMonsterInfo(monsterName, health, position);
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
        Debug.Log("Karma links cleared");
    }
    
    public bool HasKarmaLink()
    {
        return karmaLinkedMonster != null;
    }


}
