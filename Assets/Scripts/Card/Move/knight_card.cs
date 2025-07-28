using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class knight_card : CardButtonBase
{
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
        Debug.Log("pawn_card Initialize with card: " + (card != null ? card.ToString() : "null"));
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
                MoveHelper.ShowKnightMoveOptions(player, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in knight_card.OnClick");
        }
    }
}

public class KnightCard : Card
{
    public KnightCard() : base(CardType.Move, "M02", 20) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Move/BM03");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Move/BM03");
    }
    public override string GetDescription()
    {
        return "K移动";
    }

}