/***************************************************************************
// File Name:       ProjectileController.cs
// Author:          Andrew Stapay
// Creation Date:   October 5, 2023
//
// Description:     Represents a projectile and its logic
***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : FighterController
{
    // Instance variables
    [SerializeField] private HitBox hitBox;
    [SerializeField] private int speed;
    public bool movingRight;
    public bool pwProjectile;


    // <summary>
    // Start is called before the first frame update
    // Initializes all of our instance variables
    // </summary>
    void Start()
    {
        if (pwProjectile)
        {
            hitBox.StartHitBox(15, movingRight, 3, 7, new Vector2(0.3f, 0.3f));
        }
        else
        {
            hitBox.StartHitBox(23, movingRight, 3, 20, new Vector2(0.3f, 0.3f));
        }

        gm = FindObjectOfType<GameManager>();
    }

    // <summary>
    // Update is called once per frame
    // Used to control the speed of the projectile
    // </summary>
    void Update()
    {
        // If we are moving right, continue right
        // Otherwise, don't
        if (movingRight)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0);
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, 0);
        }
    }

    // <summary>
    // Called when this object collides with another collider
    // Used to destroy the projectile
    // </summary>
    // <param name="collision"> Information regarding the collision </param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(gameObject.name != collision.gameObject.name)
        {
            Destroy(gameObject);
        }
    }
}
