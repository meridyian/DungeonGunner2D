using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// path where are you gonna display it
[CreateAssetMenu(fileName= "RoomNodeGraph", menuName = "Scriptable Objects/Dungeon/Room Node Graph")]
public class RoomNodeGraphSO : ScriptableObject
{
    // add the core member variables 

    //LİST OF ROOMNODE TYPES
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
    [HideInInspector] public List<RoomNodeSO> roomNodeList = new List<RoomNodeSO>();
    //key will be unique guid
    [HideInInspector] public Dictionary<string, RoomNodeSO> roomNodeDictionary = new Dictionary<string, RoomNodeSO>();
}
