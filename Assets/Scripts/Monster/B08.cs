using UnityEngine;
using System.Collections.Generic;

public class B08 : Monster
{
    private enum MoveMode
    {
        Rook,    // 车
        Knight,  // 马
        Bishop,  // 象
        Queen    // 后
    }
    
    private MoveMode currentMode = MoveMode.Rook;
    private int turnCounter = 0;
    private List<int> skillQueue = new List<int> { 0, 1, 2 };
    private int currentSkillIndex = 0;
    private bool hasTriggeredClearLocations = false;

    public override void Initialize(Vector2Int startPos)
    {
        health = 16;
        base.Initialize(startPos);
        type = MonsterType.Queen;
        monsterName = "B08";
        displayName = "强盗王";
        turnCounter = 0;
        currentMode = MoveMode.Rook;
        ShuffleSkillQueue();
        Debug.Log($"B08 强盗王 initialized at {startPos} with mode: {currentMode}");
    }

    public override void PerformMovement()
    {
        if (IsStunned())
        {
            Debug.Log($"B08 is stunned for {stunnedStacks} turns.");
            return;
        }
        
        // 检查是否需要执行特殊回合
        if (hasTriggeredClearLocations && health <= 6)
        {
            PerformSpecialTurn();
            hasTriggeredClearLocations = false; // 重置标志，避免重复触发
            return;
        }
        
        Vector2Int targetPos = GetTargetPosition();
        List<Vector2Int> possibleMoves = GetPossibleMoves();
        
        // 过滤掉无效位置
        possibleMoves.RemoveAll(pos => !IsValidPosition(pos) || IsPositionOccupied(pos));
        
        Vector2Int bestMove = position;
        float bestDistance = Vector2Int.Distance(position, targetPos);
        
        foreach (Vector2Int move in possibleMoves)
        {
            float distance = Vector2Int.Distance(move, targetPos);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestMove = move;
            }
        }
        
        if (bestMove != position)
        {
            position = bestMove;
            UpdatePosition();
            Debug.Log($"B08 moved to {bestMove} using {currentMode} mode");
        }
        else
        {
            // 无法移动，使用技能
            UseSkill();
        }
        
        // 移动完成后切换模式
        SwitchMoveMode();
    }
    
    private void SwitchMoveMode()
    {
        turnCounter++;
        currentMode = (MoveMode)(turnCounter % 4);
        Debug.Log($"B08 switched to mode: {currentMode}");
    }
    
    private List<Vector2Int> GetPossibleMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        
        switch (currentMode)
        {
            case MoveMode.Rook:
                moves.AddRange(GetRookMoves());
                break;
            case MoveMode.Knight:
                moves.AddRange(GetKnightMoves());
                break;
            case MoveMode.Bishop:
                moves.AddRange(GetBishopMoves());
                break;
            case MoveMode.Queen:
                moves.AddRange(GetQueenMoves());
                break;
        }
        
        return moves;
    }
    
    private List<Vector2Int> GetRookMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        
        foreach (Vector2Int dir in directions)
        {
            Vector2Int currentPos = position + dir;
            
            // 翅膀技能：可以跨越障碍物，但不能停在障碍物上
            while (IsWithinBounds(currentPos))
            {
                LocationManager locManager = FindObjectOfType<LocationManager>();
                if (!IsPositionOccupied(currentPos) && 
                    (locManager == null || !locManager.IsNonEnterablePosition(currentPos)))
                {
                    moves.Add(currentPos);
                }
                currentPos += dir;
            }
        }
        
        return moves;
    }
    
    private List<Vector2Int> GetKnightMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        Vector2Int[] knightMoves = {
            new Vector2Int(2, 1), new Vector2Int(2, -1),
            new Vector2Int(-2, 1), new Vector2Int(-2, -1),
            new Vector2Int(1, 2), new Vector2Int(1, -2),
            new Vector2Int(-1, 2), new Vector2Int(-1, -2)
        };
        
        foreach (Vector2Int move in knightMoves)
        {
            Vector2Int newPos = position + move;
            if (IsValidPosition(newPos))
                moves.Add(newPos);
        }
        
        return moves;
    }
    
    private List<Vector2Int> GetBishopMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        Vector2Int[] directions = { 
            new Vector2Int(1, 1), new Vector2Int(1, -1),
            new Vector2Int(-1, 1), new Vector2Int(-1, -1)
        };
        
        foreach (Vector2Int dir in directions)
        {
            Vector2Int currentPos = position + dir;
            
            // 翅膀技能：可以跨越障碍物，但不能停在障碍物上
            while (IsWithinBounds(currentPos))
            {
                LocationManager locManager = FindObjectOfType<LocationManager>();
                if (!IsPositionOccupied(currentPos) && 
                    (locManager == null || !locManager.IsNonEnterablePosition(currentPos)))
                {
                    moves.Add(currentPos);
                }
                currentPos += dir;
            }
        }
        
        return moves;
    }
    
    private List<Vector2Int> GetQueenMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        moves.AddRange(GetRookMoves());
        moves.AddRange(GetBishopMoves());
        return moves;
    }

    public override List<Vector2Int> CalculatePossibleMoves()
    {
        List<Vector2Int> moves = GetPossibleMoves();
        // 过滤掉无效位置或被占据的位置
        moves.RemoveAll(pos => !IsValidPosition(pos) || IsPositionOccupied(pos));
        return moves;
    }
    
    public override int GetAttackDamage()
    {
        return 3;
    }
    
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        
        if (health <= 6 && !hasTriggeredClearLocations)
        {
            hasTriggeredClearLocations = true;
            Debug.Log("B08 reached 6 health - will trigger special turn!");
        }
    }
    
    private void PerformSpecialTurn()
    {
        Debug.Log("B08 performing special turn: clearing locations and transforming monsters!");
        
        // 1. 清除所有location
        ClearAllLocations();
        
        // 2. 转化所有其他怪物
        TransformAllOtherMonsters();
    }
    
    private void ClearAllLocations()
    {
        LocationManager locationManager = FindObjectOfType<LocationManager>();
        if (locationManager != null)
        {
            locationManager.ClearAllLocations();
        }
    }
    
    private void TransformAllOtherMonsters()
    {
        MonsterManager monsterManager = FindObjectOfType<MonsterManager>();
        if (monsterManager == null) return;
        
        string[] transformTypes = { "I03", "I04", "I05" };
        List<Monster> monstersToTransform = new List<Monster>();
        
        // 收集需要转化的怪物（除了自己）
        foreach (Monster monster in monsterManager.monsters)
        {
            if (monster != this && monster != null)
            {
                monstersToTransform.Add(monster);
            }
        }
        
        // 转化每个怪物
        foreach (Monster oldMonster in monstersToTransform)
        {
            Vector2Int oldPosition = oldMonster.position;
            string newType = transformTypes[Random.Range(0, transformTypes.Length)];
            
            // 移除旧怪物
            monsterManager.RemoveMonster(oldMonster);
            Destroy(oldMonster.gameObject);
            
            // 创建新怪物
            Monster newMonster = monsterManager.CreateMonsterByType(newType);
            if (newMonster != null)
            {
                monsterManager.SpawnMonsterAtPosition(newMonster, oldPosition);
                Debug.Log($"Transformed monster at {oldPosition} to {newType}");
            }
        }
    }

    private void ShuffleSkillQueue()
    {
        for (int i = 0; i < skillQueue.Count; i++)
        {
            int temp = skillQueue[i];
            int randomIndex = Random.Range(i, skillQueue.Count);
            skillQueue[i] = skillQueue[randomIndex];
            skillQueue[randomIndex] = temp;
        }
        currentSkillIndex = 0;
    }
    
    private void UseSkill()
    {
        int skillId = skillQueue[currentSkillIndex];
        currentSkillIndex++;
        
        if (currentSkillIndex >= skillQueue.Count)
        {
            ShuffleSkillQueue();
        }
        
        switch (skillId)
        {
            case 0:
                CreatePlanksAroundPlayer();
                break;
            case 1:
                SummonEliteMonster();
                break;
            case 2:
                SummonBasicMonsters();
                break;
        }
    }
    
    private void CreatePlanksAroundPlayer()
    {
        Vector2Int playerPos = player.position;
        LocationManager locationManager = FindObjectOfType<LocationManager>();
        
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int plankPos = playerPos + new Vector2Int(x, y);
                if (IsValidPosition(plankPos) && !IsPositionOccupied(plankPos) && plankPos != playerPos && plankPos != position)
                {
                    locationManager.CreatePlank(plankPos);
                }
            }
        }
        Debug.Log("B08 created planks around player");
    }
    
    private void SummonEliteMonster()
    {
        string[] eliteMonsters = { "B05", "B06", "B07" };
        string monsterType = eliteMonsters[Random.Range(0, eliteMonsters.Length)];
        SummonMonster(monsterType);
        Debug.Log($"B08 summoned elite monster: {monsterType}");
    }
    
    private void SummonBasicMonsters()
    {
        string[] basicMonsters = { "B01", "B02", "B03", "B04" };
        for (int i = 0; i < 2; i++)
        {
            string monsterType = basicMonsters[Random.Range(0, basicMonsters.Length)];
            SummonMonster(monsterType);
        }
        Debug.Log("B08 summoned 2 basic monsters");
    }
    
    private void SummonMonster(string monsterType)
    {
        MonsterManager monsterManager = FindObjectOfType<MonsterManager>();
        Monster monster = monsterManager.CreateMonsterByType(monsterType);
        if (monster != null)
        {
            monsterManager.SpawnMonster(monster);
        }
    }

    private bool IsWithinBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < player.boardSize && pos.y >= 0 && pos.y < player.boardSize;
    }
    
    public override List<string> GetSkills()
    {
        return new List<string> { "翅膀" };
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/B08");
    }
}