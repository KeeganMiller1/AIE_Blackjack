using System.Collections;
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
                        Dealer.Instance.DealCard(p.GetComponent<PlayerCardController>());
                    }
                    break;
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
    PlayerController GetPlayer()
    {
        List<GameObject> Players = GameManager.Instance.GetPlayersInGame();
        foreach (var p in Players)
        {
            if (p.GetComponent<PlayerController>().IsThisPlayer())
                return p.GetComponent<PlayerController>();
        }

        return null;
    }
}