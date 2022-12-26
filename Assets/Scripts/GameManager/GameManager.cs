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

    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;
    
    #region Tooltip

    [Tooltip("Populate with th starting dungeon level for testing, first level =0")]

    #endregion Tooltip

    [SerializeField] private int currentDungeonLevelListIndex = 0;

    private Room currentRoom;
    private Room previousRoom;
    private PlayerDetailsSO playerDetails;
    private Player player;

    [HideInInspector] public GameState gameState;

    // bcs gamemanager inherits from singleton 
    // override the awake method in monobehaviour

    protected override void Awake()
    {
        //call base class
        base.Awake();
        
        // set player details - saved in current player scriptable object from the main menu
        playerDetails = GameResources.Instance.currentPlayer.playerDetails;

        InstantiatePlayer();


    }
    
    
    // create player in scene at position
    private void InstantiatePlayer()
    {
        GameObject playerGameObject = Instantiate(playerDetails.playerPrefab);

        player = playerGameObject.GetComponent<Player>();
        
        player.Initialize(playerDetails);
        
    }

    private void OnEnable()
    {
        // subscribe to room changed event
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }
    
    private void OnDisable()
    {
        // unsubscribe to room changed event
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }
    
    // handle room changed event
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        SetCurrentRoom(roomChangedEventArgs.room);
    }

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
    
    // set the current room the player is in
    public void SetCurrentRoom(Room room)
    {
        previousRoom = currentRoom;
        currentRoom = room;
        
        // Debug
        //Debug.Log(room.prefab.name.ToString());
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
        
        // callm static event that room has changed
        StaticEventHandler.CallRoomChangedEvent(currentRoom);
        
        
        // set player roughly mid-room
        player.gameObject.transform.position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x)  / 2f, ( currentRoom.lowerBounds.y +currentRoom.upperBounds.y)/2f, 0);

        player.gameObject.transform.position =
            HelperUtilities.GetSpawnPositionNearestToPlayer(player.gameObject.transform.position);
        

    }
    
    // get the player
    public Player GetPlayer()
    {
        return player;
    }
    
    
    
    
    // get the current room the player is in
    public Room GetCurrentRoom()
    {
        return currentRoom;
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
