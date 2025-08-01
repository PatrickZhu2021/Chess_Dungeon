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

    public override void Initialize(Vector2Int startPos)
    {
        health = 16;
        base.Initialize(startPos);
        type = MonsterType.Queen;
        monsterName = "B08";
        displayName = "强盗王";
        turnCounter = 0;
        currentMode = MoveMode.Rook;
        Debug.Log($"B08 强盗王 initialized at {startPos} with mode: {currentMode}");
    }

    public override void PerformMovement()
    {
        if (IsStunned())
        {
            Debug.Log($"B08 is stunned for {stunnedStacks} turns.");
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
            for (int i = 1; i < 8; i++)
            {
                Vector2Int newPos = position + dir * i;
                if (IsValidPosition(newPos))
                    moves.Add(newPos);
                else
                    break;
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
            for (int i = 1; i < 8; i++)
            {
                Vector2Int newPos = position + dir * i;
                if (IsValidPosition(newPos))
                    moves.Add(newPos);
                else
                    break;
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

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/B08");
    }
}