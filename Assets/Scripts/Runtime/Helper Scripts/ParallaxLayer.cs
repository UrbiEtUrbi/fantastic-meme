using System.Collections.Generic;
using UnityEngine;

public class ParallaxLayer : MonoBehaviour {
    
    //::::::::::::::::::::::::::::://
    // Properties
    //::::::::::::::::::::::::::::://

    private float MinCameraX => GetMinCameraX();
    private float MaxCameraX => GetMaxCameraX();
    private float MinIndex => 1;
    private float MaxIndex => sprites.Length - 2;
    
    //::::::::::::::::::::::::::::://
    // Constants
    //::::::::::::::::::::::::::::://

    private const float screenMargin = 2f;
    
    //::::::::::::::::::::::::::::://
    // Local Fields
    //::::::::::::::::::::::::::::://

    private int midIndex = 1;
    private float minSpriteRendererX, maxSpriteRendererX, moveSpeed;
    private Vector3 previousCameraPosition;
    
    private Camera mainCamera;
    private Sprite[] sprites;
    private SpriteRenderer[] spriteRenderers;

    Vector3 offset;
    List<Vector3> actualPosition = new();
    
    //::::::::::::::::::::::::::::://
    // Unity Callbacks
    //::::::::::::::::::::::::::::://

    private void Awake() {
        // get sprite renderer components (MUST be three)
        spriteRenderers = transform.GetComponentsInChildren<SpriteRenderer>();

        for (int i = spriteRenderers.Length; i < 3; i++)
        {
            var sr = new GameObject().AddComponent<SpriteRenderer>();
            sr.transform.SetParent(transform,false);
            sr.transform.localPosition = default;
        }
        spriteRenderers = transform.GetComponentsInChildren<SpriteRenderer>();

        Debug.Assert(spriteRenderers.Length == 3, "There must be exactly three SpriteRenderer components attached to ParallaxLayer.");
        
        // get the main camera
        mainCamera = Camera.main;
        Debug.Assert(mainCamera, "The main camera could not be found.");
    }

    private void Start() {
        // get the starting position of the camera
        previousCameraPosition = mainCamera.transform.position;
    }

    private void Update() {
        MoveSpriteRenderers();
    }
    
    //-----------------------------//
    // Configuration
    //-----------------------------//



    public void Configure(ParallaxData parallaxData) {
        // set move speed (used when calculating how far to move it)
        moveSpeed = parallaxData.moveSpeed;
        
        // initialise an empty List to hold all Sprites in order (we will convert this to an array when done)
        List<Sprite> spriteList = new List<Sprite>();
        
        // add sprites to list
        foreach (var spriteData in parallaxData.spriteData) {
            for (int i = 0; i < spriteData.count; i++) spriteList.Add(spriteData.sprite);
        }
        
        // convert list to array
        sprites = spriteList.ToArray();
        
        // if we don't have at least three Sprites (one for each SpriteRenderer) we are done
        if (sprites.Length < spriteRenderers.Length) return;
        
        // initialise fields used in for loop
        float totalWidth = 0f;
        
        for (int i = 0; i < spriteRenderers.Length; i++) {
            // get the SpriteRenderer and Sprite (we know index will be in range for both arrays)
            SpriteRenderer spriteRenderer = spriteRenderers[i];
            Sprite sprite = sprites[i];
            
            // update SpriteRenderer with Sprite and sorting order
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingOrder = parallaxData.sortingOrder;

            // increment total width (we will use this to determine starting positions of SpriteRenderers)
            totalWidth += sprite.bounds.size.x;
        }
        offset = parallaxData.offset;
        // get the camera position
        Vector3 cameraPosition = mainCamera.transform.position;

        // get the starting point (this value will increment)
        float x = cameraPosition.x + totalWidth / -2f;
        
        foreach (SpriteRenderer spriteRenderer in spriteRenderers) {
            // get the sprite width
            float spriteWidth = spriteRenderer.sprite.bounds.size.x;

            // update the position of the SpriteRenderer (midpoint)
            spriteRenderer.transform.position = new Vector3(x + spriteWidth / 2f, cameraPosition.y, 0f) + offset;

            actualPosition.Add(spriteRenderer.transform.position);
            // increment x for start of next sprite
            x += spriteWidth;
        }
        
        // recalculate the min and max values of SpriteRenderers (x)
        RecalcSpriteRendererExtents();
    }
    
    //::::::::::::::::::::::::::::://
    // Property Getters
    //::::::::::::::::::::::::::::://

    private float GetMinCameraX() {
        return mainCamera.ViewportToWorldPoint(Vector3.zero).x - screenMargin;
    }

    private float GetMaxCameraX() {
        return mainCamera.ViewportToWorldPoint(Vector3.one).x + screenMargin;
    }

    //::::::::::::::::::::::::::::://
    // Moving SpriteRenderers
    //::::::::::::::::::::::::::::://


    private void MoveSpriteRenderers() {
        // get the new position of the camera
        Vector3 currentCameraPosition = mainCamera.transform.position;

        // get the distances moved by the camera
        float cameraMoveX = currentCameraPosition.x - previousCameraPosition.x;
        float cameraMoveY = currentCameraPosition.y - previousCameraPosition.y;


        // update value for the next time

        // calculate how far to move background
        float moveX = -cameraMoveX * moveSpeed + cameraMoveX;
        float moveY = -cameraMoveY * moveSpeed + cameraMoveY;

        previousCameraPosition = currentCameraPosition;
        for (int i = 0; i < spriteRenderers.Length; i++) {
            // get the current position of the SpriteRenderer
            Vector3 position = actualPosition[i];
            var spriteRenderer = spriteRenderers[i];
            // update X and Y values
            position.x += moveX;
            position.y += moveY;

            // reset to new position
            // ReSharper disable once Unity.InefficientPropertyAccess

            actualPosition[i] = position;
            var delta = currentCameraPosition.x - position.x;
            float deltadif = (delta - (int)delta);
            if (i == 1) {
                Debug.Log($"{ deltadif} {delta}");

                Debug.Log($"{currentCameraPosition.x} {position.x} {position.x + deltadif} {currentCameraPosition.x -(position.x + deltadif) }");
                }
            spriteRenderer.transform.position = new Vector3(position.x + deltadif, (int)position.y, (int)position.z);
        }
        
        // recalculate the min and max values of SpriteRenderers (x)
        RecalcSpriteRendererExtents();
        
        // shuffle SpriteRenderers so they infinite scroll
        while (MinIndex < midIndex && MinCameraX < minSpriteRendererX) ShuffleSpriteRenderersLeft();
        while (MaxIndex > midIndex && MaxCameraX > maxSpriteRendererX) ShuffleSpriteRenderersRight();
    }
    
    private void ShuffleSpriteRenderersLeft() {
        // decrement mid index (this is checked before function is called)
        midIndex--;
        
        // get the LAST SpriteRenderer (this will be reused and moved to the START of the array) and the PREVIOUS sprite (we know this index will be in range)
        SpriteRenderer reuseSpriteRenderer = spriteRenderers[2];
        Sprite sprite = sprites[midIndex - 1];
        
        // update SpriteRenderer with new sprite
        reuseSpriteRenderer.sprite = sprite;
        
        // initialise fields for calcs
        Transform spriteRendererTransform = reuseSpriteRenderer.transform;
        Vector3 position = spriteRendererTransform.position;
        
        // calculate new position for SpriteRenderer and update
        position.x = minSpriteRendererX - sprite.bounds.extents.x;
        spriteRendererTransform.position = position;
        
        // reshuffle the SpriteRenderers (put reused one to the start)
        spriteRenderers[2] = spriteRenderers[1];
        spriteRenderers[1] = spriteRenderers[0];
        spriteRenderers[0] = reuseSpriteRenderer;
        
        // recalculate the min and max values of SpriteRenderers (x)
        RecalcSpriteRendererExtents();
    }

    private void ShuffleSpriteRenderersRight() {
        // increment mid index (this is checked before function is called)
        midIndex++;
        
        // get the FIRST SpriteRenderer (this will be reused and moved to the END of the array) and the NEXT sprite (we know this index will be in range)
        SpriteRenderer reuseSpriteRenderer = spriteRenderers[0];
        Sprite sprite = sprites[midIndex + 1];
        
        // update SpriteRenderer with new sprite
        reuseSpriteRenderer.sprite = sprite;
        
        // initialise fields for calcs
        Transform spriteRendererTransform = reuseSpriteRenderer.transform;
        Vector3 position = spriteRendererTransform.position;
        
        // calculate new position for SpriteRenderer and update
        position.x = maxSpriteRendererX + sprite.bounds.extents.x;
        spriteRendererTransform.position = position;
        
        // reshuffle the SpriteRenderers (put reused one to the end)
        spriteRenderers[0] = spriteRenderers[1];
        spriteRenderers[1] = spriteRenderers[2];
        spriteRenderers[2] = reuseSpriteRenderer;
        
        // recalculate the min and max values of SpriteRenderers (x)
        RecalcSpriteRendererExtents();
    }
    
    //::::::::::::::::::::::::::::://
    // Utilities
    //::::::::::::::::::::::::::::://

    private void RecalcSpriteRendererExtents() {
        // calculate min (x)
        SpriteRenderer firstSpriteRenderer = spriteRenderers[0];
        minSpriteRendererX = firstSpriteRenderer.sprite ? firstSpriteRenderer.transform.position.x - firstSpriteRenderer.sprite.bounds.extents.x : 0f;
        
        // calculate max (x)
        SpriteRenderer lastSpriteRenderer = spriteRenderers[2];
        maxSpriteRendererX = lastSpriteRenderer.sprite ? lastSpriteRenderer.transform.position.x + lastSpriteRenderer.sprite.bounds.extents.x : 0f;
    }
}