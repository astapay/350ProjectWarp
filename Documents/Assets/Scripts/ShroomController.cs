/***************************************************************************
// File Name:       ShroomController.cs
// Author:          Andrew Stapay
// Creation Date:   October 3, 2023
//
// Description:     Represents a Shroom enemy. They can walk, kick, spread
                    spores, etc.
***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShroomController : FighterController
{
    [SerializeField] private int speed;
    [SerializeField] private HitBox hitBox;
    [SerializeField] private GameObject portalGem;
    [SerializeField] private GameObject spore;
    private GameObject pw350;

    private bool facingRight;
    private bool isMoving;
    private bool canAttack;
    private bool isAttacking;
    private bool gemSpawned;

    private int currentSprite;
    [SerializeField] private Sprite[] walkSprites;
    [SerializeField] private Sprite[] kickSprites;
    [SerializeField] private Sprite[] sporeSprites;
    [SerializeField] private Sprite[] deathSprites;

    private int attackType;
    private int startupFrameCounter;
    private int activeFrameCounter;
    private int recoveryFrameCounter;

    private int waitFrames;
    private int actionFrames;

    // Start is called before the first frame update
    void Start()
    {
        currentSprite = -1;
        pw350 = gm.GetPW();

        facingRight = false;
        isMoving = false;
        canAttack = true;
        isAttacking = false;
        gemSpawned = false;

        startupFrameCounter = 0;
        activeFrameCounter = 0;
        recoveryFrameCounter = 0;

        waitFrames = Random.Range(1, 90);
        actionFrames = Random.Range(1, 15);

        hp = 20;
        hitstunFrames = 0;
        saveTransform = transform.position;
        transformSaved = false;
        SetUpAttacks();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gm.TimeFrozen())
        {
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

            if (hp == 0)
            {
                DeathAnimation();
            }
            else if (waitFrames <= 0 && hitstunFrames <= 0)
            {
                if (transform.position.y < -4)
                {
                    ReduceHP(hp);
                }

                float distFromPW = gm.FindDistance(this.gameObject, pw350);

                if (distFromPW < 7)
                {
                    if (canAttack && distFromPW < 1)
                    {
                        int whichAttack = Random.Range(0, 1);

                        if (whichAttack == 0)
                        {
                            StartKickAttack();
                        }
                        else
                        {
                            StartSporeAttack();
                        }
                    }
                    else if (!isAttacking)
                    {
                        isMoving = true;
                        float currentY = gameObject.GetComponent<Rigidbody2D>().velocity.y;

                        if (pw350.transform.position.x < transform.position.x)
                        {
                            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, currentY);

                            if (facingRight)
                            {
                                InvertHurtboxes();
                                facingRight = false;
                            }
                        }
                        else if (pw350.transform.position.x > transform.position.x)
                        {
                            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(speed, currentY);

                            if (!facingRight)
                            {
                                InvertHurtboxes();
                                facingRight = true;
                            }
                        }
                        else
                        {
                            isMoving = false;
                            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, currentY);
                        }
                    }
                }
                else
                {
                    isMoving = false;
                    float currentY = gameObject.GetComponent<Rigidbody2D>().velocity.y;
                    gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, currentY);
                }

                actionFrames--;


            }
            else
            {
                isMoving = false;
                float currentY = gameObject.GetComponent<Rigidbody2D>().velocity.y;
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, currentY);

                hitstunFrames--;
            }

            waitFrames--;

            // Now, we handle our attack if we are indeed attacking
            // Reduce our startup
            startupFrameCounter--;

            // If startup is over, we begin drawing the hitbox
            if (startupFrameCounter == 0)
            {
                hitBox.StopHitBox();

                if (attackType != 21)
                {
                    // Begin drawing the hitbox for the attack
                    hitBox.StartHitBox(attackType, facingRight, attacks[attackType].damage,
                        attacks[attackType].hitstun, attacks[attackType].knockback);
                }
                else
                {
                    Instantiate(spore, transform.position, Quaternion.identity);
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
                // Stop drawing the hitbox
                hitBox.StopHitBox();

                // Set our recovery frame counter to keep track of
                // our end lag (assuming that we didn't cancel the attack)
                recoveryFrameCounter = (attacks[attackType].recoveryFrames + 1);
            }

            // Reduce our recovery frames
            recoveryFrameCounter--;

            if (recoveryFrameCounter == 0)
            {

            }

            UpdateAnimation();
        }
    }

    private void UpdateAnimation()
    {
        currentSprite++;

        Sprite s = null;

        // Temporarily disable the Animator for the idle animation
        gameObject.GetComponent<Animator>().enabled = false;

        if (hitstunFrames <= 0)
        {
            if (isAttacking)
            {
                switch (attackType)
                {
                    case 20:
                        {
                            currentSprite %= kickSprites.Length;
                            s = kickSprites[currentSprite];
                        }
                        break;
                    case 21:
                        {
                            currentSprite %= sporeSprites.Length;
                            s = sporeSprites[currentSprite];
                        }
                        break;
                    default:
                        gameObject.GetComponent<Animator>().enabled = true;
                        break;
                }
            }
            else if (isMoving)
            {
                currentSprite %= walkSprites.Length;
                s = walkSprites[currentSprite];

                // We also need to update which direction we are looking
                if (facingRight)
                {
                    gameObject.GetComponent<SpriteRenderer>().flipX = true;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().flipX = false;
                }
            }
            else
            {
                gameObject.GetComponent<Animator>().enabled = true;
            }
        }
        else
        {
            s = deathSprites[3];
        }

        gameObject.GetComponent<SpriteRenderer>().sprite = s;
    }

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

            if (!gemSpawned)
            {
                Instantiate(portalGem, transform.position, Quaternion.identity);

                gemSpawned = true;
            }
        }

        // Render the updated sprite through the SpriteRenderer
        gameObject.GetComponent<SpriteRenderer>().sprite = deathSprites[currentSprite];
    }

    private void StartKickAttack()
    {
        attackType = 20;
        startupFrameCounter = attacks[attackType].startupFrames;
        activeFrameCounter = 0;
        recoveryFrameCounter = 0;

        currentSprite = -1;
        canAttack = false;
        isAttacking = true;
    }

    private void StartSporeAttack()
    {
        attackType = 21;
        startupFrameCounter = attacks[attackType].startupFrames;
        activeFrameCounter = 0;
        recoveryFrameCounter = 0;

        currentSprite = -1;
        canAttack = false;
        isAttacking = true;
    }

    // <summary>
    // Draws the HUND's pushbox in Scene View
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
