using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerCardController PlayerCards;
    int PlayerGameNumber;

    bool IsPlayer = true;

    void Awake()
    {
        PlayerCards = GetComponent<PlayerCardController>();
    }

    PlayerCardController GetPlayerCards() => PlayerCards;
    public void SetPlayerNum(int num) => PlayerGameNumber = num;
    public int GetPlayerNum() => PlayerGameNumber;

    public bool IsThisPlayer() => IsPlayer;

}