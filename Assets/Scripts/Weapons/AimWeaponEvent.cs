using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;


[DisallowMultipleComponent]
public class AimWeaponEvent : MonoBehaviour
{
   // create action delegates
   public event Action<AimWeaponEvent, AimWeaponEventArgs> OnWeaponAim;

   public void CallAimWeaponEvent(AimDirection aimDirection, float aimAngle, float weaponAimAngle,
       Vector3 weaponAimDirectionVector)
   {
       OnWeaponAim?.Invoke(this, new AimWeaponEventArgs(){ aimDirection =  aimDirection, aimAngle = aimAngle, weaponAimAngle = weaponAimAngle, weaponAimDirectionVector = weaponAimDirectionVector});
   }
}


// extend event args 
public class AimWeaponEventArgs : EventArgs
{
    public AimDirection aimDirection;
    public float aimAngle;
    public float weaponAimAngle;
    public Vector3 weaponAimDirectionVector;
    
}
