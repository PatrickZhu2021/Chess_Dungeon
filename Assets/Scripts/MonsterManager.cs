using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;



public class MonsterManager : MonoBehaviour
{
    public static Player Instance { get; private set; }
    public int boardSize = 8;
    public Vector3 cellSize = new Vector3(1, 1, 0); // 每个Tile的大小
    public Vector3 cellGap = new Vector3(0, 0, 0); // Cell Gap
    public bool nextlevel = false;

    public List<Monster> monsters = new List<Monster>(); // 改为public以支持动态添加
    private List<Scene> scenes = new List<Scene>();
    private List<GameObject> warnings = new List<GameObject>();
    private List<GameObject> pointObjects = new List<GameObject>();

    private int currentLevel = 1;
    private int totalMonstersToSpawn;
    private int totalMonstersKilled;

    public Player player; // 玩家对象
    public RewardManager rewardManager;
    private LocationManager locationManager;
    private List<LevelConfig> levelConfigs; // 关卡配置列表

    public bool isLevelCompleted = false;

    public Text levelCountText;

    void Awake()
    {
        
        // Initialize the player in Awake to ensure it is set before Start
        player = FindObjectOfType<Player>();

        rewardManager = FindObjectOfType<RewardManager>();
        locationManager = FindObjectOfType<LocationManager>();
        if (player == null)
        {
            Debug.LogError("Player object not found!");
        }
        


        rewardManager.OnRewardSelectionComplete += OnRewardSelectionComplete;


        LoadLevelConfigs();
        
    }

    void Start()
    {
        // 读取 `LevelNode` 存储的关卡索引
        int selectedLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        bool isLevelNode = PlayerPrefs.GetInt("IsLevelNode", 0) == 1;
        // 如果是从 LevelNode 进入，则使用 `selectedLevel`
        if (isLevelNode)
        {
            currentLevel = selectedLevel;

        }
        // 否则，尝试加载存档
        else if (SaveSystem.GameSaveExists())
        {
            GameData gameData = SaveSystem.LoadGame();
            currentLevel = gameData.currentLevel;

        }
        // 如果没有存档，就使用默认值
        else
        {
            currentLevel = 1;

            // **新游戏时，重置 HasEnergyCard**
            PlayerPrefs.SetInt("HasEnergyCard", 0);
            PlayerPrefs.Save();
        }

        // 开始游戏
        StartLevel(currentLevel);

    }

    void LoadLevelConfigs()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Configs", "levelConfig.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            GameConfig gameConfig = JsonUtility.FromJson<GameConfig>(json);
            levelConfigs = gameConfig.levels;
        }
        else
        {
            Debug.LogError("Level configuration file not found: " + filePath);
        }
        
    }

    private void UpdateLevelCountText()
    {
        levelCountText.text = "Level: " + currentLevel.ToString();
    }

    public void StartLevel(int level)
    {
        currentLevel = level;
        // 清除上一关的动态障碍物
        LocationManager locationManager = FindObjectOfType<LocationManager>();
        locationManager.ClearAllLocations();
        locationManager.ClearMonsterSpawnPositions();

        // 获取当前关卡配置并生成对应的地形
        LevelConfig levelConfig = levelConfigs.Find(l => l.levelNumber == level);
        if (levelConfig != null)
        {
            locationManager.SpawnLocationsForLevel(levelConfig.terrainType);
        }

        // 清空之前存储的位置数据
        player.activatePointPositions.Clear();
        player.deactivatePointPositions.Clear();

        player.isCharged = false;
        player.UpdateEnergyStatus();
        
        if (levelConfig == null)
        {
            Debug.LogError("Level configuration not found for level: " + level);
            return;
        }

        // 更新 UI 上的 LevelCount 文本
        UpdateLevelCountText();

        // 清除所有场景对象
        ClearAllMonsters();
        ClearAllScenes();
        ClearAllPoints();
        
        bool hasEnergyCard = PlayerPrefs.GetInt("HasEnergyCard", 0) == 1;
        if (hasEnergyCard)
        {
            Debug.Log("⚡ 从存档加载：检测到能量卡，生成 ActivatePoints...");
            SpawnActivatepointsForLevel();
        }
        //生成场景
        //locationManager.GenerateLocation("Forest", 5);

        //生成怪物
        totalMonstersToSpawn = levelConfig.monsterTypes.Count;
        totalMonstersKilled = 0;
        SpawnMonstersForLevel(levelConfig);

        

        //if (level > 1)
        //{
          //  rewardManager.OpenRewardPanel();
        //}
        
    }

    public int GetCurrentLevel() {
        return currentLevel;
    }

    private void OnRewardSelectionComplete()
    {
    // Actions to perform after reward selection is complete
    // For example, shuffle the deck, check for energy cards, draw cards, etc.

    // Check if there are any energy cards in the deck after the reward is added
        // rewardManager.isRewardPanelOpen = false;
        bool hasEnergyCard = player.deckManager.deck.Exists(card => card.isEnergy);

        PlayerPrefs.SetInt("HasEnergyCard", hasEnergyCard ? 1 : 0);
        PlayerPrefs.Save();
        
        isLevelCompleted = false;
    }
    public void ClearAllMonsters()
    {
        foreach (Monster monster in monsters)
        {
            if (monster != null)
            {
                Destroy(monster.gameObject); // Destroy the monster GameObject
            }
        }
        monsters.Clear(); // Clear the list of monsters
        Debug.Log("All monsters cleared.");
    }

    void ClearAllScenes()
    {
        foreach (Scene scene in scenes)
        {
            if (scene != null)
            {
                Destroy(scene.gameObject);
            }
        }
        scenes.Clear(); // 清空列表
    }

    void ClearAllPoints()
    {
        foreach (GameObject point in pointObjects)
        {
            Destroy(point);
        }
        pointObjects.Clear(); // 清空列表
    }



    void SpawnMonstersForLevel(LevelConfig levelConfig)
    {
        Debug.Log($"[Spawn] Level {levelConfig.levelNumber}: monsterTemplates count = {levelConfig.monsterTemplates.Count}, fallback monsterTypes count = {levelConfig.monsterTypes.Count}");
        // 如果有多套模板，随机选一套
        if (levelConfig.monsterTemplates != null 
            && levelConfig.monsterTemplates.Count > 0)
        {
            // 随机挑一个 MonsterTemplate
            int tplIndex = Random.Range(0, levelConfig.monsterTemplates.Count);
            MonsterTemplate chosenTpl = levelConfig.monsterTemplates[tplIndex];
            Debug.Log($"[Spawn] Using template #{tplIndex} for Level {levelConfig.levelNumber} with {chosenTpl.monsterTypes.Count} entries");
            
            // 如果template有独立地形配置，重新生成地形
            if (!string.IsNullOrEmpty(chosenTpl.terrainType))
            {
                locationManager.ClearAllLocations();
                locationManager.SpawnLocationsForLevel(chosenTpl.terrainType);
            }
            
            // 创建特殊生成区域的位置索引
            Dictionary<string, int> areaIndices = new Dictionary<string, int>();
            
            // 从 chosenTpl.monsterTypes 里拿到真正的 List<string>
            foreach (string monsterType in chosenTpl.monsterTypes)
            {
                Debug.Log($"[Spawn] -> Template spawning: {monsterType}");
                Monster monster = CreateMonsterByType(monsterType);
                if (monster != null)
                {
                    // 检查是否有特殊生成规则
                    SpecialSpawnRule rule = chosenTpl.specialSpawnRules?.Find(r => r.monsterType == monsterType);
                    if (rule != null && !string.IsNullOrEmpty(rule.spawnArea))
                    {
                        List<Vector2Int> areaPositions = locationManager.GetSpecialSpawnArea(rule.spawnArea);
                        if (!areaIndices.ContainsKey(rule.spawnArea)) areaIndices[rule.spawnArea] = 0;
                        
                        if (areaIndices[rule.spawnArea] < areaPositions.Count)
                        {
                            SpawnMonsterAtPosition(monster, areaPositions[areaIndices[rule.spawnArea]]);
                            areaIndices[rule.spawnArea]++;
                        }
                        else
                        {
                            SpawnMonster(monster);
                        }
                    }
                    else
                    {
                        SpawnMonster(monster);
                    }
                }
                else 
                    Debug.LogWarning($"[Spawn] !! CreateMonsterByType returned null for '{monsterType}'");
            }

            return;
        }
        else
        {
            // 回落：老逻辑，直接遍历 monsterTypes
            foreach (string monsterType in levelConfig.monsterTypes)
            {
                Monster monster = CreateMonsterByType(monsterType);
                if (monster != null)
                    SpawnMonster(monster);
            }
        }
    }

    void SpawnActivatepointsForLevel()
    {
        // 生成 6 个 ActivatePoint
        for (int i = 0; i < 6; i++)
        {
            Vector2Int position = GetEmptyPosition();
            Vector3 worldPosition = player.CalculateWorldPosition(position);
            if (position != new Vector2Int(-1, -1))
            {
                // 实例化 ActivatePoint 的预制体
                GameObject activatePointPrefab = Resources.Load<GameObject>("Prefabs/UI/ActivatePoint");
                if (activatePointPrefab != null)
                {
                    GameObject activatePointInstance = Instantiate(activatePointPrefab, new Vector3(worldPosition.x, worldPosition.y, 0), Quaternion.identity);
                    ActivatePoint activatePoint = activatePointInstance.GetComponent<ActivatePoint>();
                    if (activatePoint != null)
                    {
                        //activatePoint.Initialize(position);
                        
                    }
                    player.activatePointPositions.Add(position);
                    pointObjects.Add(activatePointInstance); // 添加到列表
                }
                else
                {
                    Debug.LogError("ActivatePoint prefab not found.");
                }
            }   
        }

        // 生成 12 个 DeactivatePoint
        for (int i = 0; i < 12; i++)
        {
            Vector2Int position = GetEmptyPosition();
            Vector3 worldPosition = player.CalculateWorldPosition(position);
            if (position != new Vector2Int(-1, -1))
            {
                // 实例化 DeactivatePoint 的预制体
                GameObject deactivatePointPrefab = Resources.Load<GameObject>("Prefabs/UI/DeactivatePoint");
                if (deactivatePointPrefab != null)
                {
                    GameObject deactivatePointInstance = Instantiate(deactivatePointPrefab, new Vector3(worldPosition.x, worldPosition.y, 0), Quaternion.identity);
                    DeactivatePoint deactivatePoint = deactivatePointInstance.GetComponent<DeactivatePoint>();
                    if (deactivatePoint != null)
                    {
                        //deactivatePoint.Initialize(position);
                        
                    }
                    player.deactivatePointPositions.Add(position); // 添加位置信息到列表中
                    pointObjects.Add(deactivatePointInstance); // 添加到列表
                }
                else
                {
                    Debug.LogError("DeactivatePoint prefab not found.");
                }
            }
        }
    }


    public Monster CreateMonsterByType(string type)
    {
        System.Type monsterType = System.Type.GetType(type);
        if (monsterType == null)
        {
            Debug.LogError($"Monster type not found: {type}");
            return null;
        }
        
        GameObject tempObj = new GameObject();
        Monster tempMonster = tempObj.AddComponent(monsterType) as Monster;
        GameObject prefab = tempMonster.GetPrefab();
        Destroy(tempObj);
        
        if (prefab != null)
        {
            GameObject monsterObject = Instantiate(prefab);
            return monsterObject.GetComponent<Monster>();
        }
        
        Debug.LogError($"Prefab not found for monster type: {type}");
        return null;
    }


    public void SpawnMonster(Monster monster)
    {
        Vector2Int spawnPosition = GetRandomPosition(monster);
        if (spawnPosition == new Vector2Int(-1, -1))
        {
            Debug.LogWarning("Failed to spawn monster: No valid position found.");
            Destroy(monster.gameObject);
            return;
        }

        Vector3 worldPosition = player.CalculateWorldPosition(spawnPosition);
        monster.transform.position = worldPosition;
        monster.Initialize(spawnPosition);
        monsters.Add(monster);
        Debug.Log($"{monster.GetType().Name} spawned at position: " + spawnPosition);
    }

    public void SpawnMonsterAtPosition(Monster monster, Vector2Int position)
    {
        if (!IsTileValid(position))
        {
            Debug.LogWarning($"Position {position} is not valid, falling back to random spawn.");
            SpawnMonster(monster);
            return;
        }

        Vector3 worldPosition = player.CalculateWorldPosition(position);
        monster.transform.position = worldPosition;
        monster.Initialize(position);
        monsters.Add(monster);
        Debug.Log($"{monster.GetType().Name} spawned at predefined position: " + position);
    }

    public void MoveMonsters()
    {
        StartCoroutine(MoveMonstersSequentially());
    }

    private IEnumerator MoveMonstersSequentially()
    {
        List<Monster> monstersCopy = new List<Monster>(monsters);
        foreach (Monster monster in monstersCopy)
        {
            if (monster != null)
            {
                monster.MoveTowardsPlayer();
                yield return new WaitForSeconds(0.1f);
            }
        }
        // 怪物回合结束时减少眩晕层数
        foreach (Monster monster in monsters)
        {
            if (monster != null && monster.stunnedStacks > 0)
            {
                monster.ReduceStun(1);
            }
        }
    }

    // Moves the given monster to the target grid position.
    public void MoveMonster(Monster monster, Vector2Int targetPosition)
    {
        if (monster == null)
        {
            Debug.LogError("MoveMonster called with a null monster.");
            return;
        }

        if (!monster.IsValidPosition(targetPosition))
        {
            Debug.Log("Target position " + targetPosition + " is invalid. Monster not moved.");
            return;
        }

        Vector2Int oldPosition = monster.position;
        monster.position = targetPosition;
        // Update the monster's world position using its own UpdatePosition() method
        monster.UpdatePosition();

        Debug.Log("Monster moved from " + oldPosition + " to " + targetPosition);
    }

    public void OnMonsterKilled()
    {
        totalMonstersKilled++;
        if (totalMonstersKilled >= totalMonstersToSpawn)
        {
            StartLevel(++currentLevel);
        }
    }
    //回合结束检查是否进入下一个回合/开始新关卡
    public void OnTurnEnd(int turnCount)
    {
        // 移除已被销毁的Monster对象
        monsters.RemoveAll(monster => monster == null);
        nextlevel = true;
        //如果关卡完成
        if (monsters.Count == 0)
        {
            isLevelCompleted = true; // 标记关卡完成
            // 订阅奖励完成事件
            //rewardManager.OnRewardSelectionComplete += HandleRewardFlowComplete;
            // 一次性启动卡牌→遗物奖励流程
            
            //视觉问题：可以等待一秒等所有卡牌洗回后再继续
            rewardManager.StartRewardProcess();

            player.deckManager.RestoreExhaustedCards();
            foreach (FireZone zone in locationManager.activeFireZones)
            {
                if (zone != null)
                {
                    zone.DestroySelf();
                }
            }
            Effects.KeywordEffects.StopBasicRitual();
            //StartLevel(++currentLevel);
        }
        else if (turnCount % 1 == 0 && monsters.Count != 0)
        {
            OnMonsterTurnStart();
            //MoveMonsters();
            Debug.Log("Monsters move.");
        }
    }

    public void OnMonsterTurnStart()
    {
        foreach (FireZone zone in locationManager.activeFireZones)
        {
            if (zone != null)
            {
                zone.OnEnemyTurnStart();
            }
        }
        MoveMonsters();
    }


    public Vector2Int GetRandomPosition(Monster monsterType)
    {
        Vector2Int playerPosition = player.position;
        Debug.Log("Player position: " + playerPosition);
        HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int> { playerPosition };
        // Add all occupied positions
        foreach (Monster monster in monsters)
        {
            occupiedPositions.UnionWith(monster.GetOccupiedPositions(monster.position));
        }

        // 从 LocationManager 获取不可进入位置
        HashSet<Vector2Int> nonEnterablePositions = new HashSet<Vector2Int>(FindObjectOfType<LocationManager>().GetNonEnterablePositions());

        Vector2Int randomPosition;
        List<Vector2Int> monsterParts;
        int attempts = 0;
        const int maxAttempts = 50;

        do
        {
            randomPosition = new Vector2Int(Random.Range(0, boardSize), Random.Range(0, boardSize));
            monsterParts = monsterType.GetOccupiedPositions(randomPosition);
            attempts++;
        
            if (attempts >= maxAttempts)
            {
                Debug.LogWarning("Failed to find a valid spawn position after 50 attempts.");
                return new Vector2Int(-1, -1); // Return an invalid position to indicate failure
            }
        } while (occupiedPositions.Overlaps(monsterParts) || 
         !AreAllPositionsValid(monsterParts) || 
         monsterParts.Contains(playerPosition) || 
         monsterParts.Exists(part => locationManager.IsNonEnterablePosition(part)) ||
         IsInAnySpecialSpawnArea(monsterParts));  // 避免在特殊区域内生成

        return randomPosition;
    }

    public Vector2Int GetEmptyPosition()
    {
        Vector2Int playerPosition = player.position;
        Debug.Log("Player position: " + playerPosition);
    
        // 获取所有被占据的位置（包括怪物和场景物体）
        HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int> { playerPosition };
    
        // 添加所有怪物占据的位置
        foreach (Monster monster in monsters)
        {
            occupiedPositions.UnionWith(monster.GetOccupiedPositions(monster.position));
        }

        // 添加场景物体占据的位置
        foreach (Scene scene in scenes)
        {
            occupiedPositions.UnionWith(scene.GetOccupiedPositions(scene.position));
        }

        //添加所有场景占据的位置
        if (locationManager != null)
        {
            occupiedPositions.UnionWith(locationManager.GetNonEnterablePositions());
        }

        Vector2Int randomPosition;
        int attempts = 0;
        const int maxAttempts = 50;

        do
        {
            randomPosition = new Vector2Int(Random.Range(0, boardSize), Random.Range(0, boardSize));
            attempts++;
        
            if (attempts >= maxAttempts)
            {
                Debug.LogWarning("Failed to find a valid empty position after 50 attempts.");
                return new Vector2Int(-1, -1); // Return an invalid position to indicate failure
            }
        } while (occupiedPositions.Contains(randomPosition));

        return randomPosition;
    }


    bool AreAllPositionsValid(List<Vector2Int> positions)
    {
        foreach (Vector2Int pos in positions)
        {
            if (pos.x < 0 || pos.x >= boardSize || pos.y < 0 || pos.y >= boardSize)
            {
                return false;
            }
        }
        return true;
    }

    public int GetMonsterCount()
    {
        return monsters.Count;
    }

    public Monster FindNearestMonster(Vector2Int playerPosition, bool isAdjacent = false)
    {
        Monster nearestMonster = null;
        float nearestDistance = float.MaxValue;

        foreach (Monster monster in monsters)
        {
            float distance = Vector2Int.Distance(playerPosition, monster.position);

            if (isAdjacent)
            {
                // If isAdjacent is true, we only care about monsters that are adjacent to the player
                if (Mathf.Abs(playerPosition.x - monster.position.x) <= 1 && Mathf.Abs(playerPosition.y - monster.position.y) <= 1)
                {
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestMonster = monster;
                    }
                }
            }
            else
            {
                // If isAdjacent is false, find the nearest monster regardless of adjacency
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestMonster = monster;
                }
            }
        }

        return nearestMonster;
    }


    public void RemoveMonster(Monster monster)
    {
        if (monsters.Contains(monster))
        {
            monsters.Remove(monster);
        }
    }
    
    public void TriggerLevelComplete()
    {
        isLevelCompleted = true;
        rewardManager.StartRewardProcess();
        player.deckManager.RestoreExhaustedCards();
        
        // 清理火域
        foreach (FireZone zone in locationManager.activeFireZones)
        {
            if (zone != null)
            {
                zone.DestroySelf();
            }
        }
        
        // 停止仪式
        Effects.KeywordEffects.StopBasicRitual();
        
        Debug.Log("Level completed immediately after all enemies defeated!");
    }

    private bool IsInAnySpecialSpawnArea(List<Vector2Int> positions)
    {
        List<Vector2Int> allSpecialPositions = locationManager.GetMonsterSpawnPositions();
        return positions.Exists(pos => allSpecialPositions.Contains(pos));
    }
    
    public bool IsTileValid(Vector2Int tilePosition)
    {
        // Check board boundaries (assuming boardSize is an integer representing board width/height)
        if (tilePosition.x < 0 || tilePosition.x >= boardSize ||
            tilePosition.y < 0 || tilePosition.y >= boardSize)
        {
            return false;
        }

        // Build the set of occupied positions:
        HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int>
        {
            player.position  // Player occupies its position
        };

        // Add all monster-occupied positions
        foreach (Monster monster in monsters)
        {
            occupiedPositions.UnionWith(monster.GetOccupiedPositions(monster.position));
        }

        // Add all scene object occupied positions
        foreach (Scene scene in scenes)
        {
            occupiedPositions.UnionWith(scene.GetOccupiedPositions(scene.position));
        }

        // Add additional non-enterable positions from the location manager
        if (locationManager != null)
        {
            occupiedPositions.UnionWith(locationManager.GetNonEnterablePositions());
        }

        // The tile is valid if it is not in the set of occupied positions.
        return !occupiedPositions.Contains(tilePosition);
    }

}
