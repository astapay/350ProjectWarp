/***************************************************************************
// File Name:       HUNDController.cs
// Author:          Andrew Stapay
// Creation Date:   September 9, 2023
//
// Description:     Represents a HUND enemy. They can walk, pounce, bite, etc
***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

public class HUNDController : FighterController
{
    [SerializeField] private int speed;
    [SerializeField] private HitBox hitBox;
    [SerializeField] private GameObject portalGem;
    private GameObject pw350;

    private bool facingRight;
    private bool isMoving;
    private bool canAttack;
    private bool isAttacking;
    private bool gemSpawned;

    private int currentSprite;
    [SerializeField] private Sprite[] walkSprites;
    [SerializeField] private Sprite[] biteSprites;
    [SerializeField] private Sprite[] pounceSprites;
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
                        StartBiteAttack();
                    }
                    else if (canAttack && distFromPW < 3)
                    {
                        StartPounceAttack();
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

                // Begin drawing the hitbox for the attack
                hitBox.StartHitBox(attackType, facingRight, attacks[attackType].damage,
                    attacks[attackType].hitstun, attacks[attackType].knockback);

                // Set our active frame counter to keep track of
                // how long the hitbox exists for
                activeFrameCounter = (attacks[attackType].activeFrames + 1);

                //This line is used for debugging hitbox positions
                //EditorApplication.isPaused = true;
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
                if (attackType == 17)
                {
                    UndoAdjustTransform();
                }
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
                    case 16:
                        {
                            currentSprite %= biteSprites.Length;
                            s = biteSprites[currentSprite];
                        }
                        break;
                    case 17:
                        {
                            currentSprite %= pounceSprites.Length;
                            s = pounceSprites[currentSprite];

                            if (startupFrameCounter > 0)
                            {
                                AdjustTransform(0, 0.255f);

                                if (facingRight)
                                {
                                    transform.position = new Vector2(transform.position.x + 0.15f, transform.position.y);
                                    saveTransform.x = transform.position.x + 0.15f;
                                }
                                else
                                {
                                    transform.position = new Vector2(transform.position.x - 0.15f, transform.position.y);
                                    saveTransform.x = transform.position.x - 0.15f;
                                }
                            }
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

            if(!gemSpawned)
            {
                Instantiate(portalGem, transform.position, Quaternion.identity);

                gemSpawned = true;
            }
        }

        // Render the updated sprite through the SpriteRenderer
        gameObject.GetComponent<SpriteRenderer>().sprite = deathSprites[currentSprite];
    }

    private void StartBiteAttack()
    {
        attackType = 16;
        startupFrameCounter = attacks[attackType].startupFrames;
        activeFrameCounter = 0;
        recoveryFrameCounter = 0;

        currentSprite = -1;
        canAttack = false;
        isAttacking = true;
    }

    private void StartPounceAttack()
    {
        attackType = 17;
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
