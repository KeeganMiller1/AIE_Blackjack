using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardController : MonoBehaviour
{
    List<ScriptableObject> CardsInHand = new List<ScriptableObject>();

    void Awake()
    {

    }

    List<ScriptableObject> GetCardsInHand() => CardsInHand;

    public void AddCard(ScriptableObject card) => CardsInHand.Add(card);
    public void EmptyHand() => CardsInHand.Clear();

}