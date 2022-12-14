using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class RoomNodeSO : ScriptableObject
{
    //its assets will be created in editor
    //just create core members

    //system generated gui id
    [HideInInspector] public string id;
    //parent and child room node id list
    [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();
    [HideInInspector] public List<string> childRoomNodeIDList = new List<string>();
    //containing room node graph
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    //list for types
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
    
    #region Editor Code
    
    //the following code should only be run in the unity editor
#if UNITY_EDITOR

    [HideInInspector] public Rect rect;

    public void Initialise(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "RoomNode";
        this.roomNodeGraph = nodeGraph;
        this.roomNodeType = roomNodeType;

        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    public void Draw(GUIStyle nodeStyle)
    {
        // Draw nodebox using begin area
        GUILayout.BeginArea(rect, nodeStyle);
        
        //start region to detect popup selection changes
        EditorGUI.BeginChangeCheck();
        
        //Display a popup using the RoomNodeType name values that can be selected from (default to the currently set roomNodeType)
        int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);
        int selection = EditorGUILayout.Popup(" ", selected, GetRoomNodeTypesToDisplay());

        roomNodeType = roomNodeTypeList.list[selection];
        
        if(EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);
        
        GUILayout.EndArea();
    }
    
    
        //Populate a string array with the room node types to display that can be selected

        public string[] GetRoomNodeTypesToDisplay()
        {
            string[] roomArray = new string[roomNodeTypeList.list.Count];
            for (int i = 0; i < roomNodeTypeList.list.Count; i++)
            {
                if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
                {
                    roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
                    
                }
            }

            return roomArray;
            
        }

#endif

    #endregion Editor Code

}
