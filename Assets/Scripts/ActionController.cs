﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController : MonoBehaviour
{
    public static ActionController Instance;
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(this.gameObject);
        }
    }
    public void Hit()
    {
        if(CheckIfTurn())
        {
            Dealer.Instance.DealCard(GetPlayerCards());
            Debug.Log("Player Hit");
        }
    }

    public void Stand()
    {
        if(CheckIfTurn())
        {
            GameManager.Instance.NextPlayerTurn();
        }
    }

    // --- BETTING ACTIONS --- //
    public void BetFive() => GetPlayer().AddBet(5);
    public void BetTen() => GetPlayer().AddBet(10);
    public void BetTwoFive() => GetPlayer().AddBet(25);
    public void BetFiveZero() => GetPlayer().AddBet(50);
    public void BetOneZeroZero() => GetPlayer().AddBet(100);
    public void ConfirmBet()
    {
        if(GetPlayer().GetCurrrentBet() > 0)
        {
            if (Dealer.Instance.GetCurrentGameStatus() == GameStatus.BETTING)
            {
                GetPlayer().ConfirmBet();
            }
            GameManager.Instance.NextPlayerTurn();
        }
    }


    bool CheckIfTurn()
    {

        List<GameObject> Players = GameManager.Instance.GetPlayersInGame();

        foreach(var p in Players)
        {
            PlayerController controller = p.GetComponent<PlayerController>();
            if(controller != null)
            {
                if(controller.IsThisPlayer())
                {
                    if(controller.GetPlayerNum() == GameManager.Instance.GetPlayerTurn())
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    PlayerCardController GetPlayerCards()
    {
        List<GameObject> Players = GameManager.Instance.GetPlayersInGame();
        foreach(var p in Players)
        {
            if (p.GetComponent<PlayerController>().IsThisPlayer())
                return p.GetComponent<PlayerCardController>();
        }

        return null;
    }
    public PlayerController GetPlayer()
    {
        List<GameObject> Players = GameManager.Instance.GetPlayersInGame();
        foreach (var p in Players)
        {
            if (p.GetComponent<PlayerController>().IsThisPlayer())
                return p.GetComponent<PlayerController>();
        }

        return null;
    }


    public GameObject GetPlayerObject()
    {
        List<GameObject> Players = GameManager.Instance.GetPlayersInGame();
        foreach (var p in Players)
        {
            if (p.GetComponent<PlayerController>().IsThisPlayer())
                return p;
        }

        return null;
    }
}