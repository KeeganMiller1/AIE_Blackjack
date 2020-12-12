using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This enum will be used to help determine the next course of action
/// </summary>
public enum AI_TempAction
{
    Hit,
    Stand,
    Random
}

public class AI_Brain : MonoBehaviour
{
    PlayerCardController CardController;
    bool Stand = false;
    void Awake()
    {
        CardController = GetComponent<PlayerCardController>();
    }

    public void Action()
    {

        do
        {
            if (CardController != null)
            {
                var total = CardController.GetCurrentValue();
                switch (total)
                {
                    case 17:
                        ActionController.Instance.Stand();
                        break;
                    default:
                        DetermineHit();
                        break;

                }
            }
        } while (!Stand);

        // Update the statuses
        Dealer.Instance.ChangeGameStatus(GameStatus.COUNTING);
        Stand = false;
    }

    void DetermineHit()
    {
        List<GameObject> Players = GameManager.Instance.GetPlayersInGame();

        AI_TempAction NextAction = AI_TempAction.Random;

        foreach(var p in Players)
        {
            if(p.GetComponent<PlayerCardController>().GetCurrentValue() > this.CardController.GetCurrentValue())
            {
                ActionController.Instance.Hit();
                break;
            } else if(p.GetComponent<PlayerCardController>().GetCurrentValue() == this.CardController.GetCurrentValue())
            {
                NextAction = AI_TempAction.Stand;
                Stand = true;
            }
        }  
        
        if(NextAction == AI_TempAction.Hit)
        {
            ActionController.Instance.Hit();
        } else if(NextAction == AI_TempAction.Stand)
        {
            ActionController.Instance.Stand();
            Stand = true;
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
            ActionController.Instance.Hit();
        } else
        {
            Stand = true;
            ActionController.Instance.Stand();
        }
    }
}