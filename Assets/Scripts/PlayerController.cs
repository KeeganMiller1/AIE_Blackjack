using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerCardController PlayerCards;

    void Awake()
    {
        PlayerCards = GetComponent<PlayerCardController>();
    }

    PlayerCardController GetPlayerCards() => PlayerCards;

}