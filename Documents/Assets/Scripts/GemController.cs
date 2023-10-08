/***************************************************************************
// File Name:       GemController.cs
// Author:          Andrew Stapay
// Creation Date:   October 5, 2023
//
// Description:     Represents a Portal Gem. By walking into it, it can
                    be collected
***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemController : MonoBehaviour
{
    // <summary>
    // Start is called before the first frame update
    // Used to make the gem shoot up upon being spawned
    // </summary>
    void Start()
    {
        // Shoot it up
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 5);
    }

    // <summary>
    // Update is called once per frame
    // Used to make sure a Gem doesn't fall off the map
    // </summary>
    void Update()
    {
        // If it has fallen off, just let the player
        // automatically collect it
        if (transform.position.y < -4)
        {
            PlayerController pc = FindObjectOfType<PlayerController>();

            transform.position = new Vector3(pc.transform.position.x, pc.transform.position.y,
                pc.transform.position.z);
        }
    }

    // <summary>
    // Called when a collider enters this collider
    // Used to collect gems
    // </summary>
    // <param name="collision> Information regarding the other hitbox
    // in the collision </param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If the player touched it, destroy this and increment Gem count
        if (collision.gameObject.name == "PW-350")
        {
            PlayerController pc = collision.gameObject.GetComponent<PlayerController>();

            pc.IncreaseGems();

            Destroy(gameObject);
        }
    }
}
