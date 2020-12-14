using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This enum will be used to help determine the next course of action
/// </summary>
public enum AI_TempAction
{
    Hit,
    Stand,
    Random
}

public class BrainKeys
{
    bool IsTurn = false;

    public void SetBool(string key, bool value)
    {
        switch(key)
        {
            case "IsTurn":
                IsTurn = value;
                break;
            default:
                break;
        }
    }

    public bool GetBool(string key)
    {
        switch(key)
        {
            case "IsTurn":
                return IsTurn;
            default:
                return false;
        }
    }
}

public class AI_Brain : MonoBehaviour
{
    PlayerCardController CardController;
    bool Stand = false;

    [SerializeField] float ActionWaitTime = 2.0f;
    float CurrentWaitTime = 0;
    public BrainKeys Keys = new BrainKeys();


    void Awake()
    {
        // Gets the required components
        CardController = GetComponent<PlayerCardController>();
    }


    public IEnumerator Action()
    {
        // Check if it's the AI's turn 
        if (Keys.GetBool("IsTurn"))
        {
            // Show the cards
            CardController.GetCardsInHand()[0].GetComponent<CardController>().ShowCard();
            yield return new WaitForSeconds(ActionWaitTime);
            // Make sure that the controller is valid
            if (CardController != null)
            {
                // TODO: Check if everyone has busted
                // Get the current value & run a switch to see if there are any pre-determined actions, if there are then perform those actions
                // otherwise let the AI Decide based on their cards and the players cards.
                var total = CardController.GetCurrentValue();
                if(total > 17 || !HasEveryoneBusted())
                {
                    AI_Stand();
                } else
                {
                    DetermineHit();
                }
            }
        }
    }

    void DetermineHit()
    {
        // Get a list of players and set the temp action
        List<GameObject> Players = GameManager.Instance.GetPlayersInGame();
        AI_TempAction NextAction = AI_TempAction.Random;

        // Loop through each player's hand if there value is higher than the dealer hit, if it's equal to the dealer than stand
        foreach(var p in Players)
        {
            if(p.GetComponent<PlayerCardController>().GetCurrentValue() > this.CardController.GetCurrentValue())
            {
                Debug.Log("AI Hit 1");
                NextAction = AI_TempAction.Hit;
                break;
            } else if(p.GetComponent<PlayerCardController>().GetCurrentValue() == this.CardController.GetCurrentValue())
            {
                if(p.GetComponent<PlayerCardController>().GetCurrentValue() > 17)
                {
                    Debug.Log("AI Stand");
                    NextAction = AI_TempAction.Stand;
                    Stand = true;
                } else if(p.GetComponent<PlayerCardController>().GetCurrentValue() < 13)
                {
                    Debug.Log("AI_Hit 2");
                    NextAction = AI_TempAction.Hit;
                } else
                {
                    Debug.Log("AI Random");
                    NextAction = AI_TempAction.Random;
                }
                
            }
        }  
        
        if(NextAction == AI_TempAction.Hit)
        {
            AI_Hit();
        } else if(NextAction == AI_TempAction.Stand)
        {

            AI_Stand();
        } else
        {
            RandomAction();
        }
    }

    void RandomAction()
    {
        var Total = CardController.GetCurrentValue();
        if(Total < 11)
        {
            ActionController.Instance.Hit();
        } else
        {
            switch(Total)
            {
                case 12:
                    ProbAction(0.7f);
                    break;
                case 13:
                    ProbAction(0.64f);
                    break;
                case 14:
                    ProbAction(0.55f);
                    break;
                case 15:
                    ProbAction(0.5f);
                    break;
                case 16:
                    ProbAction(0.4f);
                    break;
                case 18:
                    ProbAction(0.33f);
                    break;
                case 19:
                    ProbAction(0.21f);
                    break;
                case 20:
                    ProbAction(0.11f);
                    break;
                default:
                    ProbAction(0.5f);
                    break;
            }
        }
    }

    void ProbAction(float value)
    {
        // Generate a random value between 0 & 1
        float Range = Random.Range(0.0f, 1.0f);
        // Check if that value is less than the prob, if it is than hit. otherwise stand
        if(Range < value)
        {
            AI_Hit();
            ResetWaitTime();
        } else
        {
            AI_Stand();
        }


    }

    void AI_Stand()
    {
        ActionController.Instance.Stand();
        Keys.SetBool("IsTurn", false);
        Dealer.Instance.ChangeGameStatus(GameStatus.COUNTING);
        Debug.Log("Standing");
        GameManager.Instance.ResetTurns();
    }

    void AI_Hit()
    {
        Dealer.Instance.DealCard(CardController);
        StartCoroutine(Action());
    }

    void CheckIfBust()
    {
        int value = CardController.GetCurrentValue();
        if(CardController == Dealer.Instance.GetCardController())
        {
            var go = GameObject.FindGameObjectWithTag("WinText");
            go.GetComponent<Text>().text = "Win $" + ActionController.Instance.GetPlayer().GetLastBet();
            go.GetComponent<Animator>().SetTrigger("Triggered");
            Dealer.Instance.EndRound();
        }
    }

    bool HasEveryoneBusted()
    {
        
        foreach(var p in GameManager.Instance.GetPlayersInGame())
        {
            if(!p.GetComponent<PlayerCardController>().HasPlayerBust())
            {
                return false;
            }
        }

        return true;
    }

    public void ResetWaitTime() => CurrentWaitTime = 0;
}