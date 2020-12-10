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
    int PlayersTurn = 1;

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
            PlayersInGame.Add(p);
    }

    public void ChangeGameState(EGameState NewState) => GameState = NewState;
    EGameState GetGameState() => GameState;

    // -- OBJECT CONTROLLERS -- //
    public void AddObjectInScene(GameObject newObject)
    {
        ObjectsInScene.Add(newObject);
    }

    public void ClearScene() => ObjectsInScene.Clear();
    public Sprite GetSelectedCardBack() => CardBacks[SelectedCardBacks];
    public List<GameObject> GetPlayersInGame() => PlayersInGame;
}
