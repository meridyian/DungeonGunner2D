using System;
using System.Runtime.CompilerServices;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementToPositionEvent))]
[DisallowMultipleComponent]
public class MovementToPosition : MonoBehaviour
{
   private Rigidbody2D rigidBody2D;
   private MovementToPositionEvent movementToPositionEvent;


   private void Awake()
   {
      // load components
      rigidBody2D = GetComponent<Rigidbody2D>();
      movementToPositionEvent = GetComponent<MovementToPositionEvent>();
      
   }

   private void OnEnable()
   {
      //  subscribe to movement to position event
      movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;
   }
   
   private void OnDisable()
   {
      //  subscribe to movement to position event
      movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
   }

   private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent,
      MovementToPositionArgs movementToPositionArgs)
   {
      MoveRigidBody(movementToPositionArgs.movePosition, movementToPositionArgs.currentPosition,
         movementToPositionArgs.moveSpeed);
   }


   private void MoveRigidBody(Vector3 movePosition, Vector3 currentPosition, float moveSpeed)
   {
      Vector2 unitVector = Vector3.Normalize(movePosition - currentPosition);
      
      rigidBody2D.MovePosition(rigidBody2D.position + (unitVector * moveSpeed * Time.fixedDeltaTime));
   }
}
