using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardController : MonoBehaviour
{
    List<ScriptableObject> CardsInHand = new List<ScriptableObject>();

    [Header("Prefabs")]
    [SerializeField] GameObject CardPrefab;


    [Header("Card Position")]
    [SerializeField, Tooltip("Start position for the first card")] 
    Vector2 StartPosition;
    Vector2 LastUsedPosition;
    [SerializeField, Tooltip("How much the X Position will increase each card")]
    float CardPositionIncrement;

    void Awake()
    {

    }

    List<ScriptableObject> GetCardsInHand() => CardsInHand;

    public void AddCard(ScriptableObject card)
    {
        // Add the card to the hand
        CardsInHand.Add(card);
        // Spawn the card in the world
        var go = Instantiate(CardPrefab);
        // Update the position variables & set the position of the card
        LastUsedPosition = new Vector2(LastUsedPosition.x + CardPositionIncrement, LastUsedPosition.y);
        go.transform.position = LastUsedPosition;
        // Attach the card this game object
        go.transform.parent = this.transform;
        // Setup the card details
        go.GetComponent<CardController>().Card = card;
        // Add the obejct to alist that can be cleared when needed
        GameManager.Instance.AddObjectInScene(go);
    }
    public void EmptyHand() => CardsInHand.Clear();

}