/***************************************************************************
// File Name:       Dummy.cs
// Author:          Andrew Stapay
// Creation Date:   September 9, 2023
//
// Description:     Represents a "Dummy" enemy in the game. Dummies have no
                    actions and only exist to take damage from the main
                    character.
***************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Dummy : FighterController
{
    // The sprites for the death animation,
    // and a variable to keep track of which sprite we play next
    private int currentSprite;
    [SerializeField] private Sprite[] deathSprites;

    // <summary>
    // Start is called before the first frame update
    // Initializes the Dummy's HP and sprite counter
    // </summary>
    void Start()
    {
        // Initialize our private variables
        hp = 1;
        currentSprite = -1;
    }

    // <summary>
    // Update is called once per frame
    // Handles what happens when the Dummy dies
    // </summary>
    void Update()
    {
        //If our HP is 0, we die
        if (hp == 0)
        {
            DeathAnimation();
        }
    }

    // <summary>
    // Handles the death of a dummy
    // This includes deactivating its colliders, playing an animation, etc
    // </summary>
    private void DeathAnimation()
    {
        // Disable its idle animation, pushbox, and RigidBody
        gameObject.GetComponent<Animator>().enabled = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        // Increment our sprite tracker
        currentSprite++;

        // If we are going to go out of bounds in our sprite vector,
        // we are going to correct ourselves
        if (currentSprite >= deathSprites.Length)
        {
            currentSprite = deathSprites.Length - 1;
        }

        // Render the updated sprite through the SpriteRenderer
        gameObject.GetComponent<SpriteRenderer>().sprite = deathSprites[currentSprite];
    }

    // <summary>
    // Draws the Dummy's pushbox in Scene View
    // Very useful for debugging
    // </summary>
    private void OnDrawGizmos()
    {
        // We will simply draw a cube around the Dummy's pushbox
        Gizmos.color = Color.white;

        Vector3 boxPos = GetComponent<BoxCollider2D>().bounds.center;
        Vector3 boxSize = GetComponent<BoxCollider2D>().size;

        Gizmos.DrawWireCube(boxPos, boxSize);
    }
}
