using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dealer : MonoBehaviour
{
    public static Dealer Instance;

    [Header("Card List")]
    [SerializeField] List<ScriptableObject> Deck = new List<ScriptableObject>();
    [SerializeField] List<ScriptableObject> Played = new List<ScriptableObject>();

    PlayerCardController PlayerCards;

    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(this.gameObject);
        }

        PlayerCards = GetComponent<PlayerCardController>();
    }

    public void DealCard(PlayerCardController Cards)
    {
        Cards.AddCard(GetCard());
    }

    private ScriptableObject GetCard()
    {
        var card = Deck[UnityEngine.Random.Range(0, Deck.Count)];
        Played.Add(card);
        Deck.Remove(card);
        return card;
    }
}