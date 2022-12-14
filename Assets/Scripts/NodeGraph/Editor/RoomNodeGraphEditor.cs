using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.MPE;
using UnityEngine;


public class RoomNodeGraphEditor : EditorWindow
{
    //make editorwindow appear on the unity editor menu
    //create a function that opens the editor, should be static 
    private GUIStyle roomNodeStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;
    //to be able to drag
    private RoomNodeSO currentRoomNode = null;
    private RoomNodeTypeListSO roomNodeTypeList;
    

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

        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    //open the room node graph editor window if a room node graph scriptable object asset is double clicked in the inspector
    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;
        //if the clicked go is a room node obj will be created
        if (roomNodeGraph != null)
        {
            OpenWindow();
            currentRoomNodeGraph = roomNodeGraph;
            return true;
        }
        return false;
    }


    //called when things change in editor
    private void OnGUI()
    {
        // ıf a scriptable object of type RoomNodeGraphSO has been selected then process
        if (currentRoomNodeGraph != null)
        {
            //Preocess events, like mouse clicks etc
            ProcessEvents(Event.current);
            
            // draw room nodes
            DrawRoomNodes();
        }

        if (GUI.changed)
            Repaint();
    }

    private void ProcessEvents(Event currentEvent)
    {
        // Get room node that mouse is over if it's null or not currently being dragged
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
        {
            currentRoomNode = IsMouseOverRoomNode(currentEvent);
        }
        // if mouse isn't over a room node
        if (currentRoomNode == null)
        {
            ProcessRoomNodeGraphEvents(currentEvent);
        }
        //else process room node events
        else
        {
            //process room node events
            currentRoomNode.ProcessEvents(currentEvent);
        }
        
        
        
    }
    
    // check if the mouse is o the roomnode

    private RoomNodeSO IsMouseOverRoomNode(Event currentEvent)
    {
        for (int i = currentRoomNodeGraph.roomNodeList.Count - 1; i >= 0; i--)
        {
            if (currentRoomNodeGraph.roomNodeList[i].rect.Contains(currentEvent.mousePosition))
            {
                return currentRoomNodeGraph.roomNodeList[i];
            }
        }

        return null;
    }
    
    
    
    

    private void ProcessRoomNodeGraphEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            //process mousedown events
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            
            default:
                break;
        }
    }

    private void ProcessMouseDownEvent(Event currentEvent)
    {
        //process right click mouse down on graph event (show context menu)
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
    }

    private void ShowContextMenu(Vector2 mousePosition)
    {
        // custom context menu creation
        GenericMenu menu = new GenericMenu();
        
        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        
        menu.ShowAsContext();
    }

    // create a room node at the mouse position
    private void CreateRoomNode(object mousePositionObject)
    {
        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
    }
    
    //overloadded functions, have the same name but different parameters
    //create room node at the mouse position - overloaded to also pass in RoomNodeType
    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;
        //create room node scriptable object asset
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();
        
        //add room node to current room node graph room node list
        currentRoomNodeGraph.roomNodeList.Add(roomNode);
        
        //set room node values
        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph,
            roomNodeType);
        
        //add room node to room node graph scriptable object assert database
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);
        
        AssetDatabase.SaveAssets();

    }

    // draw room nodes in the graph window
    private void DrawRoomNodes()
    {
        // loop through all room nodes and draw them
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.Draw(roomNodeStyle);
        }

        GUI.changed = true;

    }
    

}
