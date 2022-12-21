using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "MovementDetails_", menuName = "Scriptable Objects/Movement/MovementDetails" )]
public class MovementDetailsSO : ScriptableObject
{
    
    #region Header MOVEMENT DETAILS

    [Space(10)]
    [Header("MOVEMENT DETAILS")]

    #endregion Header

    #region Tooltip

    [Tooltip(
        "The minimum move speed. TheGetMoveSpeed method calculates a random value between the minimum and maximum")]

    #endregion Tooltip

    public float minMoveSpeed = 8f;
    
    #region Tooltip

    [Tooltip(
        "The maximum move speed. The GetMoveSpeed method calculates a random value between the minimum and maximum")]

    #endregion Tooltip

    public float maxMoveSpeed = 8f;
    
    
    //get a random movement speed between the minimum and maximum values

    public float GetMoveSpeed()
    {
        if (minMoveSpeed == maxMoveSpeed)
        {
            return minMoveSpeed;
        }
        else
        {
            return Random.Range(minMoveSpeed, maxMoveSpeed);
        }
    }
    
    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(minMoveSpeed), minMoveSpeed, nameof(maxMoveSpeed),
            maxMoveSpeed, false);
        
    }
    
#endif
    #endregion Validation
}
