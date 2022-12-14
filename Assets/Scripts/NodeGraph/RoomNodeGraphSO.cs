using System.Collections;
using System.Collections.Generic;
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
}
