using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class LocationManager : MonoBehaviour
{
    private List<Vector2Int> nonEnterablePositions = new List<Vector2Int>();
    private List<Vector2Int> EnterablePositions = new List<Vector2Int>();
    private Dictionary<string, GameObject> locationPrefabs = new Dictionary<string, GameObject>();
    private List<GameObject> spawnedLocations = new List<GameObject>();  // 动态生成的障碍物

    public List<FirePoint> activeFirePoints = new List<FirePoint>();
    public List<FireZone> activeFireZones = new List<FireZone>(); // 火域列表
    public List<BarrierLocation> activeBarriers = new List<BarrierLocation>(); // 拒障列表
    public AnchorLocation activeAnchor = null; // 当前锚点（同时只能存在1个）
    public List<MireLocation> activeMires = new List<MireLocation>(); // 潮沼列表
    private List<Vector2Int> waterPositions = new List<Vector2Int>(); // 水单元格位置
    private Dictionary<string, List<Vector2Int>> specialSpawnAreas = new Dictionary<string, List<Vector2Int>>(); // 特殊生成区域
    private Player player;
    private List<TerrainConfig> terrainConfigs = new List<TerrainConfig>();
    
    // DevourerMaw 相关
    public List<DevourerMouth> devourerMouths = new List<DevourerMouth>();
    private bool isDevourerLevel = false;


    void Awake()
    {
        player = FindObjectOfType<Player>(); 
        activeFireZones = new List<FireZone>();
        // 动态加载所有地点的 Prefab
        locationPrefabs["Forest"] = Resources.Load<GameObject>("Prefabs/Location/Forest");
        locationPrefabs["Wall_Horizontal"] = Resources.Load<GameObject>("Prefabs/Location/Wall_Horizontal");
        locationPrefabs["Wall_Vertical"] = Resources.Load<GameObject>("Prefabs/Location/Wall_Vertical");
        locationPrefabs["Wall_Corner_UL"] = Resources.Load<GameObject>("Prefabs/Location/Wall_Corner_UL"); // 左上角
        locationPrefabs["Wall_Corner_UR"] = Resources.Load<GameObject>("Prefabs/Location/Wall_Corner_UR"); // 右上角
        locationPrefabs["Wall_Corner_LL"] = Resources.Load<GameObject>("Prefabs/Location/Wall_Corner_LL"); // 左下角
        locationPrefabs["Wall_Corner_LR"] = Resources.Load<GameObject>("Prefabs/Location/Wall_Corner_LR"); // 右下角
        locationPrefabs["Plank"] = Resources.Load<GameObject>("Prefabs/Location/Plank");

    
        LoadTerrainConfigs();
        // 缓存场景中初始的不可进入位置
        CacheExistingLocations();
    }

    private void LoadTerrainConfigs()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Configs", "levelConfig.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            GameConfig gameConfig = JsonUtility.FromJson<GameConfig>(json);
            terrainConfigs = gameConfig.terrains;
        }
        else
        {
            Debug.LogError("Terrain configuration file not found: " + filePath);
        }
    }

    public void SpawnLocationsForLevel(string terrainType)
    {
        TerrainConfig terrainConfig = terrainConfigs.Find(t => t.name == terrainType);
        if (terrainConfig == null)
        {
            Debug.LogError($"Terrain configuration not found for terrain type: {terrainType}");
            return;
        }

        if (terrainType == "Plain")
        {

        }
        else if (terrainType == "Borderland")
        {
            GenerateEdgeTerrain(terrainConfig);
        }
        else if (terrainType == "FortifiedBorderland")
        {
            GenerateWallBorder(terrainConfig);
        }
        else if (terrainType == "DenseForest")
        {
            GenerateDenseForest(terrainConfig);
        }
        else if (terrainType == "ForestMaze")
        {
            GenerateForestMaze();
        }
        else if (terrainType == "RiverForest")
        {
            GenerateRiverForest();
        }
        else if (terrainType == "Borderland2")
        {
            GenerateBorderland2();
        }
        else if (terrainType == "DenseForest2")
        {
            GenerateDenseForest2();
        }
        else if (terrainType == "PlankField")
        {
            GeneratePlankField();
        }
        else if (terrainType == "Prison")
        {
            GeneratePrison();
        }
        else if (terrainType == "DevourerMaw")
        {
            GenerateDevourerMaw();
        }

    }

    private void GenerateEdgeTerrain(TerrainConfig config)
    {
        int mapSize = config.mapSize;
        int openAreaSize = config.openAreaSize;
        //GameObject obstaclePrefab = locationPrefabs[config.obstacleType];
        GameObject obstaclePrefab = locationPrefabs["Forest"];


        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                bool isEdge = (x < (mapSize - openAreaSize) / 2 || x >= mapSize - (mapSize - openAreaSize) / 2 ||
                               y < (mapSize - openAreaSize) / 2 || y >= mapSize - (mapSize - openAreaSize) / 2);

                if (isEdge)
                {
                    CreateLocation(obstaclePrefab, new Vector2Int(x, y));
                }
            }
        }
    }

    private void GenerateWallBorder(TerrainConfig config)
    {
        int mapSize = config.mapSize;

        GameObject wallCornerUL = locationPrefabs["Wall_Corner_UL"]; // 左上角
        GameObject wallCornerUR = locationPrefabs["Wall_Corner_UR"]; // 右上角
        GameObject wallCornerLL = locationPrefabs["Wall_Corner_LL"]; // 左下角
        GameObject wallCornerLR = locationPrefabs["Wall_Corner_LR"]; // 右下角
        GameObject wallHorizontal = locationPrefabs["Wall_Horizontal"];
        GameObject wallVertical = locationPrefabs["Wall_Vertical"];

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                bool isBorder = (x == 0 || x == mapSize - 1 || y == 0 || y == mapSize - 1);

                if (isBorder)
                {
                    if (x == 0 && y == 0) // 左下角
                        CreateLocation(wallCornerLL, new Vector2Int(x, y));
                    else if (x == 0 && y == mapSize - 1) // 左上角
                        CreateLocation(wallCornerUL, new Vector2Int(x, y));
                    else if (x == mapSize - 1 && y == 0) // 右下角
                        CreateLocation(wallCornerLR, new Vector2Int(x, y));
                    else if (x == mapSize - 1 && y == mapSize - 1) // 右上角
                        CreateLocation(wallCornerUR, new Vector2Int(x, y));
                    else if (x == 0 || x == mapSize - 1) // 左右边界
                        CreateLocation(wallVertical, new Vector2Int(x, y));
                    else if (y == 0 || y == mapSize - 1) // 上下边界
                        CreateLocation(wallHorizontal, new Vector2Int(x, y));
                }
            }
        }
    }

    private void GenerateDenseForest(TerrainConfig config)
    {
        int mapSize = config.mapSize;
        int openSize = 2; // 需要留空的列数和行数
        int centerStart = (mapSize - openSize) / 2; // 计算中间空地的起始索引
        int centerEnd = centerStart + openSize; // 计算中间空地的结束索引

        GameObject forestPrefab = locationPrefabs["Forest"];

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                // 只要 x 在中间两列 或 y 在中间两行，就留空；否则填充 `Forest`
                bool isForest = !(x >= centerStart && x < centerEnd) && !(y >= centerStart && y < centerEnd);

                if (isForest)
                {
                    CreateLocation(forestPrefab, new Vector2Int(x, y));
                }
            }
        }
    }

    /// <summary>
    /// “林中密道”示例：墙壁位置放 Forest，所有 F 和 . 都留空，跳过 (4,4)。
    /// ASCII map:
    /// ########
    /// #F..F..#
    /// #.##.#.#
    /// #...F..#
    /// #.##.#.#
    /// #..F..F#
    /// #F....F#
    /// ########
    /// </summary>
    private void GenerateASCIILayout(string[] rows, Dictionary<char, System.Action<Vector2Int>> charActions, string layoutName)
    {
        int size = rows.Length;

        for (int y = 0; y < size; y++)
        {
            int rowIndex = size - 1 - y;
            string row = rows[rowIndex];

            for (int x = 0; x < row.Length; x++)
            {
                char c = row[x];
                Vector2Int pos = new Vector2Int(x, y);
                
                if (charActions.ContainsKey(c))
                {
                    charActions[c](pos);
                }
            }
        }

        Debug.Log($"Generated {layoutName} layout with ASCII map.");
    }

    private void GenerateForestMaze()
    {
        string[] rows = new string[]
        {
            "########",
            "#F.....#",
            "#.##.#.#",
            "#...F..#",
            "#.##...#",
            "#.#F.#F#",
            "#F...###",
            "########"
        };

        GameObject forestPrefab = locationPrefabs["Forest"];
        var charActions = new Dictionary<char, System.Action<Vector2Int>>
        {
            ['#'] = pos => {
                if (!(pos.x == 4 && pos.y == 4))
                    CreateLocation(forestPrefab, pos);
            }
        };

        GenerateASCIILayout(rows, charActions, "ForestMaze");
    }

    /// <summary>
    /// "河流森林"：周围一圈是森林，中间有几条河流
    /// ASCII map:
    /// ########
    /// #......#
    /// #.~~~~.#
    /// #......#
    /// #..~~..#
    /// #.~~~~.#
    /// #......#
    /// ########
    /// </summary>
    private void GenerateRiverForest()
    {
        string[] rows = new string[]
        {
            "########",
            "#......#",
            "#.~~~~.#",
            "#......#",
            "#..~~..#",
            "#.~~~~.#",
            "#......#",
            "########"
        };

        GameObject forestPrefab = locationPrefabs["Forest"];
        var charActions = new Dictionary<char, System.Action<Vector2Int>>
        {
            ['#'] = pos => CreateLocation(forestPrefab, pos),
            ['~'] = pos => CreateWater(pos)
        };

        GenerateASCIILayout(rows, charActions, "RiverForest");
    }

    private void GenerateBorderland2()
    {
        string[] rows = new string[]
        {
            "########",
            "#......#",
            "##....##",
            "###..###",
            "###..###",
            "##....##",
            "#......#",
            "########"
        };

        GameObject forestPrefab = locationPrefabs["Forest"];
        var charActions = new Dictionary<char, System.Action<Vector2Int>>
        {
            ['#'] = pos => CreateLocation(forestPrefab, pos)
        };

        GenerateASCIILayout(rows, charActions, "Borderland2");
    }

    private void GenerateDenseForest2()
    {
        string[] rows = new string[]
        {
            "########",
            "#.#.#..#",
            "#..#.#.#",
            "#.#....#",
            "#.##.#.#",
            "#.#.#..#",
            "##.#.#.#",
            "########"
        };

        GameObject forestPrefab = locationPrefabs["Forest"];
        var charActions = new Dictionary<char, System.Action<Vector2Int>>
        {
            ['#'] = pos => CreateLocation(forestPrefab, pos)
        };

        GenerateASCIILayout(rows, charActions, "DenseForest2");
    }

    private void GeneratePlankField()
    {
        int mapSize = 8;
        int centerStart = 3; // 中间2格的起始位置
        int centerEnd = 4;   // 中间2格的结束位置

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                // 只有中间2x2格(3,3), (3,4), (4,3), (4,4)留空
                bool isCenter = (x >= centerStart && x <= centerEnd && y >= centerStart && y <= centerEnd);
                
                if (!isCenter)
                {
                    CreatePlank(new Vector2Int(x, y));
                }
            }
        }
        
        Debug.Log("Generated PlankField with destructible planks.");
    }

    private void GeneratePrison()
    {
        string[] rows = new string[]
        {
            "........",
            "........",
            "..####..",
            "..#AA#..",
            "..#AA#..",
            "..####..",
            "........",
            "........"
        };

        GameObject plankPrefab = locationPrefabs["Plank"];
        var charActions = new Dictionary<char, System.Action<Vector2Int>>
        {
            ['#'] = pos => CreatePlank(pos),
            ['A'] = pos => AddToSpecialSpawnArea("predefined", pos)
        };

        GenerateASCIILayout(rows, charActions, "Prison");
    }
    
    private void GenerateDevourerMaw()
    {
        isDevourerLevel = true;
        
        string[] rows = new string[]
        {
            "........", // y=7 安全区域
            "........", // y=6 安全区域  
            "........", // y=5 战斗区域
            "........", // y=4 战斗区域
            "........", // y=3 战斗区域
            "........", // y=2 战斗区域
            "........", // y=1 危险区域
            "MMMMMMMM"  // y=0 怪物嘴部（8个核心）
        };
        
        var charActions = new Dictionary<char, System.Action<Vector2Int>>
        {
            ['M'] = pos => CreateDevourerMouth(pos)
        };
        
        GenerateASCIILayout(rows, charActions, "DevourerMaw");
    }

    public void CreatePlank(Vector2Int position)
    {
        GameObject plankPrefab = locationPrefabs["Plank"];
        GameObject plankObject = Instantiate(plankPrefab);
        plankObject.transform.position = player.CalculateWorldPosition(position);
        
        // 添加PlankLocation组件
        PlankLocation plank = plankObject.AddComponent<PlankLocation>();
        plank.Initialize(position, "可攻击的木板", false, 1); // 血量为1
        
        nonEnterablePositions.Add(position);
        spawnedLocations.Add(plankObject);
        
        Debug.Log($"Plank created at {position} with 1 health");
    }

    public void RemovePlank(Vector2Int position, GameObject plankObject)
    {
        nonEnterablePositions.Remove(position);
        spawnedLocations.Remove(plankObject);
        Debug.Log($"Plank removed from position {position}");
    }
    
    public void CreateDevourerMouth(Vector2Int position)
    {
        GameObject mouthPrefab = Resources.Load<GameObject>("Prefabs/Location/DevourerMouth");
        if (mouthPrefab == null)
        {
            // 如果没有专用prefab，使用Forest作为临时替代
            mouthPrefab = locationPrefabs["Forest"];
        }
        
        GameObject mouthObject = Instantiate(mouthPrefab);
        mouthObject.transform.position = player.CalculateWorldPosition(position);
        
        DevourerMouth mouth = mouthObject.GetComponent<DevourerMouth>();
        if (mouth == null)
        {
            mouth = mouthObject.AddComponent<DevourerMouth>();
        }
        
        mouth.Initialize(position, "吞噬者之口", true);
        devourerMouths.Add(mouth);
        spawnedLocations.Add(mouthObject);
        
        Debug.Log($"DevourerMouth created at {position}");
    }
    
    public void OnDevourerMouthDestroyed(DevourerMouth mouth)
    {
        devourerMouths.Remove(mouth);
        Debug.Log($"DevourerMouth destroyed. Remaining: {devourerMouths.Count}");
    }




    // 缓存场景中手动摆放的不可进入位置
    private void CacheExistingLocations()
    {
        nonEnterablePositions.Clear();
        NonEnterableLocation[] nonEnterableLocations = FindObjectsOfType<NonEnterableLocation>();
        foreach (NonEnterableLocation location in nonEnterableLocations)
        {
            nonEnterablePositions.Add(location.position);
        }
        Debug.Log("Cached existing locations.");
    }

    // 清除上一关的动态生成障碍物，但保留初始手动摆放的障碍物
    public void ClearAllLocations()
    {
        foreach (GameObject location in spawnedLocations)
        {
            if (location != null)
            {
                NonEnterableLocation nonEnterableLocation = location.GetComponent<NonEnterableLocation>();
                if (nonEnterableLocation != null)
                {
                    // 从 nonEnterablePositions 中移除对应位置
                    nonEnterablePositions.Remove(nonEnterableLocation.position);
                }
                Destroy(location);
            }
        }
        spawnedLocations.Clear();
        Debug.Log("Cleared all dynamically generated locations.");
    }

    // 生成不同类型的地点
    public void GenerateLocation(string locationType, int count)
    {
        if (!locationPrefabs.ContainsKey(locationType))
        {
            Debug.LogError($"Unknown location type: {locationType}");
            return;
        }

        GameObject locationPrefab = locationPrefabs[locationType];

        for (int i = 0; i < count; i++)
        {
            Vector2Int position = GetRandomPosition();
            if (position != new Vector2Int(-1, -1))
            {
                CreateLocation(locationPrefab, position);
            }
        }
    }

    public void CreateLocation(GameObject prefab, Vector2Int position, float rotation = 0f)
    {
        GameObject locationObject = Instantiate(prefab);
        locationObject.transform.position = player.CalculateWorldPosition(position);
        locationObject.transform.rotation = Quaternion.Euler(0, 0, rotation); // 设置旋转

        NonEnterableLocation location = locationObject.GetComponent<NonEnterableLocation>();
        if (location != null)
        {
            location.Initialize(position, "A non-enterable location.", false);
            nonEnterablePositions.Add(position);  // 缓存到不可进入位置
            spawnedLocations.Add(locationObject);  // 动态生成的障碍物
            Debug.Log($"Location created at {position} using {prefab.name} with rotation {rotation}");
        }
    }

    public void CreateFirePoint(GameObject prefab, Vector2Int position, float rotation = 0f)
    {
        GameObject firePointObject = Instantiate(prefab);
        firePointObject.transform.position = player.CalculateWorldPosition(position);
        firePointObject.transform.rotation = Quaternion.Euler(0, 0, rotation); // 设置旋转

        FirePoint firePoint = firePointObject.GetComponent<FirePoint>();
        if (firePoint != null)
        {
            firePoint.Initialize(position, "fire point", true);
            spawnedLocations.Add(firePointObject); // 动态生成的对象加入列表
            OnFirePointAdded(firePoint);
            Debug.Log($"FirePoint created at {position} using {prefab.name} with rotation {rotation}");
        }
        else
        {
        Debug.LogWarning($"Prefab {prefab.name} does not contain a FirePoint component.");
        }
    }



    private Vector2Int GetRandomPosition()
    {
        int boardSize = FindObjectOfType<MonsterManager>().boardSize;
        int attempts = 0;
        const int maxAttempts = 50;

        Vector2Int randomPosition;
        do
        {
            randomPosition = new Vector2Int(Random.Range(0, boardSize), Random.Range(0, boardSize));
            attempts++;

            if (attempts >= maxAttempts)
            {
                Debug.LogWarning("Failed to find a valid location after 50 attempts.");
                return new Vector2Int(-1, -1);
            }
        } while (nonEnterablePositions.Contains(randomPosition));

        return randomPosition;
    }

    public List<Vector2Int> GetNonEnterablePositions()
    {
        return new List<Vector2Int>(nonEnterablePositions);
    }

    public bool IsNonEnterablePosition(Vector2Int position)
    {
        return nonEnterablePositions.Contains(position);
    }

    public void OnFirePointAdded(FirePoint newFirePoint)
    {
        if (!activeFirePoints.Contains(newFirePoint))
        {
            activeFirePoints.Add(newFirePoint);
            CheckAndFormFireZone();
        }
    }
    
    public void RemoveFirePoint(FirePoint firePoint)
    {
        if (activeFirePoints.Contains(firePoint))
        {
            activeFirePoints.Remove(firePoint);
        }
    }
    
    /// <summary>
    /// 清除所有当前存在的火域视觉
    /// </summary>
    private void ClearFireZones()
    {
        foreach (FireZone zone in activeFireZones)
        {
            if (zone != null)
                Destroy(zone.gameObject);
        }
        activeFireZones.Clear();
    }

    /// <summary>
    /// 检查所有燃点，将它们连接成一个最大面积的火域。
    /// 当有新的燃点时，先清除已有火域，再重新连接所有燃点。
    /// </summary>
    public void CheckAndFormFireZone()
    {
        // 先清除现有火域
        ClearFireZones();
        
        if (activeFirePoints.Count >= 2)
        {
            // 将所有FirePoint连接成一个最大面积的火域
            List<Vector3> polygonPoints = new List<Vector3>();
            
            if (activeFirePoints.Count == 2)
            {
                // 只有两个点，直接连线
                Vector3 p1 = player.CalculateWorldPosition(activeFirePoints[0].gridPosition);
                Vector3 p2 = player.CalculateWorldPosition(activeFirePoints[1].gridPosition);
                polygonPoints.Add(p1);
                polygonPoints.Add(p2);
                polygonPoints.Add(p1); // 闭合
            }
            else
            {
                // 3个或以上点，计算凸包形成最大面积
                polygonPoints = ComputeConvexHull(activeFirePoints);
                // 确保闭合
                if (polygonPoints.Count > 0 && polygonPoints[0] != polygonPoints[polygonPoints.Count - 1])
                    polygonPoints.Add(polygonPoints[0]);
            }
            
            if (polygonPoints.Count >= 2)
            {
                CreateFireZoneUsingPoints(polygonPoints);
            }
        }
    }
    


    /// <summary>
    /// 使用 Graham scan 算法计算凸包（返回世界坐标点）
    /// </summary>
    private List<Vector3> ComputeConvexHull(List<FirePoint> firePoints)
    {
        List<Vector2> points = new List<Vector2>();
        foreach (FirePoint fp in firePoints)
        {
            // 将 gridPosition 转为 Vector2，假设 CalculateWorldPosition 直接对应每格 1 单位
            points.Add(new Vector2(fp.gridPosition.x, fp.gridPosition.y));
        }
        
        if (points.Count <= 1)
            return new List<Vector3>();

        // 找到最下方（如果相同，则最左）的点作为 pivot
        Vector2 pivot = points[0];
        foreach (Vector2 p in points)
        {
            if (p.y < pivot.y || (p.y == pivot.y && p.x < pivot.x))
                pivot = p;
        }
        // 按角度排序
        float Dist2(Vector2 v) => (v.x - pivot.x) * (v.x - pivot.x) + (v.y - pivot.y) * (v.y - pivot.y);
        points.Sort((a, b) =>
        {
            float angleA = Mathf.Atan2(a.y - pivot.y, a.x - pivot.x);
            float angleB = Mathf.Atan2(b.y - pivot.y, b.x - pivot.x);
            
            if (!Mathf.Approximately(angleA, angleB))
                return angleA.CompareTo(angleB);
            else
                return Dist2(a).CompareTo(Dist2(b));
        });
        
        List<Vector2> hull = new List<Vector2>();
        foreach (Vector2 p in points)
        {
            while (hull.Count >= 2 && Cross(hull[hull.Count - 2], hull[hull.Count - 1], p) <= 0)
            {
                hull.RemoveAt(hull.Count - 1);
            }
            hull.Add(p);
        }
        
        // 转换为世界坐标（这里假设不需要额外偏移）
        List<Vector3> worldHull = new List<Vector3>();
        foreach (Vector2 p in hull)
        {
            // 使用 player.CalculateWorldPosition 对 grid 坐标转换为世界坐标
            Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.y));
            worldHull.Add(player.CalculateWorldPosition(gridPos));
        }
        return worldHull;
    }
    
    private float Cross(Vector2 o, Vector2 a, Vector2 b)
    {
        return (a.x - o.x) * (b.y - o.y) - (a.y - o.y) * (b.x - o.x);
    }
    


    /// <summary>
    /// 根据计算得到的多边形顶点创建火域视觉
    /// </summary>
    private void CreateFireZoneUsingPoints(List<Vector3> points)
    {
        GameObject zoneObj = new GameObject("FireZone");
        zoneObj.transform.SetParent(transform);
        zoneObj.transform.position = Vector3.zero; // 使用世界坐标

        // 添加 LineRenderer 组件
        LineRenderer lr = zoneObj.AddComponent<LineRenderer>();
        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());
        lr.startColor = Color.red;
        lr.endColor = Color.red;
        lr.widthMultiplier = 0.1f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.useWorldSpace = true;

        // 添加 FireZone 组件，用于管理持续时间与后续效果
        FireZone fireZone = zoneObj.AddComponent<FireZone>();
        // 使用重载的 Initialize 方法接收多边形顶点，设置持续时间，比如 2 个敌方回合
        fireZone.Initialize(points, 2);
        activeFireZones.Add(fireZone);

        Debug.Log($"Created FireZone with {points.Count} vertices.");
    }
    
    public void CreateBarrier(Vector2Int position, int durability = 4)
    {
        GameObject barrierPrefab = Resources.Load<GameObject>("Prefabs/Location/Barrier");
        if (barrierPrefab == null)
        {
            Debug.LogError("Barrier prefab not found");
            return;
        }
        
        GameObject barrierObject = Instantiate(barrierPrefab);
        barrierObject.transform.position = player.CalculateWorldPosition(position);
        
        BarrierLocation barrier = barrierObject.GetComponent<BarrierLocation>();
        if (barrier == null)
        {
            // 如果没有BarrierLocation组件，添加一个
            barrier = barrierObject.AddComponent<BarrierLocation>();
        }
        
        barrier.InitializeBarrier(position, durability);
        nonEnterablePositions.Add(position);
        spawnedLocations.Add(barrierObject);
        activeBarriers.Add(barrier);
        Debug.Log($"Barrier created at {position} with durability {durability}, nonEnterable positions count: {nonEnterablePositions.Count}");
    }
    
    public void RemoveLocation(Location location)
    {
        if (location is BarrierLocation barrier)
        {
            activeBarriers.Remove(barrier);
            nonEnterablePositions.Remove(barrier.position);
        }
        else if (location is AnchorLocation anchor)
        {
            if (activeAnchor == anchor)
                activeAnchor = null;
        }
        else if (location is MireLocation mire)
        {
            activeMires.Remove(mire);
        }
        
        GameObject locationObject = location.gameObject;
        if (spawnedLocations.Contains(locationObject))
        {
            spawnedLocations.Remove(locationObject);
        }
    }
    
    public void CreateAnchor(Vector2Int position)
    {
        // 如果已经有锚点，先移除旧的
        if (activeAnchor != null)
        {
            GameObject oldAnchorObject = activeAnchor.gameObject;
            RemoveLocation(activeAnchor);
            Destroy(oldAnchorObject);
            activeAnchor = null;
        }
        
        GameObject anchorPrefab = Resources.Load<GameObject>("Prefabs/Location/Anchor");
        if (anchorPrefab == null)
        {
            Debug.LogError("Anchor prefab not found");
            return;
        }
        
        GameObject anchorObject = Instantiate(anchorPrefab);
        anchorObject.transform.position = player.CalculateWorldPosition(position);
        
        AnchorLocation anchor = anchorObject.GetComponent<AnchorLocation>();
        if (anchor == null)
        {
            anchor = anchorObject.AddComponent<AnchorLocation>();
        }
        
        anchor.Initialize(position, "锚点地形，回合结束时拉取玩家", true);
        spawnedLocations.Add(anchorObject);
        activeAnchor = anchor;
        Debug.Log($"Anchor created at {position}");
    }
    
    public void OnTurnEnd()
    {
        // 回合结束时触发锚点效果
        if (activeAnchor != null)
        {
            activeAnchor.OnTurnEnd();
        }
    }
    
    public void CreateMire(Vector2Int position)
    {
        GameObject mirePrefab = Resources.Load<GameObject>("Prefabs/Location/Mire");
        if (mirePrefab == null)
        {
            // 如果没有专用的潮沼 prefab，使用 Forest prefab 作为临时替代
            mirePrefab = locationPrefabs["Forest"];
            if (mirePrefab == null)
            {
                Debug.LogError("No suitable prefab found for Mire");
                return;
            }
        }
        
        GameObject mireObject = Instantiate(mirePrefab);
        mireObject.transform.position = player.CalculateWorldPosition(position);
        
        MireLocation mire = mireObject.GetComponent<MireLocation>();
        if (mire == null)
        {
            mire = mireObject.AddComponent<MireLocation>();
        }
        
        mire.Initialize(position, "潮沼地形，终止敌人位移", true);
        spawnedLocations.Add(mireObject);
        activeMires.Add(mire);
        Debug.Log($"Mire created at {position}");
    }
    
    /// <summary>
    /// 检查怪物移动路径上是否有潮沼，如果有则阻止移动
    /// </summary>
    /// <param name="monster">移动的怪物</param>
    /// <param name="targetPos">目标位置</param>
    /// <returns>是否被潮沼阻止</returns>
    public bool CheckMireInterception(Monster monster, Vector2Int targetPos)
    {
        Vector2Int currentPos = monster.position;
        
        // 检查移动路径上的每个位置
        Vector2Int direction = new Vector2Int(
            targetPos.x > currentPos.x ? 1 : (targetPos.x < currentPos.x ? -1 : 0),
            targetPos.y > currentPos.y ? 1 : (targetPos.y < currentPos.y ? -1 : 0)
        );
        
        Vector2Int checkPos = currentPos + direction;
        
        while (checkPos != targetPos && player.IsValidPosition(checkPos))
        {
            // 检查该位置是否有潮沼
            foreach (MireLocation mire in activeMires)
            {
                if (mire != null && mire.position == checkPos)
                {
                    // 怪物被潮沼阻止
                    return mire.TrapMonster(monster);
                }
            }
            checkPos += direction;
        }
        
        return false; // 没有被阻止
    }
    
    public void CreateWater(Vector2Int position)
    {
        GameObject waterPrefab = Resources.Load<GameObject>("Prefabs/Location/Water");
        if (waterPrefab == null)
        {
            Debug.LogError("Water prefab not found");
            return;
        }
        
        GameObject waterObject = Instantiate(waterPrefab);
        waterObject.transform.position = player.CalculateWorldPosition(position);
        
        waterPositions.Add(position);
        nonEnterablePositions.Add(position); // 水对大部分单位是障碍物
        spawnedLocations.Add(waterObject);
        Debug.Log($"Water created at {position}");
    }
    
    public bool IsWaterPosition(Vector2Int position)
    {
        return waterPositions.Contains(position);
    }
    
    public void AddToSpecialSpawnArea(string areaName, Vector2Int position)
    {
        if (!specialSpawnAreas.ContainsKey(areaName))
            specialSpawnAreas[areaName] = new List<Vector2Int>();
        specialSpawnAreas[areaName].Add(position);
    }
    
    public List<Vector2Int> GetSpecialSpawnArea(string areaName)
    {
        return specialSpawnAreas.ContainsKey(areaName) ? new List<Vector2Int>(specialSpawnAreas[areaName]) : new List<Vector2Int>();
    }
    
    public List<Vector2Int> GetMonsterSpawnPositions()
    {
        return GetSpecialSpawnArea("predefined");
    }
    
    public void ClearMonsterSpawnPositions()
    {
        specialSpawnAreas.Clear();
    }
    
    public void OnTurnStart()
    {
        // 每回合开始时减少所有拒障的耐久度
        for (int i = activeBarriers.Count - 1; i >= 0; i--)
        {
            if (activeBarriers[i] != null)
            {
                activeBarriers[i].ReduceDurability();
            }
        }
        
        // DevourerMaw关卡的下拉逻辑
        if (isDevourerLevel)
        {
            PullEverythingDown();
        }
    }
    
    private void PullEverythingDown()
    {
        // 拉拽玩家
        if (player.position.y > 0)
        {
            Vector2Int newPlayerPos = new Vector2Int(player.position.x, player.position.y - 1);
            player.SetPosition(newPlayerPos);
            Debug.Log($"Player pulled down to {newPlayerPos}");
        }
        
        // 拉拽所有怪物（除了最下排）
        MonsterManager monsterManager = FindObjectOfType<MonsterManager>();
        if (monsterManager != null)
        {
            Monster[] monsters = FindObjectsOfType<Monster>();
            foreach (Monster monster in monsters)
            {
                if (monster.position.y > 0)
                {
                    Vector2Int newMonsterPos = new Vector2Int(monster.position.x, monster.position.y - 1);
                    monster.SetPosition(newMonsterPos);
                    Debug.Log($"Monster {monster.name} pulled down to {newMonsterPos}");
                }
            }
        }
        
        // 拉拽所有Location（除了最下排的DevourerMouth）
        Location[] allLocations = FindObjectsOfType<Location>();
        foreach (Location location in allLocations)
        {
            if (location.position.y > 0 && !(location is DevourerMouth))
            {
                Vector2Int newLocationPos = new Vector2Int(location.position.x, location.position.y - 1);
                location.position = newLocationPos;
                location.transform.position = player.CalculateWorldPosition(newLocationPos);
                Debug.Log($"Location {location.GetType().Name} pulled down to {newLocationPos}");
            }
        }
        
        // 更新nonEnterablePositions列表
        UpdateNonEnterablePositionsAfterPull();
        
        // 处理被拉到最下一格的单位
        HandleDevourerEffects();
    }
    
    private void HandleDevourerEffects()
    {
        MonsterManager monsterManager = FindObjectOfType<MonsterManager>();
        
        // 杀死所有在y=0的敌人
        if (monsterManager != null)
        {
            Monster[] monsters = FindObjectsOfType<Monster>();
            for (int i = monsters.Length - 1; i >= 0; i--)
            {
                Monster monster = monsters[i];
                if (monster.position.y <= 0)
                {
                    Debug.Log($"Monster {monster.monsterName} devoured at y=0!");
                    monsterManager.RemoveMonster(monster);
                    Destroy(monster.gameObject);
                }
            }
        }
        
        // 消灭所有在y=0的Location（除了DevourerMouth）
        Location[] allLocations = FindObjectsOfType<Location>();
        for (int i = allLocations.Length - 1; i >= 0; i--)
        {
            Location location = allLocations[i];
            if (location.position.y <= 0 && !(location is DevourerMouth))
            {
                Debug.Log($"Location {location.GetType().Name} devoured at y=0!");
                RemoveLocation(location);
                Destroy(location.gameObject);
            }
        }
        
        // 处理玩家被拉到y=0的情况
        if (player.position.y <= 0)
        {
            Debug.Log("Player partially devoured! Taking 5 damage and moving up!");
            player.TakeDamage(5);
            player.SetPosition(new Vector2Int(player.position.x, 1)); // 移动到y=1
        }
    }
    
    private void UpdateNonEnterablePositionsAfterPull()
    {
        // 重新构建nonEnterablePositions列表
        nonEnterablePositions.Clear();
        
        // 添加所有NonEnterableLocation的位置
        NonEnterableLocation[] nonEnterableLocations = FindObjectsOfType<NonEnterableLocation>();
        foreach (NonEnterableLocation location in nonEnterableLocations)
        {
            nonEnterablePositions.Add(location.position);
        }
        
        // 添加其他阻挡性Location的位置
        foreach (BarrierLocation barrier in activeBarriers)
        {
            if (barrier != null)
                nonEnterablePositions.Add(barrier.position);
        }
    }
    
    public bool CheckDevourerVictory()
    {
        if (!isDevourerLevel) return false;
        
        int destroyedMouths = 0;
        foreach (DevourerMouth mouth in devourerMouths)
        {
            if (mouth == null || mouth.IsDestroyed())
                destroyedMouths++;
        }
        
        // 胜利条件：摧毁至少4个嘴部 且 玩家在安全区域（y >= 6）
        return destroyedMouths >= 4 && player.position.y >= 6;
    }
}