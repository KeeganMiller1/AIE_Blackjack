using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCardController : MonoBehaviour
{
    List<GameObject> CardsInHand = new List<GameObject>();

    [Header("Prefabs")]
    [SerializeField] GameObject CardPrefab;


    [Header("Card Position")]
    Vector2 ResetPosition;
    Vector2 LastUsedPosition;
    [SerializeField, Tooltip("How much the X Position will increase each card")]
    float CardPositionIncrement;

    [Header("Current Hand")]
    int HandValue;
    bool Bust = false;

    void Start()
    {
        // Set the reset position & the last position to be the reset position
        ResetPosition = new Vector2(2.0f, GetComponent<Transform>().position.y);
        LastUsedPosition = ResetPosition;
    }

    public List<GameObject> GetCardsInHand() => CardsInHand;

    public void AddCard(ScriptableObject card, bool ShowCard = true)
    {
        // Add the card to the hand
        
        // Spawn the card in the world
        var go = Instantiate(CardPrefab);
        // Update the position variables & set the position of the card
        //LastUsedPosition = new Vector2(LastUsedPosition.x + CardPositionIncrement, 0);
        go.transform.position = GetNextPosition();
        // Attach the card this game object & set the position
        go.transform.parent = this.transform;
        go.transform.localPosition = GetNextPosition();
        
        // Setup the card details
        go.GetComponent<CardController>().Card = card;
        CardsInHand.Add(go);

        // Show or hide the card
        if (ShowCard)
        {
            go.GetComponent<CardController>().ShowCard();
        } else
        {
            go.GetComponent<CardController>().HideCard();
        }



        // Add the obejct to alist that can be cleared when needed
        GameManager.Instance.AddObjectInScene(go);
        // Get the current value of the players hand
        HandValue = CalculateHand();
        CheckHandStatus();
    }

    public void DoubleDown()
    {
        int last_bet = this.gameObject.GetComponent<PlayerController>().GetLastBet();
        int chips = this.gameObject.GetComponent<PlayerController>().GetTotalChips();

        if(last_bet < chips)
        {
            this.gameObject.GetComponent<PlayerController>().SetDoubleLastBet();
            Dealer.Instance.DealCard(this, true);
            GameManager.Instance.NextPlayerTurn();
        }
    }

    public void CheckIfCanSplit()
    {
        int value_1 = 0;
        int value_2 = 0;
        bool FaceCard = false;           
        foreach(var card in CardsInHand)
        {
            var c = card.GetComponent<CardController>().Card as CardObject;
            if(value_1 == 0)
            {
                if(c.CardType == ECardType.STANDARD)
                {
                    value_1 = c.CardValue;
                } else
                {
                    value_1 = CreateCardValue(c.CardType);
                }
            } else
            {
                if(c.CardType == ECardType.STANDARD)
                {
                    value_2 = c.CardValue;
                } else
                {
                    value_2 = CreateCardValue(c.CardType);
                }
            }
        }


        if(value_1 == value_2)
        {
            // TODO: Display Split Button
            
        }
    }

    int CreateCardValue(ECardType type)
    {
        switch(type)
        {
            case ECardType.JACK:
                return 12;
            case ECardType.QUEEN:
                return 13;
            case ECardType.KING:
                return 14;
            case ECardType.ACE:
                return 11;
            default:
                return 10; 
        }

        return 10;
    }


    /// <summary>
    /// Checks the current value of the hand to see if it's a win or lost, or continue playing
    /// </summary>
    private void CheckHandStatus()
    {
        // If the current value of the hand is over 21, than set the best to true find the losing object and trigger the animation
        // Than set the next players turn
        if(HandValue > 21)
        {
            Bust = true;
            GameObject go = GameObject.FindGameObjectWithTag("LoseText");
            if(go != null)
            {
                go.GetComponent<Animator>().SetTrigger("Triggered");
            }
            GameManager.Instance.NextPlayerTurn();
        }
    }

    /// <summary>
    /// This method will retrieve the next position for the card
    /// </summary>
    /// <returns></returns>
    Vector2 GetNextPosition()
    {
        LastUsedPosition = new Vector2(LastUsedPosition.x + CardPositionIncrement, 0);
        return LastUsedPosition;
    }


    // This method will clear the current hand
    public void EmptyHand() => CardsInHand.Clear();

    int CalculateHand()
    {
        // Initial value
        int currentValue = 0;
        // Loop through each card that is not an ace first to get a default score
        foreach(var card in CardsInHand)
        {
            var c = card.GetComponent<CardController>().Card as CardObject;
            if(c.CardType != ECardType.ACE)
            {
                currentValue += c.CardValue;
            }
        }

        if(totalAces() > 0)
        {
            // Check the amount of aces
            if (totalAces() > 1)         // If there is more than one 
            {
                // and the value is greater than 10 add 1 point for each card
                if (currentValue > 10)
                {
                    currentValue += totalAces();
                }
                else
                {
                    // if the value is not larger than 10, add all the aces together to check whether we can use an 11 
                    // or if we have to use 1's 
                    var aceValue = TotalValueOfAces();
                    var tempValue = aceValue + currentValue;
                    if (tempValue > 21)
                    {
                        currentValue += totalAces();
                    }
                    else if (tempValue < 21)
                    {
                        currentValue += 1;
                        currentValue += (totalAces() - 1);
                    }
                }
            }
            else
            {
                // If there is only 1 ace determine whether it should be used as a 1 or an 11
                if (currentValue > 10)
                {
                    currentValue += 1;
                }
                else
                {
                    currentValue += 11;
                }
            }
        }

        return currentValue;
    }

    /// <summary>
    /// This method will detemine the amount of aces in the players hand
    /// </summary>
    /// <returns></returns>
    int totalAces()
    {
        int aces = 0;
        foreach(var a in CardsInHand)
        {
            var card = a.GetComponent<CardController>().Card as CardObject;
            if (card.CardType == ECardType.ACE)
                aces++;
        }

        return aces;
    }

    /// <summary>
    /// This method calculates the total value of the players hand
    /// </summary>
    /// <returns></returns>
    int TotalValueOfAces()
    {
        int aceValue = 0;

        foreach(var a in CardsInHand)
        {
            var card = a.GetComponent<CardController>().Card as CardObject;
            if (card.CardType == ECardType.ACE)
            {
                aceValue += 11;
            }
        }

        return aceValue;
    }


    public int GetCurrentValue() => HandValue;
    public bool HasPlayerBust() => Bust;
    public void ResetCardPosition() => LastUsedPosition = ResetPosition;
}