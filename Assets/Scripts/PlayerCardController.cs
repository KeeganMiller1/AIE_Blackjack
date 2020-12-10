using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardController : MonoBehaviour
{
    List<ScriptableObject> CardsInHand = new List<ScriptableObject>();

    [Header("Prefabs")]
    [SerializeField] GameObject CardPrefab;


    [Header("Card Position")]
    Vector2 ResetPosition;
    Vector2 LastUsedPosition;
    [SerializeField, Tooltip("How much the X Position will increase each card")]
    float CardPositionIncrement;

    [Header("Current Hand")]
    int HandValue;

    void Start()
    {
        ResetPosition = new Vector2(-0.7f, GetComponent<Transform>().position.y);
        LastUsedPosition = ResetPosition;
    }

    List<ScriptableObject> GetCardsInHand() => CardsInHand;

    public void AddCard(ScriptableObject card, bool ShowCard = true)
    {
        // Add the card to the hand
        CardsInHand.Add(card);
        // Spawn the card in the world
        var go = Instantiate(CardPrefab);
        // Update the position variables & set the position of the card
        LastUsedPosition = new Vector2(LastUsedPosition.x + CardPositionIncrement, LastUsedPosition.y);
        go.transform.position = LastUsedPosition;
        // Attach the card this game object & set the position
        go.transform.parent = this.transform;
        go.transform.localPosition = GetNextPosition();
        
        // Setup the card details
        go.GetComponent<CardController>().Card = card;
        

        if(ShowCard)
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
        CheckForBust();
    }

    private void CheckForBust()
    {
        if(HandValue > 21)
        {
            Debug.Log("Bust: " + HandValue);
        } else
        {
            Debug.Log("Current: " + HandValue);
        }
    }

    Vector2 GetNextPosition()
    {
        LastUsedPosition = new Vector2(LastUsedPosition.x + CardPositionIncrement, 0);
        return LastUsedPosition;
    }


    public void EmptyHand() => CardsInHand.Clear();

    int CalculateHand()
    {
        // Initial value
        int currentValue = 0;
        // Loop through each card that is not an ace first to get a default score
        foreach(var card in CardsInHand)
        {
            var c = card as CardObject;
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
            var card = a as CardObject;
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
            var ace = a as CardObject;
            if(ace.CardType == ECardType.ACE)
            {
                aceValue += 11;
            }
        }

        return aceValue;
    }

}