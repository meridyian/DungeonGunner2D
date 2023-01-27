using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;
using Vector2 = System.Numerics.Vector2;

public static class  HelperUtilities 
{
    //a static validation
    //pass an if the check is empty, 
    
    // public static to hold referance
    public static Camera mainCamera;

    public static Vector3 GetMouseWorldPosition()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        Vector3 mouseScreenPosition = Input.mousePosition;
        
        // Clamp mouse position to screen size
        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.height);

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        worldPosition.z = 0f;
        return worldPosition;


    }
    
    // get the angle in degrees from a direction vector
    public static float GetAngleFromVector(Vector3 vector)
    {
        float radians = Mathf.Atan2(vector.y, vector.x);
        
        // convert radians into degreed
        float degrees = radians * Mathf.Rad2Deg;
        return degrees;

    }

    public static Vector3 GetDirectionVectorFromAngle(float angle)
    {
        Vector3 directionVector = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
        return directionVector;
    }
    
    
    
    
    
    
    // get aimdirection enum value from the passed in angleDegrees

    public static AimDirection GetAimDirection(float angleDegrees)
    {
        AimDirection aimDirection;

        if (angleDegrees >= 22f && angleDegrees <= 67f)
        {
            aimDirection = AimDirection.UpRight;
        }
        else if (angleDegrees > 67f && angleDegrees <= 112f)
        {
            aimDirection = AimDirection.Up;
        }
        else if (angleDegrees > 112f && angleDegrees <= 158f)
        {
            aimDirection = AimDirection.UpLeft;
        }
        else if ((angleDegrees <= 180f && angleDegrees > 158f || (angleDegrees > -180 && angleDegrees <=-135f)))
        {
            aimDirection = AimDirection.Left;
        }
        else if ((angleDegrees > -135f && angleDegrees <=-45f ))
        {
            aimDirection = AimDirection.Down;
        }
        else if((angleDegrees > -45f && angleDegrees <= 0f) || (angleDegrees > 0 && angleDegrees <22f))
        {
            aimDirection = AimDirection.Right;
        }
        else
        {
            aimDirection = AimDirection.Right;
        }

        return aimDirection;
    }
    
    
    
    
    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
    {
        if (stringToCheck == "")
        {
            Debug.Log(fieldName + " is empty and must contain a value in object" + thisObject.name.ToString());
            //have validation error
            return true;
        }

        return false;
    }
    
    // null value debug check

    public static bool ValidateCheckNullValue(Object thisObject, string fieldName, UnityEngine.Object objectToCheck)
    {
        if (objectToCheck == null)
        {
            Debug.Log(fieldName + " is null and must contain a value in object " + thisObject.name.ToString());
            return true;
            
        }

        return false;
    }
    
    
    // checks if the thing you passed is empty
    
    public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;
        
        //add another check

        if (enumerableObjectToCheck == null)
        {
            Debug.Log(fieldName+ " is null in object" + thisObject.name.ToString());
            return true;
        }

        foreach (var item in enumerableObjectToCheck)
        {
            //checks if there is any null value items
            if (item == null)
            {
                Debug.Log(fieldName + " has null values in object" + thisObject.name.ToString()) ;
                error = true;
            }
            else
            {
                count++;
            }
        }

        //if count is 0 no var in enuerable obj. there is error
        if (count == 0)
        {
            Debug.Log(fieldName + " has no values in object" + thisObject.name.ToString());
        }

        return error;
    }
    
    // positive value debug check if zero is allowed set isZeroAllowed to true, retruns true if there is an error
    public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, float  valueToCheck,
        bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debug.Log(fieldName + " must contain a positive value or zero in object" + thisObject.name.ToString());
                error = true;
                
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debug.Log(fieldName + " must contain a positive value in object" + thisObject.name.ToString());
                error = true;
            }
        }

        return error;
    }
    
    
    // positive range debug check - set isZeroAllowed to true if the min and max range values can both be zero. Returns true if there is an error

    public static bool ValidateCheckPositiveRange(Object thisObject, string fieldNameMinimum, float valueToCheckMinimum,
        string fieldNameMaximum, float valueToCheckMaximum, bool isZeroAllowed)
    {
        bool error = false;
        if (valueToCheckMinimum > valueToCheckMaximum)
        {
            Debug.Log(fieldNameMinimum + " must be less than or equal to" + fieldNameMaximum + " in object" + thisObject.name.ToString());
            error = true;
        }

        if (ValidateCheckPositiveValue(thisObject, fieldNameMinimum, valueToCheckMinimum, isZeroAllowed)) error = true;
        
        if (ValidateCheckPositiveValue(thisObject, fieldNameMaximum, valueToCheckMaximum, isZeroAllowed)) error = true;
        
        return error;
    }
    
        
    
    
    
    
    
    
    
    // get the nearest spawn position to the player
    public static Vector3 GetSpawnPositionNearestToPlayer(Vector3 playerPosition)
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Grid grid = currentRoom.instantiatedRoom.grid;

        Vector3 nearestSpawnPosition = new Vector3(10000f, 10000f, 0f);
        
        //loop through room spawn positions
        foreach (Vector2Int spawnPositionGrid in currentRoom.spawnPositionArray)
        {
            //convert the spawn grid position to world position
            Vector3 spawnPositionWorld = grid.CellToWorld((Vector3Int)spawnPositionGrid);

            if (Vector3.Distance(spawnPositionWorld, playerPosition) <
                Vector3.Distance(nearestSpawnPosition, playerPosition))
            {
                nearestSpawnPosition = spawnPositionWorld;
            }
        }

        return nearestSpawnPosition;
    }
    
    
    
    
}
