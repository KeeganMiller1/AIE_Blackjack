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

    List<GameObject> ObjectsInScene = new List<GameObject>();

    void Awake()
    {
        // Create a singleton
        if(Instance = null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else
        {
            Destroy(this.gameObject);
        }
    }

    public void ChangeGameState(EGameState NewState) => GameState = NewState;
    EGameState GetGameState() => GameState;

    // -- OBJECT CONTROLLERS -- //
    public void AddObjectInScene(GameObject newObject)
    {
        ObjectsInScene.Add(newObject);
    }

    public void ClearScene() => ObjectsInScene.Clear();
}
