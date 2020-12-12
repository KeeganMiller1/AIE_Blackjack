using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    PlayerCardController PlayerCards;
    int PlayerGameNumber;

    bool IsPlayer = true;

    int TotalChips = 100;
    int CurrentBet = 0;

    void Awake()
    {
        PlayerCards = GetComponent<PlayerCardController>();
    }

    void Start()
    {
        UpdateBetValue();
    }
    

    public PlayerCardController GetPlayerCards() => PlayerCards;
    public void SetPlayerNum(int num) => PlayerGameNumber = num;
    public int GetPlayerNum() => PlayerGameNumber;
    public bool IsThisPlayer() => IsPlayer;

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

    public void ConfirmBet()
    {
        Dealer.Instance.AddToPot(CurrentBet);
        CurrentBet = 0;
        UpdateBetValue();
    }


    void UpdateBetValue()
    {
        // Set the bet details
        var go = GameObject.FindGameObjectWithTag("Bet Text");
        go.GetComponent<Text>().text = "BET: $" + CurrentBet;
        // Set the new balance of the pot
        var currentValue = GameObject.FindGameObjectWithTag("Player Chips");
        currentValue.GetComponent<Text>().text = TotalChips.ToString();
    }

}