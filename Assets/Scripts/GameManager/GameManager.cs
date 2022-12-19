using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

//inherited from Singleton Monobehaviour 
//it created a singleton for the game manager and it can be accessed 
// through the instance variable defined in the singleton monobehaviour abstract class

//prevents you from adding a component more than once
[DisallowMultipleComponent] 
public class GameManager : SingletonMonobehaviour<GameManager>
{
    #region Header DUNGEON LEVELS

    [Space(10)]
    [Header("DUNGEON LEVELS")]

    #endregion Header DUNGEON LEVELS

    #region Tooltip

    [Tooltip("Populate with the dungeon level scriptable objects")]

    #endregion Tooltip

    [SerializeField]
    private List<DungeonLevelSO> dungeonLevelList;
    
    #region Tooltip

    [Tooltip("Populate with th starting dungeon level for testing, first level =0")]

    #endregion Tooltip

    [SerializeField] private int currentDungeonLevelListIndex = 0;

    [HideInInspector] public GameState gameState;

    // Start is called before the first frame update
    private void Start()
    {
        gameState = GameState.gameStarted;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleGameState();
        // for testing
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameState = GameState.gameStarted;
        }
        
    }

    private void HandleGameState()
    {
        switch (gameState)
        {
            case GameState.gameStarted:
                // Play first level
                PlayDungeonLevel(currentDungeonLevelListIndex);
                
                gameState = GameState.playingLevel;
                
                break;
        }
    }

    private void PlayDungeonLevel(int dungeonLevelListIndex)
    {
        //build dungeon for level 
        bool dungeonBuiltSuccessfully =
            DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIndex]);

        if (!dungeonBuiltSuccessfully)
        {
            Debug.LogError("Couldn't build dungeon from specified rooms and node graphs");
        }

    }
    
    #region Validation
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
    #endregion Validation
}
