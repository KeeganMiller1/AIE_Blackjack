﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardController : MonoBehaviour
{
    List<ScriptableObject> CardsInHand = new List<ScriptableObject>();

    [Header("Prefabs")]
    [SerializeField] GameObject CardPrefab;


    [Header("Card Position")]
    [SerializeField, Tooltip("Start position for the first card")] 
    Vector2 StartPosition;
    Vector2 LastUsedPosition;
    [SerializeField, Tooltip("How much the X Position will increase each card")]
    float CardPositionIncrement;

    void Awake()
    {

    }

    List<ScriptableObject> GetCardsInHand() => CardsInHand;

    public void AddCard(ScriptableObject card)
    {
        CardsInHand.Add(card);
        var go = Instantiate(CardPrefab);
        LastUsedPosition = new Vector2(LastUsedPosition.x + CardPositionIncrement, LastUsedPosition.y);
        go.transform.position = LastUsedPosition;
        go.transform.parent = this.transform;
    }
    public void EmptyHand() => CardsInHand.Clear();

}