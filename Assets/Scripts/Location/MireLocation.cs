using UnityEngine;

public class MireLocation : Location
{
    private LocationManager locationManager;
    
    public override void Initialize(Vector2Int pos, string desc, bool enterable)
    {
        base.Initialize(pos, desc, enterable);
        locationManager = FindObjectOfType<LocationManager>();
    }
    
    public override void Interact()
    {
        Debug.Log($"Mire at {position}: {description}");
    }
    
    /// <summary>
    /// 检查是否有怪物经过此位置，如果有则阻止其继续移动
    /// </summary>
    /// <param name="monster">经过的怪物</param>
    /// <returns>是否被潮沼阻止</returns>
    public bool TrapMonster(Monster monster)
    {
        Debug.Log($"Monster {monster.name} trapped by mire at {position}");
        
        // 只更新位置，不调用 UpdatePosition 避免递归
        monster.transform.position = monster.player.CalculateWorldPosition(position);
        
        // 移除潮沼地形
        RemoveMire();
        
        return true; // 返回 true 表示怪物被阻止
    }
    
    private void RemoveMire()
    {
        if (locationManager != null)
        {
            locationManager.RemoveLocation(this);
        }
        Destroy(gameObject);
    }
}