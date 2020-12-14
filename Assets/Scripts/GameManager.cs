using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EGameState
{
    MAIN_MENU,
    IN_GAME
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] int TotalPlayers;
    int PlayersTurn = 0;

    [Header("Game Status")]
    EGameState GameState;


    [Header("Game Settings | Card Back")]
    [SerializeField] List<Sprite> CardBacks = new List<Sprite>();
    [SerializeField] int SelectedCardBacks = 0;

    [Header("Players")]
    List<GameObject> PlayersInGame = new List<GameObject>();

    List<GameObject> ObjectsInScene = new List<GameObject>();

    void Awake()
    {
        // Create a singleton
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else
        {
            Destroy(this.gameObject);
        }
    }

    public void SetupPlayers()
    {
        // Clear to ensure we are updating the list of active players
        PlayersInGame.Clear();

        // Find all the players in the game and add them to the list
        GameObject[] p_arr = GameObject.FindGameObjectsWithTag("Player");
        foreach (var p in p_arr)
        {
            PlayersInGame.Add(p);
            p.GetComponent<PlayerController>().SetPlayerNum(PlayersInGame.Count - 1);
        }

        // Find the deaer & added them to the game as a player
        PlayersInGame.Add(GameObject.FindGameObjectWithTag("Dealer"));
        PlayersInGame[PlayersInGame.Count - 1].GetComponent<Dealer>().SetPlayerNum(PlayersInGame.Count - 1);
    }

    public void NextPlayerTurn()
    {
        // Increment the player turn
        PlayersTurn++;
        // If each player has had their term
        if (PlayersTurn == PlayersInGame.Count - 1)
        {
            // If the game status is equal to the betting stage than change it to the play state
            if(Dealer.Instance.GetCurrentGameStatus() == GameStatus.BETTING)
            {
                Dealer.Instance.ChangeGameStatus(GameStatus.IN_PLAY);
                PlayersTurn = 0;
                // If the game status is In play than, change it to counting
            } else if(Dealer.Instance.GetCurrentGameStatus() == GameStatus.IN_PLAY)
            {
                // Reset the brain wait time & trigger the key so the AI knows that it's their turn
                // We will change the status of the game from the AI brain
                Dealer.Instance.GetBrain().Keys.SetBool("IsTurn", true);
                StartCoroutine(Dealer.Instance.GetBrain().Action());
                
            }

        }

        
        
    }

    public void ChangeGameState(EGameState NewState) => GameState = NewState;
    EGameState GetGameState() => GameState;

    // -- OBJECT CONTROLLERS -- //
    public void AddObjectInScene(GameObject newObject)
    {
        ObjectsInScene.Add(newObject);
    }

    /// <summary>
    /// Waits 3 seconds and than removes the cards & other objects required from the screen
    /// </summary>
    /// <returns></returns>
    public IEnumerator ClearScene()
    {
        yield return new WaitForSeconds(3.0f);
        foreach (var g in ObjectsInScene)
        {
            Destroy(g);
        }
        ObjectsInScene.Clear();
       
        
    }
    public Sprite GetSelectedCardBack() => CardBacks[SelectedCardBacks];
    public List<GameObject> GetPlayersInGame() => PlayersInGame;
    public int GetPlayerTurn() => PlayersTurn;

    public void ResetTurns()
    {
        PlayersTurn = 0;
    }
}
