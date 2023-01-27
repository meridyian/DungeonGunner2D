using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFireable 
{
    //contract between interface and classes

    void InitialiseAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed,
        Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false);

    GameObject GetGameObject();
}
