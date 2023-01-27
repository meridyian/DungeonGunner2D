using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public class Ammo : MonoBehaviour, IFireable
{
    #region Tooltip

    [Tooltip("Populate with child TrailRenderer component")]

    #endregion Tooltip
    [SerializeField]
    private TrailRenderer trailRenderer;

    private float ammoRange = 0F; // the range of each ammo
    private float ammoSpeed;
    private Vector3 fireDirectionVector;
    private float fireDirectionAngle;
    private SpriteRenderer spriteRenderer;
    private AmmoDetailsSO ammoDetails;
    private float ammoChargeTimer;
    private bool isAmmoMaterialSet = false;
    private bool overrideAmmoMovement;


    private void Awake()
    {
        // cache sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        //ammo charge effect
        if (ammoChargeTimer > 0f)
        {
            ammoChargeTimer -= Time.deltaTime;
            return;
        }
        else if (!isAmmoMaterialSet)
        {
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            isAmmoMaterialSet = true;
            
        }
        
        // Calculate distance vector to move ammo
        Vector3 distanceVector = fireDirectionVector * ammoSpeed * Time.deltaTime;

        transform.position += distanceVector;
        
        //disable after max range reached
        ammoRange -= distanceVector.magnitude;

        if (ammoRange < 0)
        {
            DisableAmmo();
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        DisableAmmo();
    }
    
    
    // Initialise the ammo being fired - using ammodetails, the aimangle, weaponAngle and
    // weaponAimDirectionVector. ıf this ammo is part of a pattern the ammo movement can be overriden by setting
    // overrideAmmoMovement to true


    public void InitialiseAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed,
        Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
    {
        #region Ammo

        this.ammoDetails = ammoDetails;
        
        //Set fire direction
        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);
        
        // set ammo sprite
        spriteRenderer.sprite = ammoDetails.ammoSprite;
        
        // set initial ammo material depending on whether there is an ammo charge period
        if (ammoDetails.ammoChargeTime > 0f)
        {
            // set ammo chatge timer
            ammoChargeTimer = ammoDetails.ammoChargeTime;
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            isAmmoMaterialSet = false;
            
        }
        else
        {
            ammoChargeTimer = 0f;
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            isAmmoMaterialSet = true;
        }
        
        //Set ammo range
        ammoRange = ammoDetails.ammoRange;
        
        // set ammo speed 
        this.ammoSpeed = ammoSpeed;
        
        //override ammo movement
        this.overrideAmmoMovement = overrideAmmoMovement;
        
        gameObject.SetActive(true);

        #endregion Ammo
        
        
        #region Trail

        if (ammoDetails.isAmmoTrail)
        {
            trailRenderer.gameObject.SetActive(true);
            trailRenderer.emitting= true;
            trailRenderer.material = ammoDetails.ammoTrailMaterial;
            trailRenderer.startWidth = ammoDetails.ammoTrailEndWidth;
            trailRenderer.time = ammoDetails.ammoTrailTime;
        }

        else
        {
            trailRenderer.emitting = false;
            trailRenderer.gameObject.SetActive(false);


        }
        #endregion Trail
    }
    
    // set ammo fire direction and angle based on the input angle and direction adjusted by the random speed
    private void SetFireDirection(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle,
        Vector3 weaponAimDirectionVector)
    {
        // calculate random spread angle between min and max
        float randomSpread = Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax);
        
        // get a random spread toggle of 1 or -1
        int spreadToggle = Random.Range(0, 2) * 2 - 1;

        if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance)
        {
            fireDirectionAngle = aimAngle;
            
        }
        else
        {
            fireDirectionAngle = weaponAimAngle;
        }
        
        // adjust ammo fire angle angle by random spread
        fireDirectionAngle += spreadToggle * randomSpread;
        
        // set ammo rotation
        transform.eulerAngles = new Vector3(0f, 0f, fireDirectionAngle);
        
        // set ammo fire direction
        fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);
    }


    private void DisableAmmo()
    {
        gameObject.SetActive(false);
    }

    public void SetAmmoMaterial(Material material)
    {
        spriteRenderer.material = material;
    }


    public GameObject GetGameObject()
    {
        throw new System.NotImplementedException();
    }
    
    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(trailRenderer), trailRenderer);
    }
    
#endif
    #endregion Validation


}
