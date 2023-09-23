/***************************************************************************
// File Name:       HurtBox.cs
// Author:          Andrew Stapay
// Creation Date:   September 10, 2023
//
// Description:     The HurtBox class for taking attacks. This allows us to
                    detect hits and perform the needed steps to handle them.
***************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    // <summary>
    // Handles any damage taken from a collision with a hitbox
    // The code in this method is temporary
    // </summary>
    // <param name="damage"> The amount of damage we take from the hit </param>
    public void GetHit(int damage)
    {
        Dummy parent = GetComponentInParent<Dummy>();
        parent.ReduceHP(damage);
    }

    // <summary>
    // Draws each hurtbox in the Scene View
    // Very useful for debugging
    // </summary>
    private void OnDrawGizmos()
    {
        // We need the position and size of our hurtbox first
        Vector2 position = GetComponent<BoxCollider2D>().offset;
        Vector2 boxSize = GetComponent<BoxCollider2D>().size;

        // We now use these values to draw a simple wire cube in the Scene
        Gizmos.color = Color.green;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawWireCube(position, boxSize);
    }

    // <summary>
    // Detects when a hitbox collides with a hurtbox
    // The properties of the hitbox (such as damage) are handled
    // The code inside this method is temporary
    // </summary>
    // <param name="other"> The other collider involved in the collision
    // In this case, a HitBox's collider </param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Handle our damage
        GetHit(1);
    }
}
