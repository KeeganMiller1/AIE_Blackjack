using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    public ScriptableObject Card;
    bool CanSeeCard;
    public void SetupObject()
    {
        
    }

    public void HideCard()
    {
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.GetSelectedCardBack();
        CanSeeCard = false;
    }

    public void ShowCard()
    {
        CardObject card = Card as CardObject;
        GetComponent<SpriteRenderer>().sprite = card.CardSprite;
        CanSeeCard = true;
    }

    public bool IsCardVisible() => CanSeeCard;
}