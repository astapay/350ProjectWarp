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
            case 1: //5N
                {
                    xOffset = 0.2f;
                    yOffset = 0.1f;
                    xSize = 0.4f;
                    ySize = 0.25f;
                }
                break;
            case 2: //5NN
            case 8: //6NN
                {
                    xOffset = 0.25f;
                    yOffset = 0.26f;
                    xSize = 0.3f;
                    ySize = 0.45f;
                }
                break;
            case 3: //5NNN
                {
                    xOffset = 0.35f;
                    yOffset = 0.15f;
                    xSize = 0.35f;
                    ySize = 0.5f;
                }
                break;
            case 4: //2N
                {
                    xOffset = 0.3f;
                    yOffset = -0.4f;
                    xSize = 0.35f;
                    ySize = 0.2f;
                }
                break;
            case 5: //2NN
                {
                    xOffset = 0.35f;
                    yOffset = -0.35f;
                    xSize = 0.35f;
                    ySize = 0.25f;
                }
                break;
            case 6: //2NNN
                {
                    xOffset = 0.25f;
                    yOffset = 0.3f;
                    xSize = 0.35f;
                    ySize = 0.45f;
                }
                break;
            case 7: //6N
                {
                    xOffset = 0;
                    yOffset = 0;
                    xSize = 1;
                    ySize = 1;
                }
                break;
            case 9: //6NNN
                {
                    xOffset = 0;
                    yOffset = 0;
                    xSize = 1;
                    ySize = 1;
                }
                break;
            case 10: //j.N
                {
                    xOffset = 0.35f;
                    yOffset = 0.05f;
                    xSize = 0.3f;
                    ySize = 0.2f;
                }
                break;
            case 11: //j.NN
                {
                    xOffset = 0.3f;
                    yOffset = -0.15f;
                    xSize = 0.45f;
                    ySize = 0.3f;
                }
                break;
            case 12: //j.NNN/j.6N
                {
                    xOffset = 0.4f;
                    yOffset = -0.25f;
                    xSize = 0.3f;
                    ySize = 0.3f;
                }
                break;
            default:
                {
                    xOffset = 0;
                    yOffset = 0;
                    xSize = 0;
                    ySize = 0;
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
