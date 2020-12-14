using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    List<GameObject> Blackjacks = new List<GameObject>();

    public void Awake()
    {
        // Create an instance
        if(Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(this.gameObject);
        }

        // Get required components
        PlayerCards = GetComponent<PlayerCardController>();
        Brain = GetComponent<AI_Brain>();
    }

    void Start()
    {
        // Setup the players in game & set the game status
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

        // Check if any players have a natural
        CheckNaturals();
        
    }

    void CheckNaturals()
    {
        // Create an initial list
        List<GameObject> Naturals = new List<GameObject>();
        // Loop through each player in game & make sure that it isn't the dealer (as we will deal with them last, No pun intended)
        foreach(var p in GameManager.Instance.GetPlayersInGame())
        {
            PlayerController player = p.GetComponent<PlayerController>();
            if(player != null)
            {
                // If the value is equal to 21 than add them to the list of naturals
                if(p.GetComponent<PlayerCardController>().GetCurrentValue() == 21)
                {
                    Naturals.Add(p);
                }
            }
        }

        // Check if the dealer has a natural
        bool DealerHasNat = false;
        if (PlayerCards.GetCurrentValue() == 21)
            DealerHasNat = true;

        // If the dealer has natural than then the players get their last bet back 
        // Otherise they get 2.5 back
        if(DealerHasNat)
        {
            foreach (var p in Naturals)
            {
                p.GetComponent<PlayerController>().AddChips(p.GetComponent<PlayerController>().GetLastBet());
            }
        } else
        {
            foreach(var p in Naturals)
            {
                p.GetComponent<PlayerController>().AddChips(Mathf.RoundToInt(p.GetComponent<PlayerController>().GetLastBet() * 2.5f));
            }
        }

        // TODO: Skip the player if they have a natural
    }

    /// <summary>
    /// this method will add chips to the pot INVALID
    /// </summary>
    /// <param name="Chips">Amount of chips to add</param>
    public void AddToPot(int Chips)
    {
        CurrentPot += Chips;
    }

    /// <summary>
    /// This method will deal a card to the player
    /// </summary>
    /// <param name="Cards">Card Controller</param>
    /// <param name="ShowCard">Whether to show the card or not</param>
    public void DealCard(PlayerCardController Cards, bool ShowCard = true)
    {
        Cards.AddCard(GetCard(), ShowCard);
    }


    /// <summary>
    /// This method will get a random card from the deck
    /// </summary>
    /// <returns></returns>
    private ScriptableObject GetCard()
    {
        // Select a random card from the deck
        var card = Deck[UnityEngine.Random.Range(0, Deck.Count)];
        // Add the card to played & remove to from the deck
        Played.Add(card);
        Deck.Remove(card);
        return card;
    }

    /// <summary>
    /// Change the current status of the game
    /// </summary>
    /// <param name="NewStatus"></param>
    public void ChangeGameStatus(GameStatus NewStatus)
    {
        // Change the state
        CurrentGameStatus = NewStatus;
        // Run a switch statement to check the value
        switch(NewStatus)
        {
            case GameStatus.BETTING:
                break;
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
        foreach(var p in GameManager.Instance.GetPlayersInGame())
        {

            // If the player controller isn't the dealers
            if(p.GetComponent<PlayerCardController>() != PlayerCards && !p.GetComponent<PlayerCardController>().HasPlayerBust())
            {
                if(!PlayerCards.HasPlayerBust())
                {
                    // Get current value
                    int player_value = p.GetComponent<PlayerCardController>().GetCurrentValue();
                    // Check if the player value is larget than the dealer, if it is than double there bet.
                    // Otherwise if it's equal to the dealer they get their bet back.
                    if (player_value > PlayerCards.GetCurrentValue())
                    {
                        p.GetComponent<PlayerController>().AddChips(p.GetComponent<PlayerController>().GetLastBet() * 2);
                        ShowWin(p.GetComponent<PlayerController>().GetLastBet() * 2);
                    }
                    else if (player_value == PlayerCards.GetCurrentValue())
                    {
                        p.GetComponent<PlayerController>().AddChips(p.GetComponent<PlayerController>().GetLastBet());
                        ShowDraw();
                    }
                    else
                    {
                        ShowLose();
                    }
                } else
                {
                    ShowWin(p.GetComponent<PlayerController>().GetLastBet() * 2);
                }
            }

            EndRound();
        }
    }

    /// <summary>
    /// Have the Dealer Use AI for their turn
    /// </summary>
    public void PlayTurn()
    {
        GetComponent<AI_Brain>().Action();
    }

    public void EndRound()
    {
        // Clear all the scene objects & change the game status to betting so we can start betting
        StartCoroutine(GameManager.Instance.ClearScene());
        ChangeGameStatus(GameStatus.BETTING);
        // Loop through each player in the game & reset their hand
        foreach(var p in GameManager.Instance.GetPlayersInGame())
        {
            p.GetComponent<PlayerCardController>().GetCardsInHand().Clear();
            p.GetComponent<PlayerCardController>().ResetCardPosition();
        }

        // Check if any players need to be kicked.
        CheckBrokePlayers();
    }

    #region Win Triggers
    public void ShowWin(int value)
    {
        GameObject go = GameObject.FindGameObjectWithTag("WinText");
        if(go != null)
        {
            go.GetComponent<Text>().text = "WON $" + value.ToString();
            go.GetComponent<Animator>().SetTrigger("Triggered");
        }
    }

    public void ShowLose()
    {
        GameObject go = GameObject.FindGameObjectWithTag("LoseText");
        if (go != null)
        {
            go.GetComponent<Animator>().SetTrigger("Triggered");
        }
    }

    public void ShowDraw()
    {
        GameObject go = GameObject.FindGameObjectWithTag("DrawText");
        if (go != null)
        {
            go.GetComponent<Animator>().SetTrigger("Triggered");
        }
    }

    #endregion

    /// <summary>
    /// Check if any players need to be remove as they are out of chips
    /// </summary>
    void CheckBrokePlayers()
    {
        // Loop through each player in the game and have them check if they have chips
        foreach(var p in GameManager.Instance.GetPlayersInGame())
        {
            PlayerController player = p.GetComponent<PlayerController>();
            if(player != null)
            {
                player.CheckIfImBroke();
            }
        }
    }



    // --- GETTERS --- //
    public void SetPlayerNum(int num) => PlayerNum = num;
    public GameStatus GetCurrentGameStatus() => CurrentGameStatus;
    public int GetPlayerNum() => PlayerNum;
    public AI_Brain GetBrain() => Brain;
    public PlayerCardController GetCardController() => PlayerCards;
}