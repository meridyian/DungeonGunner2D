using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


// it includes roomnodegraphs
// path where are you gonna display it

[CreateAssetMenu(fileName= "RoomNodeGraph", menuName = "Scriptable Objects/Dungeon/Room Node Graph")]
public class RoomNodeGraphSO : ScriptableObject
{
    // add the core member variables 

    // list of roomnode types
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
    // roomnode list
    [HideInInspector] public List<RoomNodeSO> roomNodeList = new List<RoomNodeSO>();
    // key will be unique guid for roomnodes
    [HideInInspector] public Dictionary<string, RoomNodeSO> roomNodeDictionary = new Dictionary<string, RoomNodeSO>();


    private void Awake()
    {
        LoadRoomNodeDictionary();
    }


    private void LoadRoomNodeDictionary()
    {
        roomNodeDictionary.Clear();
        
        //populate dictionary
        foreach (RoomNodeSO node in roomNodeList)
        {
            roomNodeDictionary[node.id] = node;
        }
    }

    public RoomNodeSO GetRoomNode(string roomNodeID)
    {
        if (roomNodeDictionary.TryGetValue(roomNodeID, out RoomNodeSO roomNode))
        {
            return roomNode;
        }
        return null;
    }
    
    
    
    
    #region Editor Code
    
#if UNITY_EDITOR

    [HideInInspector] public RoomNodeSO roomNodeToDrawLineFrom = null;
    [HideInInspector] public Vector2 linePosition;
    
    //repopulate node dictionary every time a change is made in the editor
    public void OnValidate()
    {
        LoadRoomNodeDictionary();
    }
    

    public void SetNodeToDrawConnectionLineFrom(RoomNodeSO node, Vector2 position)
    {
        roomNodeToDrawLineFrom = node;
        linePosition = position;
    }
    
#endif
    #endregion Editor Code


}
