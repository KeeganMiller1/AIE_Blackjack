using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dealer : MonoBehaviour
{
    public static Dealer Instance;

    [Header("Card List")]
    [SerializeField, Tooltip("The Deck Of Cards")] List<ScriptableObject> Deck = new List<ScriptableObject>();
    [Tooltip("Cards Currently In play")] List<ScriptableObject> Played = new List<ScriptableObject>();

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

    void Start()
    {
        GameManager.Instance.SetupPlayers();
    }

    void BeginRound()
    {
        // Get a list of the players in game (Excluding the dealer)
        List<GameObject> Players = GameManager.Instance.GetPlayersInGame();
        // Loop through each player (validate the player) and add 2 cards to their hand
        foreach(var p in Players)
        {
            if(p != null)
            {
                PlayerCardController controller = p.GetComponent<PlayerCardController>();
                DealCard(controller);
                DealCard(controller);
            }
        }

        // Deal 2 Cards to the dealer.
        DealCard(PlayerCards);
        DealCard(PlayerCards);
    }

    public void DealCard(PlayerCardController Cards)
    {
        Cards.AddCard(GetCard());
    }

    private ScriptableObject GetCard()
    {
        // Select a random card from the deck
        var card = Deck[UnityEngine.Random.Range(0, Deck.Count)];
        // Add the card to played & remove to from the deck
        Played.Add(card);
        Deck.Remove(card);
        return card;
    }
}