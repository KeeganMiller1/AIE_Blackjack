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
        foreach(var p in GameManager.Instance.GetPlayersInGame())
        {
            PlayerController player = p.GetComponent<PlayerController>();
            if(player != null)
            {
                if(p.GetComponent<PlayerCardController>().GetCurrentValue() == 21)
                {
                    Naturals.Add(p);
                }
            }
        }

        bool DealerHasNat = false;
        if (PlayerCards.GetCurrentValue() == 21)
            DealerHasNat = true;

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
                    ShowWin(p.  GetComponent<PlayerController>().GetLastBet() * 2);
                }
            }

            EndRound();
        }
    }

    public void PlayTurn()
    {
        GetComponent<AI_Brain>().Action();
    }

    public void EndRound()
    {
        GameManager.Instance.ClearScene();
        ChangeGameStatus(GameStatus.BETTING);
        foreach(var p in GameManager.Instance.GetPlayersInGame())
        {
            p.GetComponent<PlayerCardController>().GetCardsInHand().Clear();
            p.GetComponent<PlayerCardController>().ResetCardPosition();
        }

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

    void CheckBrokePlayers()
    {
        foreach(var p in GameManager.Instance.GetPlayersInGame())
        {
            PlayerController player = p.GetComponent<PlayerController>();
            if(player != null)
            {
                player.CheckIfImBroke();
            }
        }
    }


    public void SetPlayerNum(int num) => PlayerNum = num;
    public GameStatus GetCurrentGameStatus() => CurrentGameStatus;
    public int GetPlayerNum() => PlayerNum;
    public AI_Brain GetBrain() => Brain;
}