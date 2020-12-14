using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    public ScriptableObject Card;
    bool CanSeeCard;


    /// <summary>
    /// Show the back of the card
    /// </summary>
    public void HideCard()
    {
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.GetSelectedCardBack();
        CanSeeCard = false;
    }

    /// <summary>
    /// Show the face of the card
    /// </summary>
    public void ShowCard()
    {
        CardObject card = Card as CardObject;
        GetComponent<SpriteRenderer>().sprite = card.CardSprite;
        CanSeeCard = true;
    }

    // --- GETTERS --- //
    public bool IsCardVisible() => CanSeeCard;
}