using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[DisallowMultipleComponent]
public class DoorLightingControl : MonoBehaviour
{
    private bool isLit = false;
    private Door door;


    private void Awake()
    {
        // get components
        door = GetComponentInParent<Door>();
    }
    
    // fade in door
    public void FadeInDoor(Door door)
    {
        // create new material to fade in
        Material material = new Material(GameResources.Instance.variableLitShader);
        
        
        if (!isLit)
        {
            // get all of the sprite renderers into array of type sprite renderer
            SpriteRenderer[] spriteRendererArray = GetComponentsInParent<SpriteRenderer>();

            foreach (SpriteRenderer spriteRenderer in spriteRendererArray)
            {
                StartCoroutine(FadeInDoorRoutine(spriteRenderer, material));
                
            }

            isLit = true;
            

        }
    }

    // fade in door coroutine
    private IEnumerator FadeInDoorRoutine(SpriteRenderer spriteRenderer, Material material)
    {
        spriteRenderer.material = material;

        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        spriteRenderer.material = GameResources.Instance.litMaterial;
        
    }
    
    // fade door in if triggered 
    private void OnTriggerEnter2D(Collider2D collision)
    {
       FadeInDoor(door);
    }
}
