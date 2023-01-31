using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour
{
    [HideInInspector] public Room room;
    [HideInInspector] public Grid grid;
    [HideInInspector] public Tilemap groundTilemap;
    [HideInInspector] public Tilemap decoration1Tilemap;
    [HideInInspector] public Tilemap decoration2Tilemap;
    [HideInInspector] public Tilemap frontTilemap;
    [HideInInspector] public Tilemap collisionTilemap;
    [HideInInspector] public Tilemap minimapTilemap;
    [HideInInspector] public int[,] aStarMovementPenalty; // use this 2d array to store movement penalties from the tilemaps to be used in AStar pathfinding
    [HideInInspector] public Bounds roomColliderBounds;

    private BoxCollider2D boxCollider2D;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        
        // save room collider bounds
        roomColliderBounds = boxCollider2D.bounds;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if the player triggered the collider
        if (collision.tag == Settings.playerTag && room != GameManager.Instance.GetCurrentRoom())
        {
            // set room as visited
            this.room.isPreviouslyVisited = true;
            
            // call room changed event
            StaticEventHandler.CallRoomChangedEvent(room);
        }
    }

    public void Initialise(GameObject roomGameobject)
    {
        PopulateTilemapMemberVariables(roomGameobject);
        BlockOffUnusedDoorWays();
        AddObstacles();
        AddDoorsToRooms();
        DisableCollisionTilemapRenderer();
        
    }
    
    // populate the tilemap and grid member variables

    private void PopulateTilemapMemberVariables(GameObject roomGameobject)
    {
        // get the grid component
        grid = roomGameobject.GetComponentInChildren<Grid>();
        
        // get tilemaps in children
        Tilemap[] tilemaps = roomGameobject.GetComponentsInChildren<Tilemap>();

        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap.gameObject.tag == "groundTilemap")
            {
                groundTilemap = tilemap;
            }
            else if(tilemap.gameObject.tag == "decoration1Tilemap")
            {
                decoration1Tilemap = tilemap;
            }
            else if(tilemap.gameObject.tag == "decoration2Tilemap")
            {
                decoration2Tilemap = tilemap;
            }
            else if(tilemap.gameObject.tag == "frontTilemap")
            {
                frontTilemap = tilemap;
            }
            else if(tilemap.gameObject.tag == "collisionTilemap")
            {
                collisionTilemap = tilemap;
            }
            else if(tilemap.gameObject.tag == "minimapTilemap")
            {
                minimapTilemap = tilemap;
            }
            
        }
        
    }

    private void BlockOffUnusedDoorWays()
    {
        // loop through all doorways
        foreach (Doorway doorway in room.doorWayList)
        {
            if (doorway.isConnected)
                continue;
            
                // block unconnected doorways using tiles on tilemaps
            if (collisionTilemap != null) 
            {
                BlockADoorwayOnTilemapLayer(collisionTilemap, doorway);
            }
            if (minimapTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(minimapTilemap, doorway);
            }
            if (groundTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(groundTilemap, doorway);
            }
            if (decoration1Tilemap != null)
            {
                BlockADoorwayOnTilemapLayer(decoration1Tilemap, doorway);
            }
            if (decoration2Tilemap != null)
            {
                BlockADoorwayOnTilemapLayer(decoration2Tilemap, doorway);
            }
            if (frontTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(frontTilemap, doorway);
            }
        }
        
    }

    private void BlockADoorwayOnTilemapLayer(Tilemap tilemap, Doorway doorway)
    {
        switch (doorway.orientation)
        {
            case Orientation.north:
            case Orientation.south:
                BlockDoorwayHorizontally(tilemap, doorway);
                break;
            
            case Orientation.east:
            case Orientation.west:
                BlockDoorwayVertically(tilemap, doorway);
                break;
            
            case Orientation.none:
                break;
        }
    }

    private void BlockDoorwayHorizontally(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        for(int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
        {
            for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
            {
                //get rotation of tile being copied
                Matrix4x4 transformMatrix =
                    tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos,0));
                
                // copy tile
                tilemap.SetTile(new Vector3Int(startPosition.x  +1+ xPos, startPosition.y-yPos, 0),tilemap.GetTile(new Vector3Int(startPosition.x + xPos,startPosition.y-yPos,0)));

                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x+1  + xPos, startPosition.y-yPos,0),transformMatrix);
            }
        }
    }


    private void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;
        
        // loop through all tiles to copy
        for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
        {
            for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
            {
                Matrix4x4 transformMatrix =
                    tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));
                
                tilemap.SetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - 1- yPos, 0), tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));
                
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x +xPos, startPosition.y -1 - yPos, 0), transformMatrix);
            }
        }
    }
    
    
    
    // disable collision tilemap renderer

    private void DisableCollisionTilemapRenderer()
    {
        // disable collision tilemap renderer
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }
    
    // update obstacles used by AStar pathfinding
    private void AddObstacles()
    {
        // this array will be populated wit wall obstacles
        aStarMovementPenalty = new int[room.templateUpperBounds.x - room.templateLowerBounds.x + 1,
            room.templateUpperBounds.y - room.templateLowerBounds.y + 1];
        
        // loop through all grid squares
        for (int x = 0; x < (room.templateUpperBounds.x - room.templateLowerBounds.x + 1); x++)
        {
            for (int y = 0; x < (room.templateUpperBounds.y - room.templateLowerBounds.y + 1); y++)
            {
                // set default movement penalty for grid squares
                aStarMovementPenalty[x, y] = Settings.defaultAStarMovementPenalty;
                
                // add obstacles for collision tiles the enemy can't walk on
                TileBase tile = collisionTilemap.GetTile(new Vector3Int(x + room.templateLowerBounds.x,
                    y + room.templateLowerBounds.y, 0));

                foreach (TileBase collisionTile in GameResources.Instance.enemyUnwalkableCollisionTileArray)
                {
                    if (tile == collisionTile)
                    {
                        aStarMovementPenalty[x, y] = 0;
                        break;
                    }
                }
            }
        }
    }
    
    


    private void AddDoorsToRooms()
    {
        // dont want to add doors to corridors, check if it is a corridor
        if (room.roomNodeType.isCorridorEw || room.roomNodeType.isCorridorNs) return;
        
        
        // loop through all of the doorways
        // instantiate door prefab at doorway positions
        foreach (Doorway doorway in room.doorWayList)
        {
            // if the doorway prefab isn't null and the doorway is connected
            if (doorway.doorPrefab != null && doorway.isConnected)
            {
                float tileDistance = Settings.tileSizePixels / Settings.pixelsPerUnit;
                
                // create a temporary gameobject for doors 
                GameObject door = null;

                if (doorway.orientation == Orientation.north)
                {
                    // create door with parent as the room
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f,
                        doorway.position.y + tileDistance, 0f);
                }
                else if (doorway.orientation == Orientation.south)
                {
                    // create door with parent as the room
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f,
                        doorway.position.y, 0f);

                }
                else if (doorway.orientation == Orientation.east)
                {
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance,
                        doorway.position.y + tileDistance * 1.25f, 0f);
                }
                
                else if (doorway.orientation == Orientation.west)
                {
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition =
                        new Vector3(doorway.position.x, doorway.position.y + tileDistance * 1.25f, 0f);
                }
                
                // get door component
                Door doorComponent = door.GetComponent<Door>();
                
                // set if door is part of a boss room
                if (room.roomNodeType.isBossRoom)
                {
                    doorComponent.isBossRoomDoor = true;
                    
                    // lock the door to prevent access to the room
                    doorComponent.LockDoor();
                }
                

            }
            
        }


    }
    

    
}

