using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;


[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonobehaviour<DungeonBuilder>
{
   public Dictionary<string, Room> dungeonBUilderRoomDictionary = new Dictionary<string, Room>();
   private Dictionary<string, RoomTemplateSO> roomTemplateDictionary = new Dictionary<string, RoomTemplateSO>();
   private List<RoomTemplateSO> roomTemplateList = null;
   private RoomNodeTypeListSO roomNodeTypeList;
   private bool dungeonBuildSuccessful;

   protected override void Awake()
   {
      base.Awake();
      
      // Load the room node type list
      LoadRoomNodeTypeList();
      
      //Set dimmed material to fully visible
      GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
   }

   private void LoadRoomNodeTypeList()
   {
      roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
   }
   
   // generate random dungeon, returns true if dungeon built, false if failed

   public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel)
   {
      roomTemplateList = currentDungeonLevel.roomTemplateList;
      
      // Load the scriptable object room templatees into the dictionary
      LoadRoomTemplatesIntoDictionary();

      dungeonBuildSuccessful = false;
      int dungeonBuildAttempts = 0;
      while (!dungeonBuildSuccessful && dungeonBuildAttempts < Settings.maxDungeonBuildAttempts)
      {
         dungeonBuildAttempts++;
         
         // select a random room node graph from the list
         RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentDungeonLevel.roomNodeGraphList);
         int dungeonRebuildAttemptsForNodeGraph = 0;
         dungeonBuildSuccessful = false;
         
         // Loop until dungeon successfully built or more than max attempts for node graphs
         while (!dungeonBuildSuccessful &&
                dungeonRebuildAttemptsForNodeGraph <= Settings.maxDungeonRebuildAttemptsForRoomGraph)
         {
            // Clear dungeon room gameobjects and dungeon room dictionary
            ClearDungeon();

            dungeonRebuildAttemptsForNodeGraph++;
            
            // attempt to build a random dungeon for the slleected room node graph

            dungeonBuildSuccessful = AttemptToBuildRandomDungeon(roomNodeGraph);

            if (dungeonBuildSuccessful)
            {
               //instantiate room gameobjects
               InstantiateRoomGameobjects();
            }
         }
         
      }

      return dungeonBuildSuccessful;
   }
   
   // Load the room templates into the dictionary

   private void LoadRoomTemplatesIntoDictionary()
   {
      // Clear room template dictionary
      roomTemplateDictionary.Clear();
      
      //Load room template list into dictionary
      foreach (RoomTemplateSO roomTemplate in roomTemplateList)
      {
         if (!roomTemplateDictionary.ContainsKey(roomTemplate.guid))
         {
            roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
         }
         else
         {
            Debug.Log("Duplicate Room Template Key In " + roomTemplateList);
         }
      }
   }
   
   // Attempt to randomly build the dungeon for the specified room nodeGraph,
   // returns true if a successful random layout was generated, else returns false if a problem was encountered
   // another attempt is required

   private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)
   {
      //Create open room node queue
      Queue<RoomNodeSO> openRoomNodeQueue = new Queue<RoomNodeSO>();
      
      // add entrance node to room node queue from room node graph
      RoomNodeSO entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance));

      if (entranceNode != null)
      {
         openRoomNodeQueue.Enqueue(entranceNode);
      }
      else
      {
         Debug.Log("no entrance node");
         return false; // dungeon not build
      }
      
      // start wth no room overlaps
      bool noRoomOverlaps = true;
      
      //Process open room nodes queue
      noRoomOverlaps = ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);
      
      // if all the room nodes have been processed and there hasn't been  a room overlap then return true
      if (openRoomNodeQueue.Count == 0 && noRoomOverlaps)
      {
         return true;
      }
      else
      {
         return false;
      }
   }

   private bool ProcessRoomsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue,
      bool noRoomOverlaps)
   {
      
      // while room nodes in open room node queue & no room overlaps detected.
      while (openRoomNodeQueue.Count > 0 && noRoomOverlaps == true)
      {
         // get next room node from open room node queue.
         RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();
         
         // add child nodes to queue from room node graph (with links to this parent room)
         foreach (RoomNodeSO childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode))
         {
            openRoomNodeQueue.Enqueue(childRoomNode);
         }
         
         // if the room is the entrance mark as positioned and add to room dictionary
         if (roomNode.roomNodeType.isEntrance)
         {
            RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
            Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);
            room.isPositioned = true;

            // add room to room dictionary
            dungeonBUilderRoomDictionary.Add(room.id, room);
         }

         // else if the room type isn't an entrance
         else
         {
            // Else get parent room for node
            Room parentRoom = dungeonBUilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];
            
            // See if room can be placed without overlaps
            noRoomOverlaps = CanPlaceRoomWithNoOverlaps(roomNode, parentRoom);
         }
      }

      return noRoomOverlaps;
   }
   
   
   // Attempt to place the room node in the dungeon - if room can be placed return the room, else retrun null


   private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom)
   {
      // initialize and assume overlap until proven otherwise.
      bool roomOverlaps = true;
      
      // Do while room overlaps - try to place against all available doorways of the parent until
      // the room is successfully placed without overlap
      while (roomOverlaps)
      {
         
         // select random unconnected available doorway for parent
         List<Doorway> unconnectedAvailableParentDoorways =
            GetUnconnectedAvailableDoorways(parentRoom.doorWayList).ToList();

         if (unconnectedAvailableParentDoorways.Count == 0)
         {
            //if no more doorways to try then overlap failure.
            return false; // room overlaps
            
         }

         Doorway doorwayParent =
            unconnectedAvailableParentDoorways[UnityEngine.Random.Range(0, unconnectedAvailableParentDoorways.Count)];
         
         
         // Get a random room template for room node that is consistent with the parent door orientation
         RoomTemplateSO roomtemplate = GetRandomTemplateForRoomConsistentWithParent(roomNode, doorwayParent);
         
         // create a room
         Room room = CreateRoomFromRoomTemplate(roomtemplate, roomNode);
         
         // place the room - returns true if the room doesnt overlap

         if (PlaceTheRoom(parentRoom, doorwayParent, room))
         {
            // if room doesnt overlap then set to false to exit while loop
            roomOverlaps = false;
            
            //mark room as positioned
            room.isPositioned = true;
            
            //add room to dictionary
            dungeonBUilderRoomDictionary.Add(room.id, room);
            
         }
         else
         {
            roomOverlaps = true;
         }
      }

      return true; //no room overlaps
   }
   
   
   
   // Get random room template for room node taking into account the parent doorway orientation

   private RoomTemplateSO GetRandomTemplateForRoomConsistentWithParent(RoomNodeSO roomNode, Doorway doorwayParent)
   {
      RoomTemplateSO roomtemplate = null;
      
      //if roomnode is a corridor then select random correct corridor room template based on parent doorway orientattion
      if (roomNode.roomNodeType.isCorridor)
      {
         switch (doorwayParent.orientation)
         {
            case Orientation.north:
            case Orientation.south:
               roomtemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorNs));
               break;
            
            case Orientation.east:
            case Orientation.west:
               roomtemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorEw));
               break;
            
            case Orientation.none:
               break;
            
            default:
               break;
         }
      }
      // else select random room template
      else
      {
         roomtemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
      }

      return roomtemplate;
   }
   
   // Place the room - returns true if the room doesnt overlap, false otherwise
   private bool PlaceTheRoom(Room parentRoom, Doorway doorwayParent, Room room)
   {
      // get the current room doorway position
      Doorway doorway = GetOppositeDoorway(doorwayParent, room.doorWayList);
      
      // return if no doorway in room oppposite to parent doorway
      if (doorway == null)
      {
         // just mark the parent doorway as unavailable so we dont try and connect it again
         doorwayParent.isUnavailable = true;

         return false;
      }
      
      // calculate "world" grid parent doorway position
      Vector2Int parentDoorwayPosition =
         parentRoom.lowerBounds + doorwayParent.position - parentRoom.templateLowerBounds;
      
      Vector2Int adjustment = Vector2Int.zero;
      
      // calculate adjustment position offset based on room doorway position that we are trying to connect (e.g 
      // if this  doorway is west then we need to add (1,0) to the east parent doorway)

      switch (doorway.orientation)
      {
         case Orientation.north:
            adjustment = new Vector2Int(0, -1);
            break;
         case Orientation.east:
            adjustment = new Vector2Int(-1, 0);
            break;
         case Orientation.south:
            adjustment = new Vector2Int(0, 1);
            break;
         case Orientation.west:
            adjustment = new Vector2Int(1, 0);
            break;
         case Orientation.none:
            break;
         default:
            break;
         
      }
      
      // calculate room lower bounds and upper bounds based on positioning to align with parent doorway
      room.lowerBounds = parentDoorwayPosition + adjustment + room.templateLowerBounds - doorway.position;
      room.upperBounds = room.lowerBounds + room.templateUpperBounds - room.templateLowerBounds;

      Room overlappingRoom = CheckForRoomOverlap(room);

      if (overlappingRoom == null)
      {
         // mark  doorways as connected & unavailable
         doorwayParent.isConnected = true;
         doorwayParent.isUnavailable = true;
         
         doorway.isConnected = true;
         doorway.isUnavailable = true;
         
         // return true to show rooms have been connected with no overlap
         return true;
         
      }
      else
      {
         // just mark the parent doorway as unavailable so we dont try and connect it again
         doorwayParent.isUnavailable = true;
         return false;
         
      }

   }
   
   // Get the doorway from the doorway list that has the opposite orientation to doorway
   private Doorway GetOppositeDoorway(Doorway parentDoorway, List<Doorway> doorwayList)
   {
      foreach (Doorway doorwayToCheck in doorwayList)
      {
         if (parentDoorway.orientation == Orientation.east && doorwayToCheck.orientation == Orientation.west)
         {
            return doorwayToCheck;
         }
         else if (parentDoorway.orientation == Orientation.west && doorwayToCheck.orientation == Orientation.east)
         {
            return doorwayToCheck;
         }
         else if (parentDoorway.orientation == Orientation.north && doorwayToCheck.orientation == Orientation.south)
         {
            return doorwayToCheck;
         }
         else if (parentDoorway.orientation == Orientation.south && doorwayToCheck.orientation == Orientation.north)
         {
            return doorwayToCheck;
         }
         
      }

      return null;
   }
   
   
   // check for rooms that overlap the upper and lower bounds parameters, and if there are overlapping rooms then return room else return null

   private Room CheckForRoomOverlap(Room roomToTest)
   {
      // iterate through all the rooms 
      foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBUilderRoomDictionary)
      {
         Room room = keyvaluepair.Value;
         
         // skip if the same room as room to test or room hasnt been positioned
         if(room.id == roomToTest.id || !room.isPositioned)
            continue;
         // if room overlaps
         if (IsOverLappingRoom(roomToTest, room))
         {
            return room;
         }
      }

      return null;
      
   }
   
   // Check if 2 rooms overlap each other - retrun true if they overlap or false if they dont
   private bool IsOverLappingRoom(Room room1, Room room2)
   {
      bool isOverlappingX = IsOverLappingRoomInterval(room1.lowerBounds.x, room1.upperBounds.x, room2.lowerBounds.x,
         room2.upperBounds.x);
      bool isOverlappingY = IsOverLappingRoomInterval(room1.lowerBounds.y, room1.upperBounds.y, room2.lowerBounds.y,
         room2.upperBounds.y);

      if (isOverlappingX && isOverlappingY)
      {
         return true;
      }
      else
      {
         return false;
      }
      
   }
   
   
   // check if interval 1 overlaps interval 2 - this method is used by the IsOverlappingRoom method
   private bool IsOverLappingRoomInterval(int imin1, int imax1, int imin2, int imax2)
   {
      if (Mathf.Max(imin1, imin2) <= Mathf.Min(imax1, imax2))
      {
         return true;
      }
      else
      {
         return false;
      }
   }
   
   
   
   
   // Get a random room template from the roomtemplatelist that matches the roomType and return it
   // (return null if no matching room templates found).

   private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType)
   {
      List<RoomTemplateSO> matchingRoomTemplateList = new List<RoomTemplateSO>();
      //Loop through room template list 
      
      foreach (RoomTemplateSO roomTemplate in roomTemplateList)
      {
         // Add matching room templates
         if (roomTemplate.roomNodeType == roomNodeType)
         {
            matchingRoomTemplateList.Add(roomTemplate);
         }
      }
      //return null if list is zero
      if (matchingRoomTemplateList.Count == 0)
         return null;
      
      //Select random room template from list and return
      return matchingRoomTemplateList[UnityEngine.Random.Range(0, matchingRoomTemplateList.Count)];
      
   }
   
   // get unconnected doorways
   private IEnumerable<Doorway> GetUnconnectedAvailableDoorways(List<Doorway> roomDoorwayList)
   {
      //loop through doorway list
      foreach (Doorway doorway in roomDoorwayList)
      {
         if (!doorway.isConnected && !doorway.isUnavailable)
            yield return doorway;
      }
   }
   
   

   // Create room based on roomTemplate and layoutNode, and return the created room 

   private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode)
   {
      //Initialize room from template
      Room room = new Room();

      room.templateID = roomTemplate.guid;
      room.id = roomNode.id;
      room.prefab = roomTemplate.prefab;
      room.roomNodeType = roomTemplate.roomNodeType;
      room.lowerBounds = roomTemplate.lowerBounds;
      room.upperBounds = roomTemplate.upperBounds;
      room.spawnPositionArray = roomTemplate.spawnPositionArray;
      room.templateLowerBounds = roomTemplate.lowerBounds;
      room.templateUpperBounds = roomTemplate.upperBounds;


      room.childRoomIDList = CopyStringList(roomNode.childRoomNodeIDList);
      room.doorWayList = CopyDoorwayList(roomTemplate.doorwayList);
      
      // set parent ıd for room
      if (roomNode.parentRoomNodeIDList.Count == 0) //Entrance
      {
         room.parentRoomID = "";
         room.isPreviouslyVisited = true;
         
      }
      else
      {
         room.parentRoomID = roomNode.parentRoomNodeIDList[0];
      }

      return room;

   }




   // select a random room node graph from the list of room node graphs

   private RoomNodeGraphSO SelectRandomRoomNodeGraph(List<RoomNodeGraphSO> roomNodeGraphList)
   {
      if (roomNodeGraphList.Count > 0)
      {
         return roomNodeGraphList[UnityEngine.Random.Range(0, roomNodeGraphList.Count)];
         
      }
      else
      {
         Debug.Log("No room node graph in list");
         return null;
      }
   }
   
   // Clear dungeon room gameobjects and dungeon room dictionary

   //Get a room template by room template ID, returns null if ID doesn't exist
   public RoomTemplateSO GetRoomTemplate(string roomTemplateID)
   {
      if(roomTemplateDictionary.TryGetValue(roomTemplateID, out RoomTemplateSO roomTemplate ))
      {
         return roomTemplate;
      }
      else
      {
         return null;
      }
         
   }
   
   // Instantiate the dungeon room gameobject from the prefab
   private void InstantiateRoomGameobjects()
   {
      // Iterate through all dungeon rooms
      foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBUilderRoomDictionary)
      {
         Room room = keyvaluepair.Value;

         Vector3 roomPosition = new Vector3(room.lowerBounds.x, room.lowerBounds.y - room.templateLowerBounds.y, 0f);

         //instantiate room

         GameObject roomGameobject = Instantiate(room.prefab, roomPosition, Quaternion.identity, transform);
         
         // get instantiated room component from instantiated prefab
         InstantiatedRoom instantiatedRoom = roomGameobject.GetComponentInChildren<InstantiatedRoom>();

         instantiatedRoom.room = room;
         
         // initialise the instantiated room
         
         instantiatedRoom.Initialise(roomGameobject);

         // save gameobject referance
         room.instantiatedRoom = instantiatedRoom;

         
      }
 
   }
   
   
   
   
   // Get room by room i, if no room exists with that ıd return null
   public  Room GetRoomByRoomID(string roomID)
   {
      if(dungeonBUilderRoomDictionary.TryGetValue(roomID, out Room room))
      {
         return room;
      }
      else
      {
         return null;
      }
   }
   
   
   
   
   
   
   private void ClearDungeon()
   {
      //destroy instantiated dungeon gameobjects and clear dungeon manager room dictionary
      if (dungeonBUilderRoomDictionary.Count > 0)
      {
         foreach (KeyValuePair<string, Room> keyvaluePair in dungeonBUilderRoomDictionary)
         {
            Room room = keyvaluePair.Value;

            if (room.instantiatedRoom != null)
            {
               Destroy(room.instantiatedRoom.gameObject);
            }
         }
         dungeonBUilderRoomDictionary.Clear();
      }
   }
   
   // create deep copy of doorway list
   private List<Doorway> CopyDoorwayList(List<Doorway> oldDoorwayList)
   {
      List<Doorway> newDoorwayList = new List<Doorway>();

      foreach (Doorway doorway in oldDoorwayList)
      {
         Doorway newDoorway = new Doorway();

         newDoorway.position = doorway.position;
         newDoorway.orientation = doorway.orientation;
         newDoorway.doorPrefab = doorway.doorPrefab;
         newDoorway.isConnected = doorway.isConnected;
         newDoorway.isUnavailable = doorway.isUnavailable;
         newDoorway.doorwayStartCopyPosition = doorway.doorwayStartCopyPosition;
         newDoorway.doorwayCopyTileWidth = doorway.doorwayCopyTileWidth;
         newDoorway.doorwayCopyTileHeight = doorway.doorwayCopyTileHeight;
         
         newDoorwayList.Add(newDoorway);
      }

      return newDoorwayList;
   }







   // create deep copy of string list
   private List<string> CopyStringList(List<string> oldStringList)
   {
      List<string> newStringList = new List<string>();

      foreach (string stringValue in oldStringList)
      {
         newStringList.Add(stringValue);
      }

      return newStringList;
      
   }








}
