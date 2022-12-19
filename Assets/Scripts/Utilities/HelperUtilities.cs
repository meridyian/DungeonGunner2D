using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class  HelperUtilities 
{
    //a static validation

    //pass an if the check is empty, 
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
    public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, int valueToCheck,
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
    
    
    
    
    
    
}
