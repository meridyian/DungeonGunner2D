using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(AimWeaponEvent))]
[DisallowMultipleComponent]
public class AimWeapon : MonoBehaviour
{
    // subscribe
    #region Tooltip

    [Tooltip("Populate with the Transform from the child WeaponRotationPoint gameobject")]

    #endregion Tooltip

    [SerializeField]
    private Transform weaponRotationPointTransform;


    private AimWeaponEvent aimWeaponEvent;

    private void Awake()
    {
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
    }
    
    // is called always on onenable since it is called all the time when a gameobject is enabled 

    private void OnEnable()
    {
        // subscribe to aim weapon event
        aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
    }
    
    // always include unsubscribe event 
    private void OnDisable()
    {
        aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
    }


    // aim weapon event handler
    private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArgs aimWeaponEventArgs)
    {
        
    }
}
