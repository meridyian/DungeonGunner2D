using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNodeSO : ScriptableObject
{
    //its assets will be created in editor
    //just create core members

    //system generated gui id
    [HideInInspector] public string id;
    //parent room node id list
    [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();
    [HideInInspector] public List<string> childRoomNodeIDList = new List<string>();
    //containing room node graph
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    //for types
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;

}
