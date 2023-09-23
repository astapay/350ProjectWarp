/**************************************************************************
// File Name:       HitBox.cs
// Author:          Andrew Stapay
// Creation Date:   September 10, 2023
//
// Description:     The HitBox class for attacks. This allows us to reshape
                    and draw the hitboxes as we see fit.
**************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class HitBox : MonoBehaviour
{
    // <summary>
    // When called, begins drawing a hitbox depending on the input attack
    // </summary>
    // <param name="type"> The attack in question in the form of an int </param>
    public void StartHitBox(int type)
    {
        // Offset and size values for our hitbox
        float xOffset = 0;
        float yOffset = 0;
        float xSize = 0;
        float ySize = 0;

        // We will switch on type to determine our values for each of these
        // Keep in mind that these are temporary
        switch (type)
        {
            case 1:
                {
                    xOffset = 1;
                    yOffset = 0;
                    xSize = 1;
                    ySize = 0.5f;
                }
                break;
            case 2:
                {
                    xOffset = 1;
                    yOffset = -0.5f;
                    xSize = 1;
                    ySize = 0.5f;
                }
                break;
            case 3:
                {
                    xOffset = 1;
                    yOffset = 0.5f;
                    xSize = 1;
                    ySize = 0.5f;
                }
                break;
            case 4:
                {
                    xOffset = 1;
                    yOffset = 0;
                    xSize = 1;
                    ySize = 0.5f;
                }
                break;
        }

        // Now, we apply our values to the attached BoxCollider2D
        GetComponent<BoxCollider2D>().offset = new Vector2(xOffset, yOffset);
        GetComponent<BoxCollider2D>().size = new Vector2(xSize, ySize);
    }

    // <summary>
    // When called, essentially deletes the drawn hitbox
    // by setting its offset and size to zero
    // </summary>
    public void StopHitBox()
    {
        // Simply set the offset and size values to Vector2.zero
        GetComponent<BoxCollider2D>().offset = Vector2.zero;
        GetComponent<BoxCollider2D>().size = Vector2.zero;
    }

    // <summary>
    // Draws each hitbox in the Scene View
    // Very useful for debugging
    // </summary>
    private void OnDrawGizmos()
    {
        // We need the position and size of our hitbox first
        Vector2 position = GetComponent<BoxCollider2D>().offset;
        Vector2 boxSize = GetComponent<BoxCollider2D>().size;

        // We now use these values to draw a simple wire cube in the Scene
        Gizmos.color = UnityEngine.Color.red;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawWireCube(position, boxSize);
    }
}
