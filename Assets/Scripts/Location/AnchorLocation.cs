using UnityEngine;

public class AnchorLocation : Location
{
    private Player player;
    private LocationManager locationManager;
    
    public override void Interact()
    {
        Debug.Log($"Anchor at {position}: {description}");
    }
    
    public override void Initialize(Vector2Int pos, string desc, bool enterable)
    {
        base.Initialize(pos, desc, enterable);
        player = FindObjectOfType<Player>();
        locationManager = FindObjectOfType<LocationManager>();
    }
    
    public void OnTurnEnd()
    {
        // 检查本格是否被占据
        if (!IsPositionOccupied(position))
        {
            // 移动玩家到此位置
            if (player != null)
            {
                player.Move(position);
                
                // 触发涌潮效果
                if (player.torrentStacks > 0)
                {
                    Effects.KeywordEffects.TriggerTorrentEffect(player, position);
                }
                
                Debug.Log($"Anchor pulled player to position {position}");
            }
            
            // 移除锚点地形
            RemoveAnchor();
        }
    }
    
    private bool IsPositionOccupied(Vector2Int pos)
    {
        // 检查玩家是否在此位置
        if (player != null && player.position == pos)
            return true;
            
        // 检查是否有怪物在此位置
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monsterObject in monsters)
        {
            Monster monster = monsterObject.GetComponent<Monster>();
            if (monster != null && monster.IsPartOfMonster(pos))
                return true;
        }
        
        return false;
    }
    
    private void RemoveAnchor()
    {
        if (locationManager != null)
        {
            locationManager.RemoveLocation(this);
        }
        Destroy(gameObject);
    }
}