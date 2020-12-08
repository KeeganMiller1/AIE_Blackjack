using System;
using System.Collections.Generic;
using UnityEngine;

public enum ECardSuite
{
    SPADE,
    CLUB,
    HEART,
    DIAMOND
}

public enum ECardType
{
    STANDARD,
    ACE,
    JACK,
    QUEEN,
    KING
}



class CardObject : ScriptableObject 
{
    public ECardSuite CardSuit;
    public int CardValue;
    public ECardType CardType;

    public Sprite CardSprite;
}

