using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // --- OTHER --- //
    PlayerCardController PlayerCards;
    int PlayerGameNumber;

    // --- DETAILS --- //
    bool IsPlayer = true;
    bool IsDealer = false;

    // --- BETTING --- //
    int TotalChips = 100;
    int CurrentBet = 0;
    int LastBet = 0;

    void Awake()
    {
        // Get the required components
        PlayerCards = GetComponent<PlayerCardController>();
    }

    void Start()
    {
        // On start setup the text values
        UpdateBetValue();
    }
    

   
    /// <summary>
    /// Adds to the current bet
    /// </summary>
    /// <param name="BetAmount">The amount to add</param>
    public void AddBet(int BetAmount)
    {
        if (BetAmount <= (TotalChips - CurrentBet))
        {
            CurrentBet += BetAmount;
            UpdateBetValue();
        } else
        {
            Debug.Log("Not Enough Chips");
        }
    }

    /// <summary>
    /// Confirms the bet and adds it to the pot (for the dealer)
    /// </summary>
    public void ConfirmBet()
    {
        Dealer.Instance.AddToPot(CurrentBet);
        TotalChips -= CurrentBet;
        LastBet = CurrentBet;
        CurrentBet = 0;
        UpdateBetValue();
    }

    /// <summary>
    /// Updates the UI elements
    /// </summary>
    void UpdateBetValue()
    {
        // Set the bet details
        var go = GameObject.FindGameObjectWithTag("Bet Text");
        go.GetComponent<Text>().text = "BET: $" + CurrentBet;
        // Set the new balance of the pot
        var currentValue = GameObject.FindGameObjectWithTag("Player Chips");
        currentValue.GetComponent<Text>().text = "$" + TotalChips.ToString();
    }

    /// <summary>
    /// Awards the player chips
    /// </summary>
    /// <param name="value">Ammount the player is being awarded</param>
    public void AddChips(int value)
    {
        TotalChips += value;
        UpdateBetValue();
    }

    /// <summary>
    /// Returns the player to the main menu
    /// </summary>
    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Checks if the player has gone broke
    /// </summary>
    public void CheckIfImBroke()
    {
        if(TotalChips <= 0)
        {
            BackToMenu();
        }
    }


    /// --- GETTERS --- //
    public PlayerCardController GetPlayerCards() => PlayerCards;
    public void SetPlayerNum(int num) => PlayerGameNumber = num;
    public int GetPlayerNum() => PlayerGameNumber;
    public bool IsThisPlayer() => IsPlayer;
    public int GetLastBet() => LastBet;
    public int GetCurrrentBet() => CurrentBet;

}