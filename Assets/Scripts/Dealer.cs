using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStatus
{
    BETTING,
    IN_PLAY,
    COUNTING,
    END
}

public class Dealer : MonoBehaviour
{
    public static Dealer Instance;

    [Header("Card List")]
    [SerializeField, Tooltip("The Deck Of Cards")] List<ScriptableObject> Deck = new List<ScriptableObject>();
    [Tooltip("Cards Currently In play")] List<ScriptableObject> Played = new List<ScriptableObject>();

    GameStatus CurrentGameStatus;

    [Header("Current Hand Details")]
    int CurrentPot = 0;

    [Header("Player Details")]
    int PlayerNum;
    PlayerCardController PlayerCards;
    AI_Brain Brain;

    

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
        Brain = GetComponent<AI_Brain>();
    }

    void Start()
    {
        GameManager.Instance.SetupPlayers();
        CurrentGameStatus = GameStatus.BETTING;
    }

    void BeginRound()
    {
        // Get a list of the players in game (Excluding the dealer)
        List<GameObject> Players = GameManager.Instance.GetPlayersInGame();
        int playerCount = 0;
        // Loop through each player (validate the player) and add 2 cards to their hand
        foreach(var p in Players)
        {
            if(p != null)
            {
                if(playerCount != PlayerNum)
                {
                    PlayerCardController controller = p.GetComponent<PlayerCardController>();
                    DealCard(controller);
                    DealCard(controller);

                } else
                {
                    // Deal 2 Cards to the dealer.
                    DealCard(PlayerCards, false);
                    DealCard(PlayerCards);
                }
                playerCount++;
            }
        }

        CheckNaturals();
        
    }

    void CheckNaturals()
    {
        List<GameObject> Naturals = new List<GameObject>();

        foreach (var p in GameManager.Instance.GetPlayersInGame())
        {
            int total = 0;
            foreach (var card in p.GetComponent<PlayerCardController>().GetCardsInHand())
            {
                var c = card.GetComponent<CardController>().Card as CardObject;
                total += c.CardValue;
            }

            if (total == 21)
            {
                Naturals.Add(p);
            }
        }

        if(Naturals.Count > 1)
        {
            foreach(var p in Naturals)
            {
                var cntrl = p.GetComponent<PlayerController>();
                if(cntrl != null)
                {
                    cntrl.AddChips(CurrentPot / Naturals.Count);
                }

            }

            ChangeGameStatus(GameStatus.END);
            EndRound();
        } else if(Naturals.Count == 1)
        {
            var cntrl = Naturals[0].GetComponent<PlayerController>();
            if(cntrl != null)
            {
                cntrl.AddChips(Mathf.RoundToInt(CurrentPot * 1.5f));
                
            }

            ChangeGameStatus(GameStatus.END);
            EndRound();
        }

    }

    

    public void AddToPot(int Chips)
    {
        CurrentPot += Chips;
    }

    public void DealCard(PlayerCardController Cards, bool ShowCard = true)
    {
        Cards.AddCard(GetCard(), ShowCard);
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

    public void ChangeGameStatus(GameStatus NewStatus)
    {
        // Change the state
        CurrentGameStatus = NewStatus;
        // Run a switch statement to check the value
        switch(NewStatus)
        {
            case GameStatus.IN_PLAY:
                BeginRound();
                break;
            case GameStatus.COUNTING:
                CountCards();
                break;
            default:
                Debug.LogError("Error Changing State");
                break;
        }
    }

    private void CountCards()
    {
        foreach(var card in this.PlayerCards.GetCardsInHand())
        {
            card.GetComponent<CardController>().ShowCard();
        }

        PlayerController HighestValue = null;
        foreach(var p in GameManager.Instance.GetPlayersInGame())
        {
            PlayerCardController Cntrl = p.GetComponent<PlayerCardController>();
            if(Cntrl != null)
            {
                if(Cntrl.GetCurrentValue() <= 21)
                {
                    if(HighestValue == null)
                    {
                        HighestValue = p.GetComponent<PlayerController>();
                    } else
                    {
                        if (Cntrl.GetCurrentValue() > HighestValue.GetPlayerCards().GetCurrentValue())
                            HighestValue = Cntrl.gameObject.GetComponent<PlayerController>();
                    }
                }
            }
        }

        if(this.PlayerCards.GetCurrentValue() > HighestValue.GetPlayerCards().GetCurrentValue())
        {
            Debug.Log("Dealer Wins");
        } else
        {
            Debug.Log("Player Wins");
        }
    }

    public void PlayTurn()
    {
        GetComponent<AI_Brain>().Action();
    }


    public void SetPlayerNum(int num) => PlayerNum = num;
    public GameStatus GetCurrentGameStatus() => CurrentGameStatus;
    public int GetPlayerNum() => PlayerNum;
    public AI_Brain GetBrain() => Brain;
}