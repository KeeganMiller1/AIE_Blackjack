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
    }
}