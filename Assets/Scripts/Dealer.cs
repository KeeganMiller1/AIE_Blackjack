using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dealer : MonoBehaviour
{
    public static Dealer Instance;

    [Header("Card List")]
    [SerializeField] List<ScriptableObject> Deck = new List<ScriptableObject>();
    [SerializeField] List<ScriptableObject> Played = new List<ScriptableObject>();

    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(this.gameObject);
        }
    }
}