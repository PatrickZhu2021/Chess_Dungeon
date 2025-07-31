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
    
    public void OnMonsterEnter(Monster monster)
    {
        // 终止怪物位移并停留在该格
        Debug.Log($"Monster {monster.name} trapped by mire at {position}");
        
        // 移除潮沼地形
        RemoveMire();
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