using UnityEngine;

public class RitualPoint : Location
{
    public override void Initialize(Vector2Int position, string description, bool isAccessible)
    {
        base.Initialize(position, description, isAccessible);
    }

    public override void Interact()
    {
        Debug.Log($"Player touched ritual point at {position}");
        
        // 阻止所有T02本回合移动
        T02[] allT02s = FindObjectsOfType<T02>();
        foreach (T02 t02 in allT02s)
        {
            t02.SetMovementBlocked(true);
        }
    }
}