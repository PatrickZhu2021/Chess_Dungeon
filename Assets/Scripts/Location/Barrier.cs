using UnityEngine;

public class BarrierLocation : NonEnterableLocation
{
    public int durability = 4;
    public int maxDurability = 4;
    
    public override void Interact()
    {
        Debug.Log($"You see a barrier at {position}. Durability: {durability}/{maxDurability}");
    }

    public void InitializeBarrier(Vector2Int barrierPosition, int initialDurability = 4)
    {
        maxDurability = initialDurability;
        durability = initialDurability;
        Initialize(barrierPosition, $"A barrier blocking the way. Durability: {durability}", false);
    }
    
    public void ReduceDurability()
    {
        durability--;
        description = $"A barrier blocking the way. Durability: {durability}";
        Debug.Log($"Barrier at {position} durability reduced to {durability}");
        
        if (durability <= 0)
        {
            DestroyBarrier();
        }
    }
    
    private void DestroyBarrier()
    {
        Debug.Log($"Barrier at {position} destroyed");
        LocationManager locationManager = FindObjectOfType<LocationManager>();
        if (locationManager != null)
        {
            locationManager.RemoveLocation(this);
        }
        Destroy(gameObject);
    }
}