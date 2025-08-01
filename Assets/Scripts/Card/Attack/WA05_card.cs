using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;

public class WA05_card : CardButtonBase
{
    Vector2Int[] crossDirections = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
    }

    protected override void Start()
    {
        base.Start();

        if (card != null && card.IsUpgraded())
        {
            Transform glow = transform.Find("UpgradeEffect");
            if (glow != null)
                glow.gameObject.SetActive(true);
        }
    }

    protected override void OnClick()
    {
        if (card != null)
        {
            if (player.currentCard == card)
            {
                player.DeselectCurrentCard();
            }
            else
            {
                int damage = card.GetDamageAmount();
                player.damage = damage;
                player.ShowAttackOptions(crossDirections, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in WA05_card.OnClick");
        }
    }
}

public class WA05 : Card
{
    public WA05() : base(CardType.Attack, "WA05", 10) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/WA05");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/WA05");
    }

    public override string GetDescription()
    {
        return "十字无限，对该方向所有敌方单位造成1点伤害，并向侧方击退1次";
    }

    public override void OnCardExecuted(Vector2Int attackPos)
    {
        // 计算攻击方向
        Vector2Int direction = attackPos - player.position;
        direction = new Vector2Int(Mathf.Clamp(direction.x, -1, 1), Mathf.Clamp(direction.y, -1, 1));
        
        // 对该方向所有敌方单位造成伤害并击退
        Vector2Int currentPos = player.position + direction;
        while (player.IsValidPosition(currentPos))
        {
            Monster monster = KeywordEffects.GetMonsterAtPosition(currentPos);
            if (monster != null)
            {
                // 造成伤害
                monster.TakeDamage(1);
                
                // 计算侧方击退方向（垂直于攻击方向）
                Vector2 sideDirection = GetSideDirection(direction);
                KeywordEffects.ApplyKnockback(monster, sideDirection, player);
            }
            currentPos += direction;
        }
    }
    
    private Vector2 GetSideDirection(Vector2Int attackDirection)
    {
        // 根据攻击方向返回侧方方向
        if (attackDirection == Vector2Int.up || attackDirection == Vector2Int.down)
        {
            // 垂直攻击时，随机选择左或右
            return Random.Range(0, 2) == 0 ? Vector2.left : Vector2.right;
        }
        else if (attackDirection == Vector2Int.left || attackDirection == Vector2Int.right)
        {
            // 水平攻击时，随机选择上或下
            return Random.Range(0, 2) == 0 ? Vector2.up : Vector2.down;
        }
        
        // 默认返回右方
        return Vector2.right;
    }

    public override int GetDamageAmount()
    {
        return 0;
    }
}