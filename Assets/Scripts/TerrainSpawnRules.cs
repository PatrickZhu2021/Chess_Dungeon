using System.Collections.Generic;
using UnityEngine;

public static class TerrainSpawnRules
{
    public static bool IsValidSpawnPosition(List<Vector2Int> positions, string terrainType)
    {
        switch (terrainType)
        {
            case "DevourerMaw":
                return !positions.Exists(pos => pos.y <= 2);
            case "Prison":
                // 可以添加Prison的特殊规则
                return true;
            default:
                return true;
        }
    }
    
    public static bool HasSpawnRestrictions(string terrainType)
    {
        return terrainType == "DevourerMaw" || terrainType == "Prison";
    }
}