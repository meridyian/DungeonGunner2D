using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    #region Tooltip

    [Tooltip("MovementDetailsSO scriptable object movement details such as speed")]

    #endregion Tooltip

    [SerializeField] private MovementDetailsSO movementDetails;
    
    
    #region Tooltip

    [Tooltip("The player weaponShootPosition gameobject in the hierarchy")]

    #endregion Tooltip

    [SerializeField] private Transform weaponShootPosition;

    private Player player;
    private float moveSpeed;
    private Coroutine playerRollCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    private bool isPlayerRolling = false;
    private float playerRollCooldownTimer = 0f;
    private void Awake()
    {
        player = GetComponent<Player>();
        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Start()
    {
        // create waitforfixed update for use in coroutine
        waitForFixedUpdate = new WaitForFixedUpdate();
        
    }
    
    
    private void Update()
    {
        
        // if player is rollinf then return
        if (isPlayerRolling) return;
        
        // process the player movement input
        MovementInput();
        
        // process the player weapon input
        WeaponInput();
        
        // player roll cooldown timer
        PlayerRollCooldownTimer();

    }

    private void MovementInput()
    {

        // get movement input
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");
        bool rightMouseButtonDown = Input.GetMouseButtonDown(1);
        
        
        // create a direction vector with the movement inputs
        Vector2 direction = new Vector2(horizontalMovement, verticalMovement);
        
        // each time it moves 1 unit except the cases that there are two direction inputs
        // 2(1/2) is greater than one unit so it creating a 1 unit movement on diagonal requires 0.7 on each axis
        
        // adjust distance for diagonal movement 
        if (horizontalMovement != 0f && verticalMovement != 0f)
        {
            direction *= 0.7f;
        }
        
        // if there is movement
        if (direction != Vector2.zero)
        {
            if ((!rightMouseButtonDown))
            {
                // trigger movement event
                player.movementByVelocityEvent.CallMovementByVelocityEvent(direction, moveSpeed);
            }
            // else player roll if not cooling down
            else if (playerRollCooldownTimer <= 0f)
            {
                PlayerRoll((Vector3)direction);
            }
            
        }
        else
        //else trigger idle event
        {
            player.idleEvent.CallIdleEvent();
        }

    }
    
    // player roll
    private void PlayerRoll(Vector3 direction)
    {
        playerRollCoroutine = StartCoroutine(PlayerRollRoutine(direction));
    }

    private IEnumerator PlayerRollRoutine(Vector3 direction)
    {
        //mindistance used to decide wen to exit cprputine loop
        float minDistance = 0.2f;

        isPlayerRolling = true;

        Vector3 targetPosition = player.transform.position + (Vector3)direction * movementDetails.rollDistance;

        while (Vector3.Distance(player.transform.position, targetPosition)> minDistance)
        {
            player.movementToPositionEvent.CallMovementToPositionEvent(targetPosition, player.transform.position, movementDetails.rollSpeed,
                direction, isPlayerRolling);
            
            //yield and wait for fixed update
            yield return waitForFixedUpdate;
            
        }
        isPlayerRolling = false;
        
        // set cooldown timer
        playerRollCooldownTimer = movementDetails.rollCooldownTime;
        player.transform.position = targetPosition;

    }

    private void PlayerRollCooldownTimer()
    {
        if (playerRollCooldownTimer >= 0f)
        {
            playerRollCooldownTimer -= Time.deltaTime;
        }
    }

    private void WeaponInput()
    {
        Vector3 weaponDirection;
        float weaponAngleDegrees, playerAngleDegrees;
        AimDirection playerAimDirection;
        
        // aim weapon input
        AimWeaponInput(out weaponDirection, out weaponAngleDegrees, out playerAngleDegrees, out playerAimDirection);
    }
    
    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, out AimDirection playerAimDirection)
    {
        // get mouse world position
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();
        
        // calculate direction vector of mouse cursor from weapon shoot position
        weaponDirection = (mouseWorldPosition - weaponShootPosition.position);
        
        // calculate direction vector of mouse cursor from player transform position

        Vector3 playerDirection = (mouseWorldPosition - transform.position);
        
        // get weapon to cursor angle
        weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);
        
        // get player to cursor angle
        playerAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);
        
        // set player aim direction
        playerAimDirection = HelperUtilities.GetAimDirection(playerAngleDegrees);
        
        // trigger weapon aim event
        player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if collided with something stop player roll coroutine
        StopPlayerRollRoutine();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // if in collision with something stop player roll coroutine
        StopPlayerRollRoutine();
    }

    private void StopPlayerRollRoutine()
    {
        if (playerRollCoroutine != null)
        {
            StopCoroutine(playerRollCoroutine);
            isPlayerRolling = false;
            
        }
    }


    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
        
    }
    
#endif
    #endregion Validation
    
      
}
