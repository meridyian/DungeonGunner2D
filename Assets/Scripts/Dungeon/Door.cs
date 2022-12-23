using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.SqlServer.Server;
using UnityEditor.Rendering;
using UnityEngine;


[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class Door : MonoBehaviour
{
    #region Header OBJECT REFERENCES

    [Space(10)]
    [Header("OBJECT REFERENCES")]

    #endregion

    #region Tooltip

    [Tooltip("Populate this with BoxCollider2D component on the DoorCollider gameobject")]

    #endregion

    [SerializeField] private BoxCollider2D doorCollider;

    [HideInInspector] public bool isBossRoomDoor = false;
    private BoxCollider2D doorTrigger;
    private bool isOpen = false;
    private bool previouslyOpened = false;
    private Animator animator;


    private void Awake()
    {
        // disable door collider by default
        doorCollider.enabled = false;

        // load components
        doorTrigger = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // tag the player and if th collided is player open the door
        if (collision.tag == Settings.playerTag || collision.tag == Settings.playerWeapon)
        {
            OpenDoor();
        }
    }

    private void OnEnable()
    {
        // when the parent gameobject is disabled (when the player moves far enough away from the room)
        // the animator state gets reset. Therefore we need to restore the animator state.
        animator.SetBool(Settings.open, isOpen);
    }

    // open the door 
    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            previouslyOpened = true;
            doorCollider.enabled = false;
            doorTrigger.enabled = false;
            
            // set open parameter in animator
            animator.SetBool(Settings.open, true);
        }
    }
    
    // lock the door
    public void LockDoor()
    {
        isOpen = false;
        doorCollider.enabled = true;
        doorTrigger.enabled = false;
        
        // set open to false to close door
        animator.SetBool(Settings.open,false);
    }
    // unlock the door
    public void UnlockDoor()
    {
        doorCollider.enabled = false;
        doorTrigger.enabled = true;

        if (previouslyOpened == true)
        {
            isOpen = false;
            OpenDoor();
        }
    }
    
    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorCollider), doorCollider);
    }
#endif
    #endregion
}
