using UnityEngine;

public class PlankLocation : Location
{
    public int health;
    public int maxHealth;

    public void Initialize(Vector2Int pos, string desc, bool enterable, int hp)
    {
        base.Initialize(pos, desc, enterable);
        maxHealth = hp;
        health = hp;
    }



    private void DestroyPlank()
    {
        LocationManager locationManager = FindObjectOfType<LocationManager>();
        if (locationManager != null)
        {
            locationManager.RemovePlank(position, gameObject);
        }
        
        Debug.Log($"Plank at {position} destroyed");
        Destroy(gameObject);
    }

    public bool CanBeAttacked()
    {
        return health > 0;
    }

    public override void Interact()
    {
        // 木板被攻击时受到1点伤害
        TakeDamage(1);
    }
    
    public override void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"Plank at {position} took {damage} damage, health: {health}/{maxHealth}");
        
        if (health <= 0)
        {
            DestroyPlank();
        }
    }
}