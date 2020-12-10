using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    public ScriptableObject Card;

    public void SetupObject()
    {
        CardObject card = Card as CardObject;
        GetComponent<SpriteRenderer>().sprite = card.CardSprite;
    }
}