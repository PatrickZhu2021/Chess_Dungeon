using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class TurnMetrics
{
    public int turnNumber;
    public float turnDuration;
    public int movesUsed;
    public int movesAvailable;
    public int cardsPlayed;
    public int cardsInHand;
    public float avgCardCost;
    public int damageDealt;
    public int damageTaken;
    public int tilesMovedThisTurn;
    public int enemiesHit;
    public int attacksLaunched;
    public Vector2Int playerPosition;
    public int adjacentThreats;
    public List<string> cardsPlayedThisTurn = new List<string>();
}

[System.Serializable]
public class SessionMetrics
{
    public string sessionId;
    public DateTime startTime;
    public DateTime endTime;
    public int totalTurns;
    public bool levelCompleted;
    public int finalPlayerHP;
    public int maxPlayerHP;
    public List<TurnMetrics> turns = new List<TurnMetrics>();
    
    // Aggregate metrics
    public float avgMoveUtilRate;
    public float avgCardPlayRate;
    public float avgDamagePerTurn;
    public float avgDamagePerMove;
    public int totalDamageDealt;
    public int totalDamageTaken;
    public int totalTilesMoved;
}

[System.Serializable]
public class SessionSummary
{
    public string sessionId;
    public DateTime endTime;
    public bool levelCompleted;
    public int totalTurns;
    public float avgMoveUtilRate;
    public float avgCardPlayRate;
    public float avgDamagePerTurn;
    public float avgDamagePerMove;
    public int totalDamageDealt;
    public int totalDamageTaken;
    public int totalTilesMoved;
}

[System.Serializable]
public class GameMetricsSummary
{
    public List<SessionSummary> sessions = new List<SessionSummary>();
}

public class GameMetrics : MonoBehaviour
{
    public static GameMetrics Instance { get; private set; }
    
    private SessionMetrics currentSession;
    private TurnMetrics currentTurn;
    private float turnStartTime;
    private Player player;
    private TurnManager turnManager;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSession();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        player = FindObjectOfType<Player>();
        turnManager = FindObjectOfType<TurnManager>();
    }
    
    private void InitializeSession()
    {
        currentSession = new SessionMetrics
        {
            sessionId = System.Guid.NewGuid().ToString(),
            startTime = DateTime.Now
        };
    }
    
    public void StartTurn(int turnNumber)
    {
        currentTurn = new TurnMetrics
        {
            turnNumber = turnNumber,
            movesAvailable = player != null ? player.actions : 3
        };
        turnStartTime = Time.time;
    }
    
    public void EndTurn()
    {
        if (currentTurn == null) return;
        
        // 在弃牌前收集手牌数据
        currentTurn.cardsInHand = GetHandSize();
        
        currentTurn.turnDuration = Time.time - turnStartTime;
        int currentActions = player != null ? player.actions : 0;
        currentTurn.movesUsed = currentTurn.movesAvailable - currentActions;
        
        // 计算avgCardCost为使用的行动点数/使用的卡牌数
        if (currentTurn.cardsPlayed > 0)
        {
            currentTurn.avgCardCost = (float)currentTurn.movesUsed / currentTurn.cardsPlayed;
        }
        else
        {
            currentTurn.avgCardCost = 0f;
        }
        
        currentTurn.playerPosition = player != null ? player.position : Vector2Int.zero;
        currentTurn.adjacentThreats = CountAdjacentThreats();
        
        currentSession.turns.Add(currentTurn);
        currentSession.totalTurns++;
        
        Debug.Log($"Turn ended - Cards in hand: {currentTurn.cardsInHand}, Used: {currentTurn.movesUsed}, Cards played: {currentTurn.cardsPlayed}");
        
        currentTurn = null;
    }
    
    public void RecordCardPlayed(string cardName, int cost)
    {
        if (currentTurn == null) return;
        
        currentTurn.cardsPlayed++;
        currentTurn.cardsPlayedThisTurn.Add(cardName);
        // avgCardCost现在表示使用的行动点数/使用的卡牌数
        // 在EndTurn时计算
    }
    
    public void RecordDamageDealt(int damage, int enemiesHit = 1)
    {
        if (currentTurn == null) return;
        
        currentTurn.damageDealt += damage;
        currentTurn.enemiesHit += enemiesHit;
        currentTurn.attacksLaunched++;
    }
    
    public void RecordDamageTaken(int damage)
    {
        if (currentTurn == null) return;
        currentTurn.damageTaken += damage;
    }
    
    public void RecordMovement(int tilesMoved)
    {
        if (currentTurn == null) return;
        currentTurn.tilesMovedThisTurn += tilesMoved;
    }
    
    public void EndSession(bool completed = false)
    {
        currentSession.endTime = DateTime.Now;
        currentSession.levelCompleted = completed;
        currentSession.finalPlayerHP = player != null ? player.health : 0;
        currentSession.maxPlayerHP = player != null ? 3 : 0; // Player默认最大生命值
        
        CalculateAggregateMetrics();
        SaveSessionData();
    }
    
    private void CalculateAggregateMetrics()
    {
        if (currentSession.turns.Count == 0) return;
        
        float totalMoveUtil = 0f;
        float totalCardPlayRate = 0f;
        int totalDamage = 0;
        int totalDamageTaken = 0;
        int totalMoves = 0;
        int totalTilesMoved = 0;
        
        foreach (var turn in currentSession.turns)
        {
            totalMoveUtil += turn.movesAvailable > 0 ? (float)turn.movesUsed / turn.movesAvailable : 0f;
            totalCardPlayRate += turn.cardsInHand > 0 ? (float)turn.cardsPlayed / turn.cardsInHand : 0f;
            totalDamage += turn.damageDealt;
            totalDamageTaken += turn.damageTaken;
            totalMoves += turn.movesUsed;
            totalTilesMoved += turn.tilesMovedThisTurn;
        }
        
        int turnCount = currentSession.turns.Count;
        currentSession.avgMoveUtilRate = totalMoveUtil / turnCount;
        currentSession.avgCardPlayRate = totalCardPlayRate / turnCount;
        currentSession.avgDamagePerTurn = (float)totalDamage / turnCount;
        currentSession.avgDamagePerMove = totalMoves > 0 ? (float)totalDamage / totalMoves : 0f;
        currentSession.totalDamageDealt = totalDamage;
        currentSession.totalDamageTaken = totalDamageTaken;
        currentSession.totalTilesMoved = totalTilesMoved;
    }
    
    private void SaveSessionData()
    {
        // 保存详细的session数据
        string json = JsonUtility.ToJson(currentSession, true);
        string fileName = $"GameMetrics_{currentSession.sessionId}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        
        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log($"Game metrics saved to: {filePath}");
            
            // 更新主文档
            UpdateSummaryFile();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save metrics: {e.Message}");
        }
    }
    
    private void UpdateSummaryFile()
    {
        string summaryPath = Path.Combine(Application.persistentDataPath, "GameMetricsSummary.json");
        GameMetricsSummary summary;
        
        // 读取现有的主文档
        if (File.Exists(summaryPath))
        {
            try
            {
                string existingJson = File.ReadAllText(summaryPath);
                summary = JsonUtility.FromJson<GameMetricsSummary>(existingJson);
            }
            catch
            {
                summary = new GameMetricsSummary();
            }
        }
        else
        {
            summary = new GameMetricsSummary();
        }
        
        // 检查是否已存在该session，如果存在则更新，否则添加
        SessionSummary existingSession = summary.sessions.Find(s => s.sessionId == currentSession.sessionId);
        
        SessionSummary sessionSummary = new SessionSummary
        {
            sessionId = currentSession.sessionId,
            endTime = currentSession.endTime,
            levelCompleted = currentSession.levelCompleted,
            totalTurns = currentSession.totalTurns,
            avgMoveUtilRate = currentSession.avgMoveUtilRate,
            avgCardPlayRate = currentSession.avgCardPlayRate,
            avgDamagePerTurn = currentSession.avgDamagePerTurn,
            avgDamagePerMove = currentSession.avgDamagePerMove,
            totalDamageDealt = currentSession.totalDamageDealt,
            totalDamageTaken = currentSession.totalDamageTaken,
            totalTilesMoved = currentSession.totalTilesMoved
        };
        
        if (existingSession != null)
        {
            // 更新现有记录
            int index = summary.sessions.IndexOf(existingSession);
            summary.sessions[index] = sessionSummary;
        }
        else
        {
            // 添加新记录
            summary.sessions.Add(sessionSummary);
        }
        
        // 保存更新后的主文档
        try
        {
            string summaryJson = JsonUtility.ToJson(summary, true);
            File.WriteAllText(summaryPath, summaryJson);
            Debug.Log($"Summary updated: {summaryPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to update summary: {e.Message}");
        }
    }
    
    [ContextMenu("Show Data Path")]
    public void ShowDataPath()
    {
        Debug.Log($"Game metrics save location: {Application.persistentDataPath}");
    }
    
    private int GetHandSize()
    {
        DeckManager deckManager = FindObjectOfType<DeckManager>();
        return deckManager != null ? deckManager.hand.Count : 0;
    }
    
    private int CountAdjacentThreats()
    {
        if (player == null) return 0;
        
        int threats = 0;
        Vector2Int[] directions = {
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(0, 1), new Vector2Int(0, -1),
            new Vector2Int(1, 1), new Vector2Int(-1, 1),
            new Vector2Int(1, -1), new Vector2Int(-1, -1)
        };
        
        foreach (var dir in directions)
        {
            Vector2Int checkPos = player.position + dir;
            if (IsPositionThreatened(checkPos))
                threats++;
        }
        
        return threats;
    }
    
    private bool IsPositionThreatened(Vector2Int position)
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monsterObj in monsters)
        {
            Monster monster = monsterObj.GetComponent<Monster>();
            if (monster != null && monster.health > 0)
            {
                var possibleMoves = monster.CalculatePossibleMoves();
                if (possibleMoves.Contains(position))
                    return true;
            }
        }
        return false;
    }
}