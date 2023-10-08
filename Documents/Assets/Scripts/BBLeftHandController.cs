/***************************************************************************
// File Name:       BBLeftHandController.cs
// Author:          Andrew Stapay
// Creation Date:   October 5, 2023
//
// Description:     Represents the boss's left hand. It will follow the
                    player and shoot projectile at them
***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BBLeftHandController : FighterController
{
    // Variables related to GameObjects
    [SerializeField] private int speed;
    [SerializeField] private GameObject projectilePrefab;
    private GameObject pw350;

    // Bools for what we can or can't do
    private bool facingRight;
    private bool canAttack;
    private bool isAttacking;

    // Sprite Handlers
    private int currentSprite;
    [SerializeField] private Sprite[] fireSprites;

    // Attack info
    private int attackType;
    private int startupFrameCounter;
    private int activeFrameCounter;
    private int recoveryFrameCounter;

    // Basic AI: wait for a certain amount of time,
    // then act for a certain amount of time
    private int waitFrames;
    private int actionFrames;

    // <summary>
    // Start is called before the first frame update
    // Used to initialize instance variables
    // </summary>
    void Start()
    {
        // Variables related to Serialized Fields
        currentSprite = -1;
        pw350 = gm.GetPW();

        // Bools
        facingRight = true;
        canAttack = true;
        isAttacking = false;

        // Attack info
        attackType = 0;
        startupFrameCounter = 0;
        activeFrameCounter = 0;
        recoveryFrameCounter = 0;

        // AI frames
        waitFrames = Random.Range(1, 90);
        actionFrames = Random.Range(1, 15);

        // Superclass variables
        hp = 20;
        hitstunFrames = 0;
        saveTransform = transform.position;
        transformSaved = false;
        SetUpAttacks();
    }

    // <summary>
    // Update is called once per frame
    // Used for logic with AI, movement, etc
    // </summary>
    void Update()
    {
        // Check if the game is paused first
        if (!gm.TimeFrozen())
        {
            // We can get our hitstun from BB-Hands
            hitstunFrames = GetComponentInParent<BBHandsController>().GetHitstunFrames();

            // Reset our AI frames
            if (actionFrames <= 0)
            {
                waitFrames = Random.Range(1, 90);
                actionFrames = Random.Range(1, 15);
            }

            // If all of our attack frame counters are below 0,
            // then we surely aren't attacking
            if (startupFrameCounter <= 0 && activeFrameCounter <= 0 && recoveryFrameCounter <= 0)
            {
                canAttack = true;
                isAttacking = false;
                attackType = 0;
            }

            // Otherwise, as long as we are allowed to act...
            if (waitFrames <= 0 && hitstunFrames <= 0)
            {
                // We need the distance from the main character
                // If it is small enough, we are close to being on-screen
                float distFromPW = gm.FindDistance(this.gameObject, pw350);

                // Check to see if on-screen
                if (distFromPW < 7)
                {
                    // If we are horizontal to the player, we will fire
                    if (canAttack && Mathf.Abs(transform.position.y - pw350.transform.position.y) < 0.5f)
                    {
                        StartFireAttack();
                    }
                    // Otherwise, we will follow the player
                    else if (!isAttacking)
                    {
                        // Get the x and y distances from the player
                        float xChange = pw350.transform.position.x - transform.position.x;
                        float yChange = pw350.transform.position.y - transform.position.y;

                        // Find the magnitude
                        float mag = Mathf.Sqrt(xChange * xChange + yChange * yChange);

                        // Create a Vector in the direction of the player
                        Vector2 temp1 = new Vector2(xChange / mag, yChange / mag);
                        Vector2 temp2 = new Vector2(speed * temp1.x, speed * temp1.y);

                        // Keep up velocity
                        GetComponent<Rigidbody2D>().velocity = temp2;

                        // Turn around to keep up with the player
                        if (temp2.x < 0)
                        {
                            facingRight = false;
                        }
                        else if (temp2.x > 0)
                        {
                            facingRight = true;
                        }
                    }
                }
                else
                {
                    // We can't move
                    gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                }

                // Decrement the number of frames we can act
                actionFrames--;
            }
            else
            {
                // Decrement the hitstun to be sure that is not
                // the reason why we didn't move
                gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                hitstunFrames--;
            }

            //Reduce waiting frames
            waitFrames--;

            // Now, we handle our attack if we are indeed attacking
            // Reduce our startup
            startupFrameCounter--;

            // If startup is over, we begin drawing the hitbox
            if (startupFrameCounter == 0)
            {
                // We will go ahead and fire
                // Get direction of projectile
                float xOffset = transform.position.x + 1;
                float yOffset = transform.position.y;

                // Account for hand facing left
                if (!facingRight)
                {
                    xOffset = transform.position.x - 1;
                }

                // Put it in a Vector
                Vector2 temp = new Vector2(xOffset, yOffset);

                // Create the projectile
                GameObject pc = Instantiate(projectilePrefab, temp, Quaternion.identity);

                // It is not the player's projectile
                pc.GetComponent<ProjectileController>().pwProjectile = false;

                // Set direction of projectile
                if (facingRight)
                {
                    pc.GetComponent<ProjectileController>().movingRight = true;
                }
                else
                {
                    pc.GetComponent<ProjectileController>().movingRight = false;
                }

                // Set our active frame counter to keep track of
                // how long the hitbox exists for
                activeFrameCounter = (attacks[attackType].activeFrames + 1);
            }

            // Reduce our active frames
            activeFrameCounter--;

            // If active frames are over, we must stop drawing the hitbox
            if (activeFrameCounter == 0)
            {
                // Set our recovery frame counter to keep track of
                // our end lag (assuming that we didn't cancel the attack)
                recoveryFrameCounter = (attacks[attackType].recoveryFrames + 1);
            }

            // Reduce our recovery frames
            recoveryFrameCounter--;

            // Update our animation
            UpdateAnimation();
        }
    }

    // <summary>
    // Updates the currently playing animation, if applicable
    // </summary>
    private void UpdateAnimation()
    {
        // Sprite counter goes up, and create a sprite
        currentSprite++;
        Sprite s = null;

        // Temporarily disable the Animator for the idle animation
        gameObject.GetComponent<Animator>().enabled = false;

        // Special check to see if we are getting hit
        if (hitstunFrames <= 0)
        {
            // If we are attacking, then we should play the right sprite
            if (isAttacking)
            {
                switch (attackType)
                {
                    case 23: // BB-Hands Fire
                        {
                            currentSprite %= fireSprites.Length;
                            s = fireSprites[currentSprite];
                        }
                        break;
                    default: // If we find another attack, turn the Animator back on
                        {
                            gameObject.GetComponent<Animator>().enabled = true;
                        }
                        break;
                }
            }
            else
            {
                // We also need to update which direction we are looking
                if (facingRight)
                {
                    gameObject.GetComponent<SpriteRenderer>().flipX = false;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().flipX = true;
                }

                gameObject.GetComponent<Animator>().enabled = true;
            }
        }

        // Apply the sprite
        gameObject.GetComponent<SpriteRenderer>().sprite = s;
    }

    // <summary>
    // Starts the boss's fire attack
    // </summary>
    private void StartFireAttack()
    {
        // Set all attacking variables
        attackType = 23;
        startupFrameCounter = attacks[attackType].startupFrames;
        activeFrameCounter = 0;
        recoveryFrameCounter = 0;

        currentSprite = -1;
        canAttack = false;
        isAttacking = true;
    }
}
