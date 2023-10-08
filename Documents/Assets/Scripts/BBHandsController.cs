/***************************************************************************
// File Name:       BBHandsController.cs
// Author:          Andrew Stapay
// Creation Date:   October 5, 2023
//
// Description:     Represents the boss that appears at the end of level 2.
                    Handles all logic pertaining to attacks, hitboxes, etc.
***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BBHandsController : FighterController
{
    // Other GameObjects we need
    [SerializeField] private HitBox hitBox;
    [SerializeField] private GameObject portalGem;
    private GameObject pw350;

    // BB-Hands' hands move independently of the body
    // Still, just in case, we'll leave this here
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;

    // bools determining what we can or can't do
    private bool facingRight;
    private bool canAttack;
    private bool isAttacking;
    private bool gemSpawned;

    // Used for updating sprites
    private int currentSprite;
    [SerializeField] private Sprite[] kickSprites;
    [SerializeField] private Sprite[] deathSprites;

    // Attack information
    private int attackType;
    private int startupFrameCounter;
    private int activeFrameCounter;
    private int recoveryFrameCounter;

    // Very basic AI: we have a certain number of frames
    // where we wait, then where we can act
    private int waitFrames;
    private int actionFrames;

    // <summary>
    // Start is called before the first frame update
    // Used to initialize instance variables
    // </summary>
    void Start()
    {
        // Variables accosicated with Serialized Fields
        currentSprite = -1;
        pw350 = gm.GetPW();

        // bools
        facingRight = true;
        canAttack = true;
        isAttacking = false;
        gemSpawned = false;

        // Attack frames
        startupFrameCounter = 0;
        activeFrameCounter = 0;
        recoveryFrameCounter = 0;

        // AI
        waitFrames = Random.Range(1, 90);
        actionFrames = Random.Range(1, 15);

        // Superclass variables
        hp = 75;
        hitstunFrames = 0;
        saveTransform = transform.position;
        transformSaved = false;
        SetUpAttacks();
    }

    // <summary>
    // Update is called once per frame
    // Used to determine AI logic, attacks, etc
    // </summary>
    void Update()
    {
        // Check if the game is paused first
        if (!gm.TimeFrozen())
        {
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

            // Play death animation if dead
            if (hp == 0)
            {
                DeathAnimation();
            }
            // Otherwise, as long as we are allowed to act...
            else if (waitFrames <= 0 && hitstunFrames <= 0)
            {
                // Prevents the player from not getting a Portal Gem
                // should the enemy fall
                // See GemController.Update() for more
                if (transform.position.y < -4)
                {
                    ReduceHP(hp);
                }

                // We need the distance from the main character
                // If it is small enough, we are close to being on-screen
                float distFromPW = gm.FindDistance(this.gameObject, pw350);

                // Check to see if on-screen
                if (distFromPW < 7)
                {
                    // Within close range, the boss will kick
                    if (canAttack && distFromPW < 1)
                    {
                        StartKickAttack();
                    }
                    // Otherwise, we can't move,
                    // so we just turn in the player's direction
                    else if (!isAttacking)
                    {
                        // Keep constant y velocity
                        float currentY = gameObject.GetComponent<Rigidbody2D>().velocity.y;
                        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, currentY);

                        // Player is on left
                        if (pw350.transform.position.x < transform.position.x)
                        {
                            if (facingRight)
                            {
                                InvertHurtboxes();
                                facingRight = false;
                            }
                        }
                        // Player is on right
                        else if (pw350.transform.position.x > transform.position.x)
                        {
                            if (!facingRight)
                            {
                                InvertHurtboxes();
                                facingRight = true;
                            }
                        }
                    }
                }
                else
                {
                    // If not close to on-screen, do nothing
                    float currentY = gameObject.GetComponent<Rigidbody2D>().velocity.y;
                    gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, currentY);
                }

                // Decrement the number of frames we can act
                actionFrames--;
            }
            else
            {
                // Decrement the hitstun to be sure that is not
                // the reason why we didn't move
                float currentY = gameObject.GetComponent<Rigidbody2D>().velocity.y;
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, currentY);

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
                // Stop any current hitboxes being drawn
                hitBox.StopHitBox();

                // Begin drawing the hitbox for the attack
                hitBox.StartHitBox(attackType, facingRight, attacks[attackType].damage,
                    attacks[attackType].hitstun, attacks[attackType].knockback);

                // Set our active frame counter to keep track of
                // how long the hitbox exists for
                activeFrameCounter = (attacks[attackType].activeFrames + 1);
            }

            // Reduce our active frames
            activeFrameCounter--;

            // If active frames are over, we must stop drawing the hitbox
            if (activeFrameCounter == 0)
            {
                // Stop drawing the hitbox
                hitBox.StopHitBox();

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
                    case 22: // BB-Hands Kick
                        {
                            currentSprite %= kickSprites.Length;
                            s = kickSprites[currentSprite];
                        }
                        break;
                    default: // If we find another attack, turn the Animator back on
                        gameObject.GetComponent<Animator>().enabled = true;
                        break;
                }
            }
            else
            {
                // We need to update which direction we are looking
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
        else
        {
            // Play the getting hit sprite
            s = deathSprites[4];
        }

        // Apply the sprite
        gameObject.GetComponent<SpriteRenderer>().sprite = s;
    }

    // <summary>
    // Handles the fighter's death animation
    // Includes allowing it to be passed through
    // and spawning a Portal Gem
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

            // Spawn our Gem if we have not done so already
            if (!gemSpawned)
            {
                Instantiate(portalGem, transform.position, Quaternion.identity);

                gemSpawned = true;
            }
        }

        // Render the updated sprite through the SpriteRenderer
        gameObject.GetComponent<SpriteRenderer>().sprite = deathSprites[currentSprite];
    }

    // <summary>
    // Starts the boss's kick attack
    // </summary>
    private void StartKickAttack()
    {
        // Start setting attack variables
        attackType = 22;
        startupFrameCounter = attacks[attackType].startupFrames;
        activeFrameCounter = 0;
        recoveryFrameCounter = 0;

        currentSprite = -1;
        canAttack = false;
        isAttacking = true;
    }

    // <summary>
    // Returns the boss's hitstun frames
    // Used by the boss's hands
    // </summary>
    public int GetHitstunFrames()
    {
        // Simply return the frames
        return hitstunFrames;
    }

    // <summary>
    // Draws the HUND's pushbox in Scene View
    // Very useful for debugging
    // </summary>
    private void OnDrawGizmos()
    {
        // We will simply draw a cube around BB_Hands's pushbox
        Gizmos.color = Color.white;

        // Getting the position of its pushbox
        Vector3 boxPos = GetComponent<BoxCollider2D>().bounds.center;
        Vector3 boxSize = GetComponent<BoxCollider2D>().size;

        // Draw it
        Gizmos.DrawWireCube(boxPos, boxSize);
    }
}
