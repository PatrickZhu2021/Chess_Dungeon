using UnityEngine;

public class DevourerMouth : Location
{
    [Header("Devourer Mouth Settings")]
    public int health = 3;
    public int maxHealth = 3;
    
    private void Start()
    {
        // 设置为可进入但可攻击的地形
        isAccessible = true;
    }
    
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"DevourerMouth at {position} took {damage} damage. Health: {health}/{maxHealth}");
        
        if (health <= 0)
        {
            OnDestroyed();
        }
    }
    
    private void OnDestroyed()
    {
        Debug.Log($"DevourerMouth at {position} destroyed!");
        
        // 通知LocationManager移除
        LocationManager locationManager = FindObjectOfType<LocationManager>();
        if (locationManager != null)
        {
            locationManager.OnDevourerMouthDestroyed(this);
        }
        
        Destroy(gameObject);
    }
    
    public bool IsDestroyed()
    {
        return health <= 0;
    }
    
    public override void Interact()
    {
        Debug.Log($"DevourerMouth at {position} cannot be interacted with directly - attack to damage it!");
    }
}