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
using System.Security.Cryptography;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    [SerializeField] private GameManager gm;

    // <summary>
    // Handles any damage taken from a collision with a hitbox
    // The code in this method is temporary
    // </summary>
    // <param name="damage"> The amount of damage we take from the hit </param>
    public void GetHit(int damage, int hitstun, Vector2 knockback)
    {
        FighterController parent = GetComponentInParent<FighterController>();

        parent.ReduceHP(damage);
        parent.ApplyHitstun(hitstun);
        parent.ApplyKnockback(knockback);
    }

    // <summary>
    // Draws each hurtbox in the Scene View
    // Very useful for debugging
    // </summary>
    private void OnDrawGizmos()
    {
        BoxCollider2D[] boxColliders = GetComponents<BoxCollider2D>();

        for (int i = 0; i < boxColliders.Length; i++)
        {
            // We need the position and size of our hurtbox first
            Vector2 position = boxColliders[i].offset;
            Vector2 boxSize = boxColliders[i].size;

            // We now use these values to draw a simple wire cube in the Scene
            Gizmos.color = Color.green;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
            Gizmos.DrawWireCube(position, boxSize);
        }
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
        HitBox hitBox = other.GetComponent<HitBox>();

        //Handle our damage
        GetHit(hitBox.getDamage(), hitBox.getHitstun(), hitBox.getKnockback());

        gm.FreezeTime(10);
    }
}
