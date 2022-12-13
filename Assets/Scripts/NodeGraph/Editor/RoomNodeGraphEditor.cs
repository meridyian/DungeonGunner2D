using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomNodeGraphEditor : EditorWindow
{
    //make editorwindow appear on the unity editor menu
    //create a function that opens the editor, should be static 

    private GUIStyle roomNodeStyle;

    //node layout values
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;




    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]

    private static void OpenWindow()
    {
        //adds a new window with a title to default unity editor windows
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

    private void OnEnable()
    {
        //create a new guı style and construct it, editorguıutility is predefined assets
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

    }

    private void OnGUI()
    {
        //define a GUI style, difference between guistyle and editorguistyle is that editor one is customizable
        //darw a box using the path with the style created 
        GUILayout.BeginArea(new Rect(new Vector2(100f, 100f), new Vector2(nodeWidth, nodeHeight)), roomNodeStyle);
        EditorGUILayout.LabelField("Node1");
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(new Vector2(300f, 300f), new Vector2(nodeWidth, nodeHeight)), roomNodeStyle);
        EditorGUILayout.LabelField("Node2");
        GUILayout.EndArea();

    }
    
    
    
    
    
}
