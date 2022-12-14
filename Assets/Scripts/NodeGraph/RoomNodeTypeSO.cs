using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// will be created from menu so:

[CreateAssetMenu(fileName = "RoomNodeType_", menuName = "Scriptable Objects/Dungeon/Room Node Type")]
public class RoomNodeTypeSO : ScriptableObject
{
    // entrance, large etc.
    public string roomNodeTypeName;
    
    #region Header
    [Header("Only flag the RoomNodeTypes that should be visible in the editor")]
    #endregion Header
    public bool displayInNodeGraphEditor = true;
    #region Header
    [Header("One Type should be a corridor")]
    #endregion Header
    public bool isCorridor;
    #region Header
    [Header("One Type should be a corridorNS")]
    #endregion Header
    public bool isCorridorNs;
    #region Header
    [Header("One Type should be a corridorEW")]
    #endregion Header
    public bool isCorridorEw;
    #region Header
    [Header("One Type should be an entrance")]
    #endregion Header
    public bool isEntrance;
    #region Header
    [Header("One Type should be a boss room")]
    #endregion Header
    public bool isBossRoom;
    #region Header
    [Header("One Type should be None (Unassigned)")]
    #endregion Header
    public bool isNone;
    
    
    #region Validation
    //only runs in unity editor
#if UNITY_EDITOR
    //to detect changes in inspector 
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(roomNodeTypeName), roomNodeTypeName);
    }
#endif
    #endregion
}
